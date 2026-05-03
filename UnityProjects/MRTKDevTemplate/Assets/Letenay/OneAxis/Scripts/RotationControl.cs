using UnityEngine;
using MixedReality.Toolkit.UX;
using MixedReality.Toolkit;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

namespace Letenay
{
    public class RotationControl : MonoBehaviour
    {
        // objects in scene
        [SerializeField]
        private GameObject directionControl;
        [SerializeField]
        private GameObject directionPlate;
        [SerializeField]
        private Settings settings;

        [SerializeField]
        private AnimationCurve rotationCurve;

        [SerializeField]
        private int directionMovementStep = 64;

        // input values for wheelchair
        private float currentRotation = 128f;
        private float currentDirection = 128f;

        // script for managing eye tracking and wheelchair movement
        private SupportFunctions supportFunctions;

        // timers
        private float buttonGazeTime;

        // GIU
        [SerializeField]
        private Transform mainPlate;
        private Slider rotationSlider;

        // buttons
        [SerializeField]
        private StatefulInteractable leftButtonInteractable;
        [SerializeField]
        private StatefulInteractable rightButtonInteractable;
        [SerializeField]
        private StatefulInteractable forwardButtonInteractable;
        [SerializeField]
        private StatefulInteractable reverseButtonInteractable;
        private List<StatefulInteractable> directionButtons = new List<StatefulInteractable>();

        private Vector3 initialMainPlatePosition;

        public ControlMode CurrentControlMode {get; private set;}
        public MovementDirection CurrentMovementDirection {get; set;}

        private int[] rotationValues = {0, 32, 64, 96, 128, 160, 192, 224, 255};
        private int rotationValueIndex = 4; // default value = 128, at index 4

        private float[] speedMultipliers = {0.5f, 0.75f, 1f, 1.25f, 1.5f};

        public int SpeedIndex {get; set;} // used to retrieve the correct multiplier based on speed setting

        void Awake()
        {
            supportFunctions = GetComponent<SupportFunctions>();
            rotationSlider = mainPlate.GetComponentInChildren<Slider>();
            directionButtons.AddRange(new []{leftButtonInteractable, rightButtonInteractable,
                forwardButtonInteractable, reverseButtonInteractable});
            initialMainPlatePosition = mainPlate.localPosition;
        }

        void OnEnable()
        {
            CheckStartMode();
        }

        void Update()
        {
            if (supportFunctions.EyelidAvailable())
            {
                supportFunctions.CheckEyelidAperture();
            }

            int x = 128;
            int y = 128;
            switch (CurrentControlMode)
            {
                case ControlMode.Auto:
                    CheckRotation();
                    float direction = (CurrentMovementDirection == MovementDirection.Forward)
                        ? 128 + directionMovementStep
                        : 128 - directionMovementStep;
                     x = Mathf.RoundToInt(ApplySpeedMultiplier(currentRotation));
                     y = Mathf.RoundToInt(ApplySpeedMultiplier(direction));
                    //Debug.LogWarning($"x = {x} y = {y}");
                    supportFunctions.MoveWheelchair(x, y);
                    break;

                case ControlMode.Manual:
                    CheckDirection();
                    CheckRotation();
                    x = Mathf.RoundToInt(ApplySpeedMultiplier(currentRotation));
                    y = Mathf.RoundToInt(ApplySpeedMultiplier(currentDirection));
                    //Debug.LogWarning($"x = {x} y = {y}");
                    supportFunctions.MoveWheelchair(x, y);
                    break;

                case ControlMode.Dwell:
                default:
                    break;
            }
        }

        private float ApplySpeedMultiplier(float inputValue)
        {
            /*
             * wheelchair speeds start from 1, but we index from zero
             * if going forward or right, value has to be greater than 0 but less than 128
             * if going reverse or left, value has to be grater than 128 but less than 256
             */

            float selectedMultiplier = speedMultipliers[SpeedIndex - 1];
            float offsetFromCenter = inputValue - 128f;
            float multiplied = 128f + (offsetFromCenter * selectedMultiplier);
            return Mathf.Clamp(multiplied, 0f, 255f);
        }

