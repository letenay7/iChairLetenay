using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Letenay
{
    public class ContinuousMovementHandler : MonoBehaviour
    {
        [SerializeField]
        private GameObject avatar;

        private AvatarController avatarController;

        [SerializeField]
        private TextMeshProUGUI currentlySelected;

        [SerializeField]
        private Direction currentDirection;

        private bool isSelected;
        private bool isHovering;

        private void Start()
        {
            avatarController = avatar.GetComponent<AvatarController>();
        }

        public void OnGazeEntered()
        {
            // stop current Action
            StopAction();
            // start visual feedback for hover
            GetComponent<VisualFeedbackHandler>().OnGazeEntered();
            isHovering = true;
        }

        public void OnGazeExited() // if user exits hover before 2s dwell completes
        {
            // reset if hover started but action was not triggered
            if (isHovering && !isSelected)
            {
                GetComponent<VisualFeedbackHandler>().OnGazeExited();
            }
            isHovering = false;
        }

        public void ResetToDefault()
        {
            foreach (Transform child in transform.parent)
            {
                if (child.TryGetComponent(out VisualFeedbackHandler handler))
                {
                    if (!child.name.Equals(gameObject.name))
                    {
                        handler.OnGazeExited();
                    }
                }
                else
                {
                    Debug.LogWarning("No visual feedback found for " + child.name);
                }
            }
        }
        // Called when the 2-second dwell is complete (XRI Select Entered)
        public void OnSelectEntered(SelectEnterEventArgs eventArgs)
        {
            isSelected = true;
            StartAction();
        }

        private void StartAction()
        {
            if (avatarController == null)
            {
                Debug.Log("Avatar Controller is null");
                return;
            }
            switch (currentDirection)
            {
                case Direction.Forward:
                    avatarController.SetMovementInput(Vector3.forward);
                    break;
                case Direction.Reverse:
                    avatarController.SetMovementInput(Vector3.back);
                    break;
                case Direction.Left:
                    avatarController.SetRotationInput(-1f);
                    break;
                case Direction.Right:
                    avatarController.SetRotationInput(1f);
                    break;
                default:
                    StopAction();
                    break;
            }

            currentlySelected.text = $"Currently selected: {currentDirection}";
        }

        private void StopAction()
        {
            if (avatarController == null)
            {
                Debug.Log("Avatar Controller is null");
                return;
            }

            isSelected = false;
            isHovering = false;
            ResetToDefault();

            switch (currentDirection)
            {
                case Direction.Forward:
                case Direction.Reverse:
                    avatarController.SetMovementInput(Vector3.zero);
                    break;
                case Direction.Left:
                case Direction.Right:
                    avatarController.SetRotationInput(0f);
                    break;
                default:
                    avatarController.StopMovement();
                    break;
            }
        }
    }
}
