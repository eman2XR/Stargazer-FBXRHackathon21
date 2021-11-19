using UnityEngine;

namespace Pixelplacement
{
    [ExecuteAlways]
    public class KeepInFront : MonoBehaviour
    {
        //Public Variables:
        public float distance = 2;
        public bool invertForward;
        
        //Private Variables:
        private Camera _mainCamera;
        private float speed = 4;
        
        //Startup:
        private void OnEnable()
        {
            //calls:
            Snap();
        }
        
        //Inspector:
        private void OnValidate()
        {
            Snap();
        }

        //Loops:
        private void LateUpdate()
        {
            //sets:
            Vector3 position = _mainCamera.transform.position + _mainCamera.transform.forward * distance;
            Vector3 forward = _mainCamera.transform.forward;
            if (invertForward)
            {
                forward *= -1;
            }
            Quaternion rotation = Quaternion.LookRotation(forward);
            
            //uses:
            transform.position = Vector3.Lerp(transform.position, position, Time.deltaTime * speed);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * speed);
        }
        
        //Private Variables:
        private void Snap()
        {
            if (!_mainCamera)
            {
                _mainCamera = Camera.main;
            }

            if (!_mainCamera)
            {
                return;
            }

            transform.position = _mainCamera.transform.position + _mainCamera.transform.forward * distance;
            Vector3 forward = _mainCamera.transform.forward;
            if (invertForward)
            {
                forward *= -1;
            }
            Quaternion rotation = Quaternion.LookRotation(forward);
            transform.rotation = rotation;
        }
    }
}