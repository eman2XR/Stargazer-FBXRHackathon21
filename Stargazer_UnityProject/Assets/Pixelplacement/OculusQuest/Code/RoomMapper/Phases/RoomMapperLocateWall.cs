using UnityEngine;

namespace Pixelplacement.XRTools
{
    public class RoomMapperLocateWall : RoomMapperPhase
    {
        //Public Variables:
        public ChildActivator cursor;
        public LineRenderer connectionLine;
        
        //Private Variables:
        private float _lerpSpeed = 3.5f;
        private Vector3 _positionTarget;
        private float _maxCastDistance = 6f;
        private bool _onFloor;
        
        //Startup:
        protected override void Awake()
        {
            base.Awake();
            
            //sets:
            cursor.Activate(0);
        }

        private void OnEnable()
        {
            //sets:
            _onFloor = false;
            cursor.Activate(0);
        }

        //Loops:
        private void Update()
        {
            //parts:
            Plane floor = new Plane(Vector3.up, RoomAnchor.Instance.transform.position);
            Ray castRay = new Ray(ovrCameraRig.rightControllerAnchor.position, ovrCameraRig.rightControllerAnchor.forward);
            float castDistance;
            
            //cursor state:
            if (floor.Raycast(castRay, out castDistance))
            {
                if (!_onFloor)
                {
                    _onFloor = true;
                    cursor.Activate(1);
                }
            }
            else
            {
                if (_onFloor)
                {
                    _onFloor = false;
                    cursor.Activate(0);
                }
            }
            
            //clamp:
            if (castDistance <= 0 || castDistance > _maxCastDistance)
            {
                castDistance = _maxCastDistance;
            }
            
            //position (force to floor):
            Vector3 position = castRay.GetPoint(castDistance);
            if (_onFloor)
            {
                position.y = RoomAnchor.Instance.transform.position.y;
                //snap to floor:
                cursor.transform.position = new Vector3(cursor.transform.position.x, RoomAnchor.Instance.transform.position.y, cursor.transform.position.z);
            }
            transform.position = Vector3.Lerp(transform.position, position, Time.deltaTime * _lerpSpeed);
            
            //rotation:
            if (_onFloor)
            {
                Vector3 flatForward = Vector3.ProjectOnPlane(ovrCameraRig.rightControllerAnchor.forward, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(flatForward), Time.deltaTime * _lerpSpeed);
            }
            else
            {
                Quaternion rotation = Quaternion.LookRotation(ovrCameraRig.rightControllerAnchor.forward);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * _lerpSpeed);
            }
            
            //lines:
            if (_onFloor)
            {
                Vector3 controllerPosition = connectionLine.transform.InverseTransformPoint(ovrCameraRig.rightControllerAnchor.position);
                connectionLine.SetPosition(0, controllerPosition);
                connectionLine.SetPosition(3, controllerPosition);
            }
            
            //confirmation:
            if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
            {
                if (_onFloor)
                {
                    Next();
                }
            }
        }
    }
}