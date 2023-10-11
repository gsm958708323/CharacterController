using UnityEngine;

namespace FlexalonCopilot
{
    [ExecuteAlways]
    internal class FlexalonCopilotData : MonoBehaviour
    {
        public string GeneratedId;

        void Awake()
        {
            hideFlags = HideFlags.HideInInspector;
        }
    }
}