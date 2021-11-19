using UnityEngine;

namespace Pixelplacement.XRTools
{
    public abstract class RoomMapperPhase : MonoBehaviour
    {
        //Protected Variables:
        protected OVRCameraRig ovrCameraRig;
    
        //Private Variables:
        private ChildActivator _parentChildActivator;
    
        //Startup:
        protected virtual void Awake()
        {
            //refs:
            _parentChildActivator = transform.parent.GetComponent<ChildActivator>();
            ovrCameraRig = FindObjectOfType<OVRCameraRig>();
        }
     
        //Protected Methods:
        protected GameObject Next(bool disableAfterLast = false)
        {
            return _parentChildActivator.Next(disableAfterLast);
        }

        protected GameObject GoTo(string childName)
        {
            return _parentChildActivator.Activate(childName);
        }
    }
}