using UnityEngine;
using MixedReality.Toolkit.UX;

namespace Letenay
{
    public class Settings : MonoBehaviour
    {
        [SerializeField]
        private PressableButton autoModeToggle;

        [SerializeField]
        private PressableButton dwellModeToggle;

        public bool IsAutoModeToggled()
        {
            return autoModeToggle.IsToggled;
        }

        public bool IsDwellModeToggled()
        {
            return dwellModeToggle.IsToggled;
        }
    }
}
