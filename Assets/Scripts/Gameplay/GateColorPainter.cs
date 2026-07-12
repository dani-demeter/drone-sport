using UnityEngine;

namespace DroneSport.Gameplay
{
    internal static class GateColorPainter
    {
        private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
        private static readonly int ColorId = Shader.PropertyToID("_Color");

        private static MaterialPropertyBlock _block;

        public static void Paint(Renderer[] renderers, Color color)
        {
            if (renderers == null)
            {
                return;
            }

            _block ??= new MaterialPropertyBlock();

            foreach (Renderer renderer in renderers)
            {
                if (renderer == null)
                {
                    continue;
                }

                renderer.GetPropertyBlock(_block);
                _block.SetColor(BaseColorId, color);
                _block.SetColor(ColorId, color);
                renderer.SetPropertyBlock(_block);
            }
        }
    }
}
