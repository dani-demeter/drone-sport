using UnityEngine;

namespace DroneSport.Gameplay
{
    [CreateAssetMenu(fileName = "TeamTailColorPalette", menuName = "Drone Sport/Team Tail Color Palette")]
    public class TeamTailColorPalette : ScriptableObject
    {
        [SerializeField]
        private Color[] teamAColors =
        {
            new(0.95f, 0.5f, 0.1f), // orange
            new(0.95f, 0.85f, 0.1f), // yellow
            new(0.9f, 0.15f, 0.15f) // red
        };

        [SerializeField]
        private Color[] teamBColors =
        {
            new(0.1f, 0.4f, 0.95f), // blue
            new(0.15f, 0.85f, 0.2f), // green
            new(0.6f, 0.2f, 0.85f) // purple
        };

        public Color GetColor(TeamId team, int slotIndex)
        {
            Color[] colors = team == TeamId.A ? teamAColors : teamBColors;
            if (colors == null || colors.Length == 0)
            {
                return Color.white;
            }

            int index = Mathf.Clamp(slotIndex, 0, colors.Length - 1);
            return colors[index];
        }
    }
}
