using Mirror;

namespace DroneSport.Networking
{
    [System.Serializable]
    public struct MapOption
    {
        public string displayName;
        [Scene] public string sceneName;
    }
}
