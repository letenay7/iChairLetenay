using MixedReality.Toolkit;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using MixedReality.Toolkit.UX;

namespace Letenay
{
    public class UtilityFunctions : MonoBehaviour
    {
        [SerializeField]
        private GameObject avatar;

        private AvatarController avatarController;
        private Slider speedSlider;
        private void Start()
        {
            avatarController = avatar.GetComponent<AvatarController>();
            speedSlider = GetComponentInChildren<Slider>(includeInactive: true);
            if (speedSlider == null)
            {
                Debug.LogWarning("No slider attached to " + gameObject.name);
            }
        }

        // the actual toggle state change happens when the select interaction ends (OnSelectExit)
        public void ToggleUtilityPanel(SelectExitEventArgs eventArgs)
        {
            Transform toggleButton = transform.GetChild(0);
            StatefulInteractable interactable = toggleButton.GetComponent<StatefulInteractable>();
            for (int i = 1; i < transform.childCount; i++)
            {
                GameObject child = transform.GetChild(i).gameObject;
                child.SetActive(interactable.IsToggled);
            }
        }

        public void SpeedUp(SelectEnterEventArgs eventArgs)
        {
            avatarController.IncreaseSpeed();
            UpdateSpeedSlider();
        }

        public void SlowDown(SelectEnterEventArgs eventArgs)
        {
            avatarController.DecreaseSpeed();
            UpdateSpeedSlider();
        }

        private void UpdateSpeedSlider()
        {
            speedSlider.Value = avatarController.ForwardSpeed;
        }
    }
}
