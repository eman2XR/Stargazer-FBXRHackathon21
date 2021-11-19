using System;
using Pixelplacement.XRTools;
using UnityEngine;

namespace Pixelplacement.RoomMapperDemo
{
    public class PhysicsDemo : RoomMapperDemoState
    {
        //Public Variables:
        public Rigidbody ball;
        public float launchForce = 300;

        //Private Variables:
        private Rigidbody _current;

        //Startup:
        protected override void Awake()
        {
            base.Awake();
            
            ball.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            MakeBall();
        }

        //Shutdown:
        private void OnDisable()
        {
            //clean up:
            Destroy(_current.gameObject);
            _current = null;
        }

        //Loops:
        protected override void Update()
        {
            base.Update();
            
            //follow controller:
            transform.SetPositionAndRotation(_rig.rightControllerAnchor.position, _rig.rightControllerAnchor.rotation);
        
            //fire:
            if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
            {
                ShootBall();
            }
            
            //prep:
            if (OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
            {
                MakeBall();
            }
        }

        //Private Methods:
        private void ShootBall()
        {
            //nothing to shoot?
            if (!_current)
            {
                return;
            }
            
            //send off:
            _current.transform.parent = RoomAnchor.Instance.transform;
            _current.isKinematic = false;
            _current.AddForce(transform.forward * launchForce);
            //Destroy(_current, 60); easy way to limit the mess
        }
    
        private void MakeBall()
        {
            //birth:
            _current = Instantiate(ball, transform.position, Quaternion.identity, transform);
            _current.gameObject.SetActive(true);
        }
    }
}