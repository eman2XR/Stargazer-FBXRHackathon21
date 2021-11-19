using System;

namespace Pixelplacement.RoomMapperDemo
{
    public abstract class RoomMapperDemoState : State
    {
        //Private Variables:
        protected OVRCameraRig _rig;
    
        //Startup:
        protected virtual void Awake()
        {
            _rig = FindObjectOfType<OVRCameraRig>();
        }

        //Loops:
        protected virtual void Update()
        {
            //next:
            if (OVRInput.GetDown(OVRInput.Button.PrimaryThumbstickRight, OVRInput.Controller.RTouch))
            {
                Next();
            }
            
            //previous:
            if (OVRInput.GetDown(OVRInput.Button.PrimaryThumbstickLeft, OVRInput.Controller.RTouch))
            {
                Previous();
            }
        }
    }
}