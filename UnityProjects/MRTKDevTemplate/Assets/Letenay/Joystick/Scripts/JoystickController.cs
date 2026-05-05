using UnityEngine;
using MixedReality.Toolkit;
using Microsoft.MixedReality.GraphicsTools;

namespace Letenay
{

    public class JoystickController : MonoBehaviour
    {
        [SerializeField]
        private int cruiseModeMovementValue = 192;
        [SerializeField]
        private float colliderLimit = 150f;

        // assign in editor if missing
        [SerializeField]
        private GameObject gazeIndicator;
        [SerializeField]
        private Material hoverMaterial;
        [SerializeField]
        private GameObject cruiseToggle;

        // script for managing eye tracking and wheelchair movement
        private SupportFunctions supportFunctions;

        private Material defaultIndicatorMaterial;
        private CanvasElementRoundedRect gazeIndicatorRect;
        private Vector3 defaultIndicatorPosition = Vector3.zero;
        private BoxCollider indicatorCollider;
        private bool isJoystickEnabled;
        private bool cruiseMode;

        private void Start()
        {
            supportFunctions = GetComponent<SupportFunctions>();
            defaultIndicatorPosition = gazeIndicator.transform.localPosition;
            gazeIndicatorRect = gazeIndicator.GetComponent<CanvasElementRoundedRect>();
            defaultIndicatorMaterial = gazeIndicatorRect.material;
            indicatorCollider = gazeIndicator.GetComponent<BoxCollider>();
        }

        private void Update()
        {
            if (isJoystickEnabled)
            {
                if (supportFunctions.EyelidAvailable())
                {
                    supportFunctions.CheckEyelidAperture();
                }
                int joystickX = Mathf.RoundToInt(gazeIndicator.transform.localPosition.x) + 128;
                int joystickY = Mathf.RoundToInt(gazeIndicator.transform.localPosition.y) + 128;
                //Debug.LogWarning($"x: {joystickX} y: {joystickY}");
                joystickX = Mathf.Clamp(joystickX, 0, 255);
                joystickY = Mathf.Clamp(joystickY, 0, 255);
                supportFunctions.MoveWheelchair(joystickX, joystickY);
            }
            else if (cruiseMode)
            {
                // in cruise mode and we want to move constantly forward
                supportFunctions.MoveWheelchair(128, cruiseModeMovementValue);
            }
            else
            {
               StopMoving();
            }
        }

        public void OnIndicatorGazeEntered()
        {
            gazeIndicatorRect.material = hoverMaterial;
        }

        public void OnIndicatorGazeExited()
        {
            gazeIndicatorRect.material = defaultIndicatorMaterial;
        }

        public void EnableMovement()
        {
            gazeIndicatorRect.material = defaultIndicatorMaterial;
            isJoystickEnabled = true;
            // disable collider because of raycasts
            indicatorCollider.enabled = false;
        }

        public void DisableMovement()
        {
            StopMoving();
            isJoystickEnabled = false;
            gazeIndicator.transform.localPosition = defaultIndicatorPosition;
            indicatorCollider.enabled = true;
        }

        private void StopMoving()
        {
            supportFunctions.StopWheelchair();
        }

        public void OnRaycastHit(Transform hitTransform, Vector3 hitPoint, Vector2 hitTextureCoord)
        {
            if (!isJoystickEnabled)
            {
                return;
            }

            Vector3 localPosition = transform.InverseTransformPoint(hitPoint);
            Vector3 targetPosition = localPosition;
            targetPosition.z = defaultIndicatorPosition.z;
            gazeIndicator.transform.localPosition = targetPosition;
            //Debug.LogWarning($"Target:  {targetPosition}");

            /*  Values accepted by the wheelchair are <0, 255>
               Our joystick pointer is at local coords (0, 0, 0) and the joystick has size 260x260
               considering the size of the gaze pointer and tolerance local values on the joystick are <-128, 128>
            */
            if (Vector3.Distance(targetPosition, defaultIndicatorPosition) > colliderLimit)
            {
                DisableMovement();
            }
        }

        public void ToggleCruiseMode()
        {
            StatefulInteractable toggleInteractable = cruiseToggle.GetComponent<StatefulInteractable>();
            cruiseMode = toggleInteractable.IsToggled; // match cruise mode bool to toggle state
            DisableMovement();
            gazeIndicator.SetActive(!cruiseMode);
        }
    }
}
