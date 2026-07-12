using Mirror;

namespace DroneSport.Gameplay
{
    public class DroneTeam : NetworkBehaviour
    {
        [SyncVar] private TeamId team;

        public TeamId Team => team;

        public void SetTeamServerSide(TeamId newTeam)
        {
            if (!NetworkServer.active)
            {
                return;
            }

            team = newTeam;
        }
    }
}
