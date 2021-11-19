using Pixelplacement.XRTools;
using UnityEngine;

namespace Pixelplacement.RoomMapperDemo
{
    public class PlacementDemo : RoomMapperDemoState
    {
        //Public Variables:
        public LineRenderer pointer;
        public Transform cursor;
        public GameObject floorContent;
        public GameObject wallContent;
        public GameObject ceilingContent;

        //Startup:
        protected override void Awake()
        {
            base.Awake();
            
            //activation:
            floorContent.SetActive(false);
            wallContent.SetActive(false);
            ceilingContent.SetActive(false);
        }

        //Loops:
        protected override void Update()
        {
            base.Update();
            
            //scan:
            RaycastHit hit;
            if (Physics.Raycast(_rig.rightControllerAnchor.position, _rig.rightControllerAnchor.forward, out hit))
            {
                //pointer:
                pointer.gameObject.SetActive(true);
                pointer.SetPosition(0, _rig.rightControllerAnchor.position);
                pointer.SetPosition(1, hit.point);
            
                //cursor:
                cursor.gameObject.SetActive(true);
                cursor.position = hit.point + hit.normal * .001f; //otherwise there will be z sorting with the surface
                cursor.rotation = Quaternion.LookRotation(hit.normal); //orient to surface

                //place content based on surface orientation:
                if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
                {
                    //surface:
                    float surfaceDot = Vector3.Dot(hit.normal, Vector3.up);
                    
                    //floor:
                    if (surfaceDot == -1)
                    {
                        //ceiling:
                        GameObject newCeilingContent = Instantiate(ceilingContent);
                        newCeilingContent.transform.position = hit.point;
                        newCeilingContent.transform.parent = RoomAnchor.Instance.transform;
                        newCeilingContent.SetActive(true);
                    } 
                    else if (surfaceDot == 0)
                    {
                        //wall:
                        GameObject newWallContent = Instantiate(wallContent);
                        newWallContent.transform.position = hit.point;
                        newWallContent.transform.forward = hit.normal;
                        newWallContent.transform.parent = RoomAnchor.Instance.transform;
                        newWallContent.SetActive(true);
                    }
                    else
                    {
                        //floor:
                        GameObject newFloorContent = Instantiate(floorContent);
                        newFloorContent.transform.position = hit.point;
                        newFloorContent.transform.forward = -Vector3.ProjectOnPlane(_rig.rightControllerAnchor.forward, Vector3.up).normalized; //face controller
                        newFloorContent.transform.parent = RoomAnchor.Instance.transform;
                        newFloorContent.SetActive(true);
                    }
                    
                    //ceiling:
                    if (surfaceDot == -1)
                    {
                        GameObject newCeilingContent = Instantiate(ceilingContent);
                        newCeilingContent.transform.position = hit.point;
                        newCeilingContent.transform.parent = RoomAnchor.Instance.transform;
                        newCeilingContent.SetActive(true);
                    }
                }
            }
            else
            {
                //disable:
                pointer.gameObject.SetActive(false);
                cursor.gameObject.SetActive(false);
            }
        }
    }
}