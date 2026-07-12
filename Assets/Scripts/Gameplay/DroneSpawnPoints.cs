using UnityEngine;

namespace DroneSport.Gameplay
{
    public class DroneSpawnPoints : MonoBehaviour
    {
        public static DroneSpawnPoints Instance { get; private set; }

        [SerializeField] private Transform[] teamASpawnPoints;
        [SerializeField] private Transform[] teamBSpawnPoints;

        private int _teamANextIndex;
        private int _teamBNextIndex;

        private void Awake()
        {
            Instance = this;
        }

        public Transform GetNextSpawnPoint(TeamId team)
        {
            return team == TeamId.A
                ? NextFrom(teamASpawnPoints, ref _teamANextIndex)
                : NextFrom(teamBSpawnPoints, ref _teamBNextIndex);
        }

        private static Transform NextFrom(Transform[] points, ref int nextIndex)
        {
            if (points == null || points.Length == 0)
            {
                return null;
            }

            Transform point = points[nextIndex % points.Length];
            nextIndex++;
            return point;
        }
    }
}
