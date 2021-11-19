using UnityEngine;

namespace Pixelplacement.XRTools
{
    public class RoomMapperLocateCeiling : RoomMapperPhase
    {
        //Public Variables:
        public RoomMapperLocateWall locateWall;
        public LineRenderer connectionLine;
        public ChildActivator cursor;

        //Private Variables:
        private bool _aboveHead;
        private float _lerpSpeed = 3.5f;
        
        //Startup:
        protected override void Awake()
        {
            base.Awake();
            
            //refs:
            connectionLine = GetComponentInChildren<LineRenderer>(true);
        }

        private void OnEnable()
        {
            //sets:
            _aboveHead = false;
            transform.SetPositionAndRotation(locateWall.transform.position, locateWall.transform.rotation);
            cursor.Activate(0);
        }

        //Loops:
        private void Update()
        {
            //parts:
            Plane wall = new Plane(-locateWall.transform.forward, locateWall.transform.position);
            Ray castRay = new Ray(ovrCameraRig.rightControllerAnchor.position, ovrCameraRig.rightControllerAnchor.forward);
            float castDistance;
            
            //cast:
            wall.Raycast(castRay, out castDistance);

            //current state:
            if (transform.position.y > ovrCameraRig.centerEyeAnchor.position.y)
            {
                if (!_aboveHead)
                {
                    _aboveHead = true;
                    cursor.Activate(1);
                }
            }
            else
            {
                if (_aboveHead)
                {
                    _aboveHead = false;
                    cursor.Activate(0);
                }
            }
            
            //position:
            Vector3 location = locateWall.transform.position;
            location.y = castRay.GetPoint(castDistance).y;
            transform.position = Vector3.Lerp(transform.position, location, Time.deltaTime * _lerpSpeed);

            //lines:
            if (_aboveHead)
            {
                Vector3 controllerPosition = connectionLine.transform.InverseTransformPoint(ovrCameraRig.rightControllerAnchor.position);
                connectionLine.SetPosition(0, controllerPosition);
                connectionLine.SetPosition(3, controllerPosition);
            }
            
            //confirmation:
            if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
            {
                if (_aboveHead)
                {
                    Next();
                }
            }
        }
    }
}