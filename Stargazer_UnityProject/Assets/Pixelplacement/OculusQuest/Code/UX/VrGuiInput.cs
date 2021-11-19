using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR;

namespace Pixelplacement.XRTools
{
    /// <summary>
    /// Offers a simple way of using Unity GUI with a VR device for buttons and similar input controls.
    /// NOTE: Does not support sliders yet.
    /// </summary>
    public class VrGuiInput : MonoBehaviour
    {
        //Public Variables:
        public LineRenderer pointerTemplate;

        //Private Variables:
        private LineRenderer _leftPointer;
        private LineRenderer _rightPointer;
        private InputDevice _leftController;
        private InputDevice _rightController;
        private Selectable _closest;
        private Selectable _selected;
        private RectTransform _closestRectTransform;
        private Renderer _closestRenderer;
        private float _boundsInflation = .025f; //ui bounds can be very thin so this helps with intersection detection
        private List<LineRenderer> _pointers = new List<LineRenderer>();
        private bool _leftTriggerDown;
        private bool _rightTriggerDown;

        //Startup:
        private IEnumerator Start()
        {
            //pointers construction:
            pointerTemplate.gameObject.SetActive(false);
            pointerTemplate.useWorldSpace = false;
            _leftPointer = Instantiate(pointerTemplate);
            _rightPointer = Instantiate(pointerTemplate);
            _leftPointer.gameObject.SetActive(true);
            _rightPointer.gameObject.SetActive(true);
            _pointers.Add(_leftPointer);
            _pointers.Add(_rightPointer);

            //find controllers:
            while (!_leftController.isValid)
            {
                _leftController = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
                yield return new WaitForSeconds(.5f);
            }

            while (!_rightController.isValid)
            {
                _rightController = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
                yield return new WaitForSeconds(.5f);
            }
        }

        //Loops:
        private void Update()
        {
            //toggle based on gui availability:
            bool active = Selectable.allSelectablesArray.Length > 0;
            _leftPointer.gameObject.SetActive(active);
            _rightPointer.gameObject.SetActive(active);

            if (!active)
            {
                Clear();
                return;
            }

            //update controllers:
            UpdatePointer(_leftController, _leftPointer.transform);
            UpdatePointer(_rightController, _rightPointer.transform);

            //closest buckets:
            float highestDot = float.MinValue;
            Selectable closest = null;
            LineRenderer closestPointer = null;

            //what is the closest we are pointing towards?
            foreach (var selectable in Selectable.allSelectablesArray)
            {
                foreach (var pointer in _pointers)
                {
                    Vector3 to = Vector3.Normalize(selectable.transform.position - pointer.transform.position);
                    float dot = Vector3.Dot(pointer.transform.forward, to);

                    if (dot > highestDot)
                    {
                        highestDot = dot;
                        closest = selectable;
                        closestPointer = pointer;
                    }
                }
            }

            //new closest:
            if (_closest != closest)
            {
                _closest = closest;
                _closestRectTransform = closest.GetComponent<RectTransform>();
                _closestRenderer = closest.GetComponent<Renderer>();
            }

            //targeting detection:
            if (_closest)
            {
                //cast pieces:
                Plane plane = new Plane(_closest.transform.forward, _closest.transform.position);
                Ray ray = new Ray(closestPointer.transform.position, closestPointer.transform.forward);
                float distance;

                //within bounds?
                if (plane.Raycast(ray, out distance))
                {
                    Bounds bounds = new Bounds(_closest.transform.position, Vector3.zero);

                    if (_closestRectTransform)
                    {
                        //corners:
                        Vector3[] corners = new Vector3[4];
                        _closestRectTransform.GetWorldCorners(corners);

                        //inflated bounds:
                        foreach (var corner in corners)
                        {
                            bounds.Encapsulate(corner);
                        }

                        bounds.Expand(Vector3.one * _boundsInflation);
                    }
                    else
                    {
                        bounds = _closestRenderer.bounds;
                    }

                    //contained?
                    if (bounds.Contains(ray.GetPoint(distance)))
                    {
                        if (_selected != _closest)
                        {
                            _selected = _closest;
                            _closest.Select();
                        }

                        closestPointer.SetPositions(new Vector3[] {Vector3.zero, new Vector3(0, 0, distance)});
                    }
                    else
                    {
                        Clear();

                        //reset pointer length:
                        Vector3[] points = new Vector3[pointerTemplate.positionCount];
                        pointerTemplate.GetPositions(points);
                        closestPointer.SetPositions(points);
                    }
                }
            }

            //input:
            if (_selected)
            {
                DetectInput(_leftController, ref _leftTriggerDown);
                DetectInput(_rightController, ref _rightTriggerDown);
            }
        }
        
        //Public Methods:
        public static void Establish()
        {
            if (!FindObjectOfType<VrGuiInput>())
            {
                VrGuiInput vrGuiInput = new GameObject("(VrGuiInput)").AddComponent<VrGuiInput>();
                vrGuiInput.pointerTemplate = Resources.Load<LineRenderer>("Pointer");
            }

            if (!FindObjectOfType<EventSystem>())
            {
                new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
            }
        }
        
        //Private Methods:
        private void DetectInput(InputDevice source, ref bool status)
        {
            bool down;
            source.TryGetFeatureValue(CommonUsages.triggerButton, out down);

            if (down && !status)
            {
                status = true;
                ExecuteEvents.Execute(_selected.gameObject, new BaseEventData(EventSystem.current), ExecuteEvents.submitHandler);
            }

            if (!down && status)
            {
                status = false;
            }
        }

        private void UpdatePointer(InputDevice source, Transform target)
        {
            Vector3 position;
            Quaternion rotation;
            source.TryGetFeatureValue(CommonUsages.devicePosition, out position);
            source.TryGetFeatureValue(CommonUsages.deviceRotation, out rotation);
            target.transform.SetPositionAndRotation(position, rotation);
        }

        private void Clear()
        {
            if (_selected)
            {
                _selected = null;
                EventSystem.current.SetSelectedGameObject(null);
            }
        }
    }
}