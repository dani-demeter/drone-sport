using Mirror;

namespace DroneSport.Networking
{
    public class MapSelection : NetworkBehaviour
    {
        public static MapSelection Instance { get; private set; }

        [SyncVar] private int selectedMapIndex;

        public int SelectedMapIndex => selectedMapIndex;

        private void Awake()
        {
            Instance = this;
        }

        [Server]
        public void ServerSetSelectedMap(int mapIndex)
        {
            selectedMapIndex = mapIndex;
        }
    }
}
