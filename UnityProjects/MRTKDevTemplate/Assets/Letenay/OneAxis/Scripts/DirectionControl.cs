using MixedReality.Toolkit.UX;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Letenay
{
    public class DirectionControl : MonoBehaviour
    {
        [SerializeField]
        private GameObject rotationControl;
        [SerializeField]
        private GameObject settingsPanel;

        [SerializeField]
        private Slider speedSlider;
        [SerializeField]
        private Slider utilitySpeedSlider;

        private const int MinSpeed = 1;
        private const int MaxSpeed = 5;
        private int currentSpeed = MinSpeed;

        private RotationControl rotationControlScript;

        void Start()
        {
            rotationControlScript = rotationControl.GetComponent<RotationControl>();
        }

        private void UpdateSpeedSliders()
        {
            rotationControlScript.SpeedIndex = currentSpeed;
            speedSlider.Value = currentSpeed;
            utilitySpeedSlider.Value = currentSpeed;
        }

        public void SpeedUp(SelectExitEventArgs exitEventArgs)
        {
            if (IsManualModeToggled() || currentSpeed == MaxSpeed)
            {
                return;
            }

            currentSpeed++;
            UpdateSpeedSliders();
        }

        public void SpeedDown(SelectExitEventArgs exitEventArgs)
        {
            if (IsManualModeToggled() || currentSpeed == MinSpeed)
            {
                return;
            }

            currentSpeed--;
            UpdateSpeedSliders();
        }

        public void MoveForward(SelectExitEventArgs exitEventArgs)
        {
            rotationControlScript.CurrentMovementDirection = MovementDirection.Forward;
            StartMovement();
        }

        public void MoveReverse(SelectExitEventArgs exitEventArgs)
        {
            rotationControlScript.CurrentMovementDirection = MovementDirection.Reverse;
            StartMovement();
        }

        public void StartMovement()
        {
            rotationControlScript.SpeedIndex = currentSpeed;
            gameObject.SetActive(false);
            rotationControl.SetActive(true);
        }

        public void OpenSettingsPanel(SelectExitEventArgs exitEventArgs)
        {
            gameObject.SetActive(false);
            settingsPanel.SetActive(true);
        }

        public void CloseSettingsPanel(SelectExitEventArgs exitEventArgs)
        {
            settingsPanel.SetActive(false);
            gameObject.SetActive(true);
        }

        public void SetSpeedToMinimum()
        {
            currentSpeed = MinSpeed;
            UpdateSpeedSliders();
        }

        private ControlMode GetCurrentControlMode()
        {
            return rotationControlScript.CurrentControlMode;
        }

        private bool IsManualModeToggled()
        {
            Settings settings = settingsPanel.GetComponent<Settings>();
            return !settings.IsAutoModeToggled() || GetCurrentControlMode() == ControlMode.Manual;
        }
    }
}
