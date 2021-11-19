using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pixelplacement.XRTools
{
    /// <summary>
    /// Creates a reliable room center and "north" for use with the Oculus Integration's playspace tooling.
    /// Intended usage is to just grab a reference to the transform of the anchor through its singleton instance.
    /// Will properly recover if a user reorients.
    /// Needs the OVRManager Tracking Origin to be FloorLevel
    /// </summary>
    public class RoomAnchor : MonoBehaviour
    {
        //Public Properties:
        public static RoomAnchor Instance
        {
            get
            {
                if (!_instance)
                {
                    _instance = FindObjectOfType<RoomAnchor>();
                }

                if (!_instance)
                {
                    _instance = new GameObject("(RoomAnchor)").AddComponent<RoomAnchor>();
                }

                return _instance;
            }
        }

        public Vector3[] Points
        {
            get;
            private set;
        }

        //Inspector:
        private void Reset()
        {
            transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
        }

        //Private Variables:
        private static RoomAnchor _instance;
        private bool _updated;
        private List<Action> _callbacks = new List<Action>();

        //Startup:
        private void Awake()
        {
            //hooks:
            Debug.Log(OVRManager.display);
            OVRManager.display.RecenteredPose += HandleDisplayOnRecenteredPose;

            //runs:
            StartCoroutine(SetupPlayArea());
        }

        private void Start()
        {
            if (OVRManager.instance.trackingOriginType != OVRManager.TrackingOrigin.FloorLevel)
            {
                Debug.LogError("RoomAnchor requires OVRManager's Tracking Origin Type to be set as FloorLevel for proper operation.");
            }
        }

        //Shutdown:
        private void OnDestroy()
        {
            //hooks:
            OVRManager.display.RecenteredPose -= HandleDisplayOnRecenteredPose;
            
            //sets:
            _instance = null;
            _callbacks.Clear();
        }

        //Event Handlers:
        private void HandleDisplayOnRecenteredPose()
        {
            StartCoroutine(SetupPlayArea());
        }
        
        //Public Methods:
        public void RegisterForUpdates(Action callback)
        {
            if (_updated)
            {
                callback();
            }
            else
            {
                _callbacks.Add(callback);
            }
        }

        //Coroutines:
        private IEnumerator SetupPlayArea()
        {
            //Oculus Link doesn't allow boundary query:
            if (Application.isEditor)
            {
                transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
                //dummy room anchors:
                Points = new Vector3[] { new Vector3(-2,0,2), new Vector3(2,0,2), new Vector3(2,0,-2), new Vector3(-2, 0, -2)};
            }
            else
            {
                //wait for boundary configuration:
                while (!OVRManager.boundary.GetConfigured())
                {
                    yield return null;
                }

                //find boundary:
                Points = OVRManager.boundary.GetGeometry(OVRBoundary.BoundaryType.PlayArea);
                
                //set anchor:
                transform.position = Vector3.Lerp(Points[0], Points[2], .5f);
                Vector3 roomForward = Vector3.Normalize(Points[1] - Points[0]);
                transform.rotation = Quaternion.LookRotation(roomForward);
                
                //callbacks:
                foreach (var callback in _callbacks)
                {
                    callback?.Invoke();
                }
                
                _callbacks.Clear();
            }

            _updated = true;
        }
        
        //Public Methods:
        public void Create()
        {
            //dummy call to get the singleton fired
        }
    }
}