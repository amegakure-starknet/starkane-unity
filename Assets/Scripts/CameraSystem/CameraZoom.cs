using UnityEngine;
using Cinemachine;

namespace Amegakure.Starkane.CameraSystem
{
    public class CameraZoom : MonoBehaviour
    {
        [SerializeField] CinemachineVirtualCamera virtualCamera;
        [SerializeField] float zoomPercentaje = 10f;
        [SerializeField] float minDistance = 10;
        [SerializeField] float maxDistance = 50;

        private float cameraDistance;
  
        // Update is called once per frame
        void Update()
        {
            float mouseValue = Input.GetAxis("Mouse ScrollWheel");
        
            if(mouseValue != 0)
            {
                cameraDistance = mouseValue * zoomPercentaje;
                float actualZoomValue = virtualCamera.m_Lens.FieldOfView - cameraDistance;

                if(actualZoomValue >= minDistance && actualZoomValue <= maxDistance )
                {
                    virtualCamera.m_Lens.FieldOfView = actualZoomValue;
                }
            }
        }
    }
}
