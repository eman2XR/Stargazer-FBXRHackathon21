using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pixelplacement
{
    /// <summary>
    /// Activate a child; sequentially, by name, by index.
    /// Very helpful for logic flow as a state machine.
    /// </summary>
    [DefaultExecutionOrder(-100)]
    public class ChildActivator : MonoBehaviour
    {
        //Events:
        public Action<ChildActivator> OnLastChild;
    
        //Public Variables:
        public bool activateFirstOnEnable;
        /// <summary>
        /// Should next and previous calls loop around?
        /// </summary>
        public bool loop;
    
        //Public Properties:
        public GameObject Current
        {
            get
            {
                return _current;
            }
        
            private set
            {
                if (_current)
                {
                    //same?
                    if (value == _current)
                    {
                        return;
                    }
                
                    _current.SetActive(false);
                }

                _current = value;
                _current.SetActive(true);
            } 
        }
    
        //Private Variables:
        private Dictionary<string, GameObject> _children = new Dictionary<string, GameObject>();
        private GameObject _current;
        private bool _initialized;
    
        //Startup:
        private void OnEnable()
        {
            CacheChildren();
            DeactivateAll();
        
            if (activateFirstOnEnable)
            {
                Activate(0);
            }
        }
    
        //Public Methods:
        public void Initialize()
        {
            if (_initialized)
            {
                return;
            }
            
            CacheChildren();
        }

        public void CacheChildren()
        {
            _initialized = true;
            _children.Clear();
            foreach (Transform child in transform)
            {
                _children.Add(child.name, child.gameObject);
            }
        }
    
        public GameObject Activate(string childName)
        {
            Initialize();
            Current = _children[childName];
            return Current;
        }
    
        public void Activate(int childIndex)
        {
            Current = transform.GetChild(childIndex).gameObject;
            Current.SetActive(true);
        }

        public GameObject Next(bool disableAfterLast = false)
        {
            if (!Current)
            {
                Current = transform.GetChild(0).gameObject;
            }
            else
            {
                int index = Current.transform.GetSiblingIndex();
                index++;

                if (loop)
                {
                    index = index % transform.childCount;
                }
                else
                {
                    index = Mathf.Clamp(index, 0, transform.childCount - 1);
                }
            
                //already at last?
                if (index == Current.transform.GetSiblingIndex())
                {
                    OnLastChild?.Invoke(this);
                    if (disableAfterLast)
                    {
                        gameObject.SetActive(false);
                    }
                    return null;
                }
            
                Current = transform.GetChild(index).gameObject;
            }
        
            return Current;
        }
    
        public GameObject Previous()
        {
            if (!Current)
            {
                Current = transform.GetChild(transform.childCount - 1).gameObject;
            }
            else
            {
                int index = Current.transform.GetSiblingIndex();
                index--;
            
                if (loop)
                {
                    if (index == -1)
                    {
                        index = transform.childCount - 1;
                    }
                }
                else
                {
                    index = Mathf.Clamp(index, 0, transform.childCount);
                }
            
                //already at first?
                if (index == Current.transform.GetSiblingIndex())
                {
                    return null;
                }
            
                Current = transform.GetChild(index).gameObject;
            }

            return Current;
        }

        public GameObject Random()
        {
            int index = UnityEngine.Random.Range(0, transform.childCount);
            Current = transform.GetChild(index).gameObject;
            return Current;
        }
    
        public void DeactivateAll()
        {
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(false);
            }

            _current = null;
        }
    }
}