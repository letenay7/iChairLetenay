using UnityEngine;
using PupilLabs;

namespace Letenay
{
    public class SupportFunctions : MonoBehaviour
    {
        // script for bluetooth communication assigned in editor via BLE GameObject
        [SerializeField]
        private TestBle ble;

        // thresholds
        [SerializeField]
        private float eyelidApertureThreshold = 0.5f;
        [SerializeField]
        private float eyelidClosedTimeThreshold = 0.2f;

        private GazeDataProvider gazeDataProvider;
        private float blinkTimer;

        void Start()
        {
            gazeDataProvider = ServiceLocator.Instance.GazeDataProvider;
        }

        public void CheckEyelidAperture()
        {
            Eyelid eyelid = gazeDataProvider.RawEyelid;
            float aperture = Mathf.Max(eyelid.eyelidApertureLeft, eyelid.eyelidApertureRight);
            if (aperture < eyelidApertureThreshold)
            {
                blinkTimer += Time.deltaTime;
                if (blinkTimer > eyelidClosedTimeThreshold)
                {
                    StopWheelchair();
                }
            }
            else
            {
                blinkTimer = 0f;
            }
        }

        public void StopWheelchair()
        {
            ble.SetXY(128, 128);
        }

        public void MoveWheelchair(int x, int y)
        {
            ble.SetXY(x, y);
        }

        public bool EyelidAvailable()
        {
            return gazeDataProvider.EyelidAvailable;
        }
    }
}
