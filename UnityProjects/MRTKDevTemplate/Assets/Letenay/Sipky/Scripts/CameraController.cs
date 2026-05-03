using UnityEngine;

namespace Letenay
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField]
        private GameObject avatar;

        private Vector3 offset;

        private void Start()
        {
            offset = transform.position - avatar.transform.position;
        }

        private void LateUpdate()
        {
            transform.position = avatar.transform.position + offset;
        }
    }
}
