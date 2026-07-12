using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;

namespace DroneSport.Input
{
#if UNITY_EDITOR
    [UnityEditor.InitializeOnLoad]
#endif
    public class AxisRemapProcessor : InputProcessor<float>
    {
        public float fromMin = -1f;
        public float fromMax = 1f;
        public float toMin = -1f;
        public float toMax = 1f;

        static AxisRemapProcessor()
        {
            Register();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Register()
        {
            InputSystem.RegisterProcessor<AxisRemapProcessor>();
        }

        public override float Process(float value, InputControl control)
        {
            float t = Mathf.InverseLerp(fromMin, fromMax, value);
            return Mathf.Lerp(toMin, toMax, t);
        }
    }
}
