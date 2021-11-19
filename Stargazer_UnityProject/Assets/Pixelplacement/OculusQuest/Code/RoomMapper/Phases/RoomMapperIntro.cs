using UnityEngine.UI;

namespace Pixelplacement.XRTools
{
    public class RoomMapperIntro : RoomMapperPhase
    {
        //Public Variables:
        public Button goButton;
        
        //Startup:
        private void OnEnable()
        {
            //hooks:
            goButton.onClick.AddListener(HandleGo);
        }
        
        //Shutdown:
        private void OnDisable()
        {
            //hooks:
            goButton.onClick.RemoveListener(HandleGo);
        }
        
        //Event Handlers:
        private void HandleGo()
        {
            Next();
        }
    }
}