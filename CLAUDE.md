# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project overview

Drone Sport is a Unity competitive multiplayer FPV-drone-racing-style game. Two teams of three fly drones through gates to score points within a 5-minute match. Full gameplay rules (gate types, scoring formula, match structure, variation ideas not yet implemented) are documented in `Drone Sport.md` — read it before working on any scoring/gate/match logic, since the code should match those rules exactly.

Unity version: `6000.5.3f1` (see `ProjectSettings/ProjectVersion.txt`). Render pipeline: URP. Networking: [Mirror](https://mirror-networking.com/) (vendored in `Assets/Mirror`). Input: the new Unity Input System (`com.unity.inputsystem`).

## Working in this repo (no git, GUI steps needed)

This directory is **not a git repository** — there is no version control here, so don't suggest git commands.

Development is split by necessity: Claude edits `.cs` scripts and `.asmdef` files; the user performs anything that requires the Unity Editor GUI (creating/wiring prefabs, building Canvas/UI hierarchies, setting Inspector fields, registering scenes in Build Settings, entering Play mode). When a task needs both, do the code half and clearly enumerate the manual Editor steps the user still needs to do — don't assume they're done.

## Commands

- **Run EditMode tests**: Unity Editor → Window → General → Test Runner → EditMode tab → Run All. There is no CLI test runner configured in this repo; tests are C# NUnit under `Assets/Tests/EditMode/` (asmdef: `DroneSport.Tests.EditMode`, only compiles in the Editor).
- **Build/Play**: no command-line build script exists. Playtesting happens inside the Editor (Play mode) or via a standalone build made through File → Build Settings.
- **Local two-client multiplayer testing** (no second person needed): make a standalone build, run the `.exe` and click Host (becomes server + player 1), then in the Editor enter Play mode and Join/Client against `localhost` (the default `networkAddress`). This is the only way to verify Mirror networking end-to-end short of two machines.
- There is no linter or formatter configured; match the existing code style (see below).

## Architecture

### Assembly layout
Three custom asmdefs, one-directional dependency chain:
- `DroneSport.Runtime` (`Assets/Scripts/DroneSport.Runtime.asmdef`) — all gameplay/physics/input/networking code. References `Unity.InputSystem`, `Mirror`, `Mirror.Components`.
- `DroneSport.UI` (`Assets/Scripts/UI/`) — references `DroneSport.Runtime`, `Mirror`, TextMeshPro, `UnityEngine.UI`.
- `DroneSport.Tests.EditMode` (`Assets/Tests/EditMode/`) — references `DroneSport.Runtime` + NUnit, Editor-only.

**Important constraint**: `DroneSport.Runtime` must never reference `DroneSport.UI` (would create an asmdef cycle, since UI already depends on Runtime). UI-side networked types (e.g. `DroneSportRoomPlayer`) should not push data into UI controllers directly — UI controllers should poll for the data they need (e.g. via `NetworkClient.localPlayer`) instead of the networking layer knowing about UI.

### Code organization (`Assets/Scripts/`)
- `Drone/` — flight physics. `DroneFlightMath` is a static, pure-function class (stick shaping curves, thrust/torque math) with no Unity dependencies beyond `Vector3`/`Mathf` — this is what's unit tested. `DroneFlightController` (a `NetworkBehaviour`) is the thin integration layer that reads input and calls into the math each `FixedUpdate`.
- `Input/` — device abstraction. `IDroneInputSource.ReadChannels()` returns a `DroneInputChannels` struct; `PlayerDroneInput` is the concrete Input-System-backed implementation. This indirection exists specifically so `DroneFlightController.ApplyChannels(DroneInputChannels)` stays decoupled from *how* input arrives — swap devices without touching physics. `DroneControls.cs` is **generated** from `DroneControls.inputactions` by the Input System's code generator; don't hand-edit it, edit the `.inputactions` asset and regenerate. `AxisRemapProcessor` is a custom `InputProcessor<float>` for calibrating HID devices (e.g. RC transmitters) whose raw axis range doesn't match Unity's expectations.
- `Gameplay/` — match/scoring logic, deliberately split into pure C# classes (unit-testable, no `NetworkBehaviour`/`MonoBehaviour`) wrapped by thin `NetworkBehaviour` singletons that own `[SyncVar]`s and expose the server-authoritative API:
  - `MatchClock` / `MatchPhaseTracker` / `MatchResult` / `TeamScoreBoard` / `GateScoring` — pure logic, covered by EditMode tests.
  - `MatchManager` (`NetworkBehaviour` singleton, `Instance`) wraps `MatchClock` + `MatchPhaseTracker`, drives match phase transitions (`InProgress` → `Overtime` if gates are still open when time expires → `Ended`), determines the winner via `MatchResult`.
  - `ScoreManager` (`NetworkBehaviour` singleton, `Instance`) wraps `TeamScoreBoard`, exposes `AwardPoints`/`SetMultiplierControl`/`GetTeamColor`. Nullable `TeamId?` fields are synced as a raw `int` (`-1` sentinel for "none") since Mirror `[SyncVar]` doesn't support nullable enums directly — same idiom used by `LobbyCountdown`'s `-1f` "not counting down" sentinel.
  - `StandardGate` / `MultiplierGate` (`NetworkBehaviour`, server-authoritative trigger logic in `OnTriggerEnter`, gated by `isServer`) — visual feedback (`GateColorPainter`, via `MaterialPropertyBlock` to avoid material instancing) is pushed to clients via `[ClientRpc]`, never computed client-side.
  - `DroneTeam` — plain `MonoBehaviour` holding a drone's `TeamId`; set server-side only (`SetTeamServerSide`, guarded by `NetworkServer.active`).
- `Networking/` — `DroneSportNetworkManager` (`NetworkRoomManager` subclass — chosen over hand-rolling scene-transition/player-replacement plumbing) and `DroneSportRoomPlayer` (`NetworkRoomPlayer` subclass, `[SyncVar] TeamId Team` set via `[Command] CmdSetTeam`). `LobbyCountdown` is a scene-singleton `NetworkBehaviour` for the pre-match countdown, following the same `Instance` + server-ticked `[SyncVar]` pattern as `MatchManager`.
- `Debug/` — `DroneDebugHud`, `MatchHud`: simple runtime HUDs, local-player/owner gated.

### Networking model
Mirror-based, client-authoritative flight / server-authoritative scoring:
- The drone's `Rigidbody` is `isKinematic` for everyone except the owning client (`OnStartAuthority`/`OnStopAuthority` toggle this) — the owner simulates its own physics and position syncs out via `NetworkTransformReliable` on `Assets/Prefabs/Drone.prefab`.
- All scoring, gate state, match phase, and multiplier control are computed only on the server (`isServer` checks) and pushed to clients as `[SyncVar]`s or `[ClientRpc]` visual updates — clients never independently decide a gate opened/closed or a point was scored.
- Flow: `Menu.unity` (has the `NetworkManager` GameObject; offline scene) → `Lobby.unity` (`NetworkRoomManager`'s room scene; team select + ready-up) → countdown (`LobbyCountdown`) → `Game.unity` (`NetworkRoomManager`'s gameplay scene, team copied onto the spawned `Drone` in `OnRoomServerSceneLoadedForPlayer`).

### Testing philosophy
Only pure logic (math, scoring, phase transitions) gets automated EditMode NUnit tests under `Assets/Tests/EditMode/`. Physics feel, input device behavior, and full networking are verified manually in Play mode (or via the two-build-instances method above), since they need a live Editor and sometimes real hardware (e.g. an RC transmitter) — don't try to force these into automated tests.

## Third-party code

`Assets/Mirror/` is a vendored copy of the Mirror networking library, including its own examples, transports, and editor tooling — treat it as external/reference code, not part of the game's own architecture, unless a task specifically requires modifying it (which should be rare).
