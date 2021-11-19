using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Pixelplacement.XRTools
{
    public class RoomMapperLocateCorners : RoomMapperPhase
    {
        //Public Variables:
        public RoomMapperLocateCeiling locateCeiling;
        public RoomMapperLocateWall locateWall;
        public ChildActivator cursor;
        public LineRenderer cornerLine;
        public LineRenderer progress;
        public TMP_Text instructions;
        public GameObject cornerMarker;
        
        //Private Variables:
        private bool _onCeiling;
        private float _lerpSpeed = 3.5f;
        private float _maxCastDistance = 6f;
        private float _minWallWidth = .4572f;
        private string _markFirstCornerText = "MARK\nFIRST\nCORNER";
        private string _markNextCornerText = "MARK\nNEXT\nCORNER";
        private string _closurePossibleText = "ALL DONE?";
        private List<GameObject> _cornerMarkers;
        private List<Vector3> _ceilingCorners;
        private bool _closurePossible;
        private bool _overlapping;
        
        //Startup:
        protected override void Awake()
        {
            base.Awake();
            
            //sets:
            cornerMarker.SetActive(false);
        }

        private void OnEnable()
        {
            //sets:
            _onCeiling = false;
            _overlapping = false;
            _closurePossible = false;
            _ceilingCorners = new List<Vector3>();
            _cornerMarkers = new List<GameObject>();
            ChangeInstructions(_markFirstCornerText);
            
            //snap locator pose:
            cursor.transform.position = locateCeiling.transform.position;
            Vector3 flatForward = Vector3.ProjectOnPlane(ovrCameraRig.rightControllerAnchor.forward, Vector3.up);
            cursor.transform.rotation = Quaternion.LookRotation(flatForward);
            
            //lines:
            progress.positionCount = 2;
            progress.SetPositions(new Vector3[]{locateCeiling.transform.position, locateCeiling.transform.position});
            float height = locateCeiling.transform.position.y - locateWall.transform.position.y;
            cornerLine.SetPosition(1, new Vector3(0, -height, 0));
        }

        //Shutdown:
        private void OnDisable()
        {
            //clean up:
            foreach (var marker in _cornerMarkers)
            {
                Destroy(marker);
            }
        }

        //Loops:
        private void Update()
        {
            //overlap detection:
            if (_ceilingCorners.Count > 2)
            {
                //check all previous lines for an overlap:
                Vector3 activeLineStart = _ceilingCorners[_ceilingCorners.Count - 1];
                Vector3 activeLineEnd = cursor.transform.position;
                bool foundOverlap = false;
                for (int i = 0; i < _ceilingCorners.Count - 2; i++)
                {
                    Vector3 existingLineStart = _ceilingCorners[i];
                    Vector3 existingLineEnd = _ceilingCorners[i + 1];

                    //do lines overlap?
                    if (MathUtilities.AreLinesIntersecting(activeLineStart, activeLineEnd, existingLineStart, existingLineEnd))
                    {
                        foundOverlap = true;
                        break;
                    }
                }

                //instructions visibility:
                if (foundOverlap && !_overlapping)
                {
                    _overlapping = true;
                    instructions.transform.parent.gameObject.SetActive(false);
                }

                if (!foundOverlap && _overlapping)
                {
                    _overlapping = false;
                    instructions.transform.parent.gameObject.SetActive(true);
                }
            }
            
            //parts:
            Plane ceiling = new Plane(Vector3.down, locateCeiling.transform.position);
            Ray castRay = new Ray(ovrCameraRig.rightControllerAnchor.position, ovrCameraRig.rightControllerAnchor.forward);
            float castDistance;
            
            //cursor state:
            if (ceiling.Raycast(castRay, out castDistance))
            {
                if (!_onCeiling)
                {
                    _onCeiling = true;
                    cursor.Activate(1);
                }
            }
            else
            {
                if (_onCeiling)
                {
                    _onCeiling = false;
                    cursor.Activate(0);
                }
            }

            //clamp:
            if (castDistance <= 0 || castDistance > _maxCastDistance)
            {
                castDistance = _maxCastDistance;
            }

            //position:
            Vector3 position = castRay.GetPoint(castDistance);
            if (_onCeiling)
            {
                position.y = locateCeiling.transform.position.y;
                //snap to ceiling:
                cursor.transform.position = new Vector3(cursor.transform.position.x, locateCeiling.transform.position.y, cursor.transform.position.z);
            }
            cursor.transform.position = Vector3.Lerp(cursor.transform.position,position, Time.deltaTime * _lerpSpeed);

            //rotation:
            if (_onCeiling)
            {
                Vector3 flatForward = Vector3.ProjectOnPlane(ovrCameraRig.rightControllerAnchor.forward, Vector3.up);
                cursor.transform.rotation = Quaternion.Slerp(cursor.transform.rotation, Quaternion.LookRotation(flatForward), Time.deltaTime * _lerpSpeed);
            }
            else
            {
                Quaternion rotation = Quaternion.LookRotation(ovrCameraRig.rightControllerAnchor.forward);
                cursor.transform.rotation = Quaternion.Slerp(cursor.transform.rotation, rotation, Time.deltaTime * _lerpSpeed);
            }
            
            //perimeter update:
            if (_onCeiling)
            {
                progress.SetPosition(progress.positionCount - 1, cursor.transform.position);
            }
            
            //closure state:
            bool canClose = _ceilingCorners.Count > 2 && Vector3.Distance(cursor.transform.position, _ceilingCorners[0]) < _minWallWidth;
            if (!_closurePossible && canClose)
            {
                _closurePossible = true;
                ChangeInstructions(_closurePossibleText);
            }

            if (_closurePossible && !canClose && !_overlapping)
            {
                _closurePossible = false;
                ChangeInstructions(_markNextCornerText);
            }
            
            //input:
            if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
            {
                if (_overlapping && !_closurePossible)
                {
                    return;
                }

                //extend perimeter:
                if (_onCeiling)
                {
                    //first corner?
                    if (_ceilingCorners.Count == 0)
                    {
                        ChangeInstructions(_markNextCornerText);
                        
                        //reset to only corners in perimeter on first corner marking:
                        progress.SetPosition(0, cursor.transform.position);
                        _ceilingCorners.Add(cursor.transform.position);
                        
                        PlaceCornerMarker();
                        return;
                    }

                    //loop closure?
                    if (_closurePossible)
                    {
                        //close loop:
                        _ceilingCorners.Add(_ceilingCorners[0]);
                        
                        //convert to anchor space and cache:
                        for (int i = 0; i < _ceilingCorners.Count; i++)
                        {
                            _ceilingCorners[i] = RoomAnchor.Instance.transform.InverseTransformPoint(_ceilingCorners[i]);
                        }
                        RoomMapper.Instance.CeilingCorners = _ceilingCorners.ToArray();
                        
                        //continue:
                        Next();
                        return;
                    }
                    
                    //plot:
                    RecordNewCorner();
                    PlaceCornerMarker();
                }
            }
        }
        
        //Private Methods:
        private void ChangeInstructions(string newInstructions)
        {
            //this exists to have the text "pop" after it is changed:
            instructions.transform.parent.gameObject.SetActive(false);
            instructions.text = newInstructions;
            instructions.transform.parent.gameObject.SetActive(true);
        }

        private void RecordNewCorner()
        {
            //extend perimeter and mark corner:
            progress.positionCount++;
            progress.SetPosition(progress.positionCount-1, cursor.transform.position);
            _ceilingCorners.Add(cursor.transform.position);
        }
        
        private void PlaceCornerMarker()
        {
            GameObject marker = Instantiate(cornerMarker, cursor.transform.position, Quaternion.identity, transform);
            _cornerMarkers.Add(marker);
            marker.SetActive(true);
        }
    }
}