using UnityEngine;

namespace Pixelplacement
{
    public class PopInTransform : MonoBehaviour
    {
        //Public Variables:
        public float duration = .35f;
        public float delay;
    
        //Private Variables:
        private Vector3 _initialScale;
    
        //Startup:
        private void Awake()
        {
            //sets:
            _initialScale = transform.localScale;
        }

        private void OnEnable()
        {
            Tween.LocalScale(transform, _initialScale * .25f, _initialScale, duration, delay, Tween.EaseOutBack);
        }
    }
}