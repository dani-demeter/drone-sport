using Mirror;

namespace DroneSport.Gameplay
{
    public class DroneTeam : NetworkBehaviour
    {
        [SyncVar] private TeamId team;
        [SyncVar] private int teamSlotIndex;

        public TeamId Team => team;
        public int TeamSlotIndex => teamSlotIndex;

        public void SetTeamServerSide(TeamId newTeam, int slotIndex = 0)
        {
            if (!NetworkServer.active)
            {
                return;
            }

            team = newTeam;
            teamSlotIndex = slotIndex;
        }
    }
}
