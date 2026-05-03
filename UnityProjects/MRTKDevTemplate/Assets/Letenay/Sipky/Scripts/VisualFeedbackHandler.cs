using UnityEngine;
using UnityEngine.UI;

namespace Letenay
{
    public class VisualFeedbackHandler : MonoBehaviour
    {
        private Transform backplate;
        private Material defaultMaterial;
        [SerializeField]
        private Material hoverMaterial;
        [SerializeField]
        private Material selectMaterial;

        private void Start()
        {
            backplate = transform.Find("Backplate");
            defaultMaterial = backplate.GetComponent<RawImage>().material;
        }

        public void OnGazeEntered()
        {
            backplate.GetComponent<RawImage>().material = hoverMaterial;
        }

        public void OnGazeExited()
        {
            backplate.GetComponent<RawImage>().material = defaultMaterial;
        }

        public void OnSelectEntered()
        {
            backplate.GetComponent<RawImage>().material = selectMaterial;
        }
    }
}