        private void CheckRotation()
        {
            if (leftButtonInteractable.IsGazeHovered)
            {
                buttonGazeTime += Time.deltaTime;
                Rotate(MovementDirection.Left);
            }
            else if (rightButtonInteractable.IsGazeHovered)
            {
                buttonGazeTime += Time.deltaTime;
                Rotate(MovementDirection.Right);
            }
            else
            {
                // Reset timer when looking away
                buttonGazeTime = 0f;
                //RecenterRotation();
            }
        }

        private void CheckDirection()
        {
            if (forwardButtonInteractable.IsGazeHovered)
            {
                currentDirection = 128 + directionMovementStep;
                RecenterRotation();
            }
            else if (reverseButtonInteractable.IsGazeHovered)
            {
                currentDirection = 128 - directionMovementStep;
                RecenterRotation();
            }
            else
            {
                currentDirection = 128f;
            }
            currentDirection = Mathf.Clamp(currentDirection, 0f, 255f);
        }

        private void RecenterRotation()
        {
            currentRotation = 128f;
            UpdateRotationSlider();
        }

        private void Move(MovementDirection direction)
        {
            float y = direction switch
            {
                MovementDirection.Forward => ApplySpeedMultiplier(128f + directionMovementStep),
                MovementDirection.Reverse => ApplySpeedMultiplier(128f - directionMovementStep),
                _ => 128f
            };
            //Debug.LogWarning($"y = {Mathf.RoundToInt(y)}");

            supportFunctions.MoveWheelchair(128, Mathf.RoundToInt(y));
            RecenterRotation();
        }

        public void Rotate(MovementDirection direction)
        {
            if (CurrentControlMode == ControlMode.Dwell)
            {
                CalculateDwellModeRotation(direction);
                int x = Mathf.RoundToInt(ApplySpeedMultiplier(currentRotation));
                //Debug.LogWarning($"x = {x}");
                supportFunctions.MoveWheelchair(Mathf.RoundToInt(x), 128);
            }
            else
            {
                float speed = rotationCurve.Evaluate(buttonGazeTime);
                float change = speed * Time.deltaTime;
                if (direction == MovementDirection.Left)
                {
                    change = -change;
                }
                //Debug.LogWarning($"change: {change}");
                currentRotation += change;
            }
            UpdateRotationSlider();
        }

        private void CalculateDwellModeRotation(MovementDirection direction)
        {
            if (direction == MovementDirection.Left)
            {
                if (rotationValueIndex > 0)
                {
                    rotationValueIndex--;
                }
            }
            else if (direction == MovementDirection.Right)
            {
                if (rotationValueIndex < rotationValues.Length - 1)
                {
                    rotationValueIndex++;
                }
            }
            currentRotation = rotationValues[rotationValueIndex];
        }

        private void UpdateRotationSlider()
        {
            currentRotation = Mathf.Clamp(currentRotation, 0, 255);
            rotationSlider.Value = currentRotation;
        }

        public void Stop(SelectExitEventArgs exitEventArgs)
        {
            StopMoving();
            gameObject.SetActive(false);
            directionControl.SetActive(true);
        }

        public void EnableAutoMode()
        {
            mainPlate.localPosition = initialMainPlatePosition;
            RemoveButtonListeners();
            CurrentControlMode = ControlMode.Auto;
            SetupHoverListeners();
            directionPlate.SetActive(false);
        }

        public void EnableManualMode()
        {
            MoveMainPlate();
            StopMoving();
            RemoveButtonListeners();
            CurrentControlMode = ControlMode.Manual;

            /* in manual mode we set the speed to minimum value to allow fine-tuning of movement
             and increase safety
             */
            SetSpeedToMinimum();

            SetupHoverListeners();
            directionPlate.SetActive(true);
        }

        private void MoveMainPlate()
        {
            string activeSceneName =  SceneManager.GetActiveScene().name;
            if (!activeSceneName.Equals("OneAxisControl"))
            {
                return;
            }
            if (mainPlate.localPosition.Equals(initialMainPlatePosition))
            {
                // move main plate slightly to accomodate for direction plate
                mainPlate.localPosition = new Vector3(mainPlate.localPosition.x - 25f, mainPlate.localPosition.y,
                    mainPlate.localPosition.z);
            }
        }

