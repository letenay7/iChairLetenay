using UnityEngine;
using TMPro;

namespace Letenay
{
    public class AvatarController : MonoBehaviour
    {
        private Rigidbody rigidBody;

        public float ForwardSpeed {get; set;} = 1f;

        [SerializeField]
        private float reverseSpeedMultiplier = 0.5f;

        [SerializeField]
        private float rotationSpeedMultiplier = 0.8f;

        [SerializeField]
        private TextMeshProUGUI currentlySelected;

        private Vector3 currentMovementInput = Vector3.zero;
        private float currentRotationInput = 0f;

        private void Start()
        {
            rigidBody = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            ApplyMovement();
            ApplyRotation();
        }

        private void ApplyMovement()
        {
            if (currentMovementInput != Vector3.zero)
            {
                float speed = ForwardSpeed;
                if (currentMovementInput == Vector3.back)
                {
                    speed *= reverseSpeedMultiplier;
                }
                Vector3 moveVector = currentMovementInput * speed;
                // Apply velocity, keeping existing Y velocity (e.g., gravity)
                rigidBody.velocity = new Vector3(moveVector.x, rigidBody.velocity.y, moveVector.z);
            }
        }

        private void ApplyRotation()
        {
            if (currentRotationInput != 0f)
            {
                float rotationAmount = currentRotationInput * rotationSpeedMultiplier;
                transform.Rotate(Vector3.up, rotationAmount);
            }
        }

        public void StopMovement()
        {
            rigidBody.velocity = Vector3.zero;
            currentMovementInput = Vector3.zero;
            SetRotationInput(0f);
            rigidBody.angularVelocity = Vector3.zero;
            currentlySelected.text = "Currently selected: Stop";
        }

        public void SetMovementInput(Vector3 direction)
        {
            currentMovementInput = direction;
            if (currentMovementInput == Vector3.zero)
            {
                StopMovement();
            }
        }

        public void SetRotationInput(float direction)
        {
            /* -1 for left
          1 for right,
          0 for stop
        */
            currentRotationInput = direction;
        }

        public void IncreaseSpeed()
        {
            if (ForwardSpeed < 5)
            {
                ForwardSpeed++;
                Debug.Log($"Speed increased. Current speed: {ForwardSpeed}");
            }
        }

        public void DecreaseSpeed()
        {
            if (ForwardSpeed > 1)
            {
                ForwardSpeed--;
                Debug.Log($"Speed decreased. Current speed: {ForwardSpeed}");
            }
        }


    }
}
