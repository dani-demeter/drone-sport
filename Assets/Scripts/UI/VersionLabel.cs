using TMPro;
using UnityEngine;

namespace DroneSport.UI
{
    public class VersionLabel : MonoBehaviour
    {
        [SerializeField] private TMP_Text versionText;
        [SerializeField] private string format = "v{0}";

        private void Awake()
        {
            versionText.text = string.Format(format, Application.version);
        }
    }
}