        public void SyncManualToggle()
        {
            // sync settings and utility plate toggles in OneAxis scene
            Transform manualControl = transform.Find("UtilityPlate/ManualControl");
            StatefulInteractable toggle = manualControl.GetComponent<StatefulInteractable>();
            if (!toggle.IsToggled)
            {
                toggle.ForceSetToggled(true, fireEvents: false);
            }
        }

        private void SetupHoverListeners()
        {
            foreach (StatefulInteractable button in directionButtons)
            {
                // in Auto/Manual mode just assign visual feedback, logic is handled in Update
                // since we don't use dwell time we go directly to selected state

                button.IsGazeHovered
                    .OnEntered
                    .AddListener(_ => button.GetComponentInChildren<VisualFeedbackHandler>().OnSelectEntered());

                button.IsGazeHovered
                    .OnExited
                    .AddListener(_ => button.GetComponentInChildren<VisualFeedbackHandler>().OnGazeExited());
            }
            rotationSlider.SliderStepDivisions = 255;
        }

        public void EnableDwellMode()
        {
            StopMoving();
            RemoveButtonListeners();
            CurrentControlMode = ControlMode.Dwell;
            SetupDwellListeners();
            directionPlate.SetActive(true);
        }

        public void SyncDwellToggle()
        {
            SyncManualToggle();
            Transform dwellToggle = transform.Find("DirectionPlate/DwellToggle");
            StatefulInteractable toggle = dwellToggle.GetComponent<StatefulInteractable>();
            if (!toggle.IsToggled)
            {
                toggle.ForceSetToggled(true, fireEvents: false);
            }
        }

        private void SetupDwellListeners()
        {
            foreach (StatefulInteractable button in directionButtons)
            {
                button.UseGazeDwell = true;
                button.GazeDwellTime = 1f;

                button.IsGazeHovered
                    .OnEntered
                    .AddListener(_ => button.GetComponentInChildren<VisualFeedbackHandler>()
                        .OnGazeEntered());

                button.IsGazeHovered
                    .OnExited
                    .AddListener(_ => button.GetComponentInChildren<VisualFeedbackHandler>()
                        .OnGazeExited());

                button.selectEntered
                    .AddListener(_ => button.GetComponentInChildren<VisualFeedbackHandler>()
                        .OnSelectEntered());
            }

            forwardButtonInteractable.selectEntered.AddListener(_ => Move(MovementDirection.Forward));
            reverseButtonInteractable.selectEntered.AddListener(_ => Move(MovementDirection.Reverse));
            leftButtonInteractable.selectEntered.AddListener(_ => Rotate(MovementDirection.Left));
            rightButtonInteractable.selectEntered.AddListener(_ => Rotate(MovementDirection.Right));
            rotationSlider.SliderStepDivisions = rotationValues.Length;
        }

        private void RemoveButtonListeners()
        {
            foreach (StatefulInteractable button in directionButtons)
            {
                button.IsGazeHovered.OnEntered.RemoveAllListeners();
                button.IsGazeHovered.OnExited.RemoveAllListeners();
                button.selectEntered.RemoveAllListeners();
                button.selectExited.RemoveAllListeners();
                button.UseGazeDwell = false;
            }
        }

        public void CheckStartMode()
        {
            if (settings.IsAutoModeToggled())
            {
                EnableAutoMode();
            }
            else
            {
                EnableManualMode();
            }

            if (settings.IsDwellModeToggled())
            {
                EnableDwellMode();
            }
        }

        private void SetSpeedToMinimum()
        {
            DirectionControl directionControlScript = directionControl.GetComponent<DirectionControl>();
            directionControlScript.SetSpeedToMinimum();
        }

        private void StopMoving()
        {
            currentRotation = 128f;
            currentDirection = 128f;
            //Debug.LogWarning("Stopping movement");
            supportFunctions.StopWheelchair();
            UpdateRotationSlider();
        }
    }
}
