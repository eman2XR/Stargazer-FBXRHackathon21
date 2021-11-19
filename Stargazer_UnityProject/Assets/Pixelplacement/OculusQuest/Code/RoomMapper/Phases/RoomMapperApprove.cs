using UnityEngine;
using UnityEngine.UI;

namespace Pixelplacement.XRTools
{
    public class RoomMapperApprove : RoomMapperPhase
    {
        //Public Variables:
        public Button redoButton;
        public Button acceptButton;
        public LineRenderer wireframe;
        
        //Startup:
        private void OnEnable()
        {
            //hooks:
            redoButton.onClick.AddListener(HandleRedo);
            acceptButton.onClick.AddListener(HandleAccept);
        }
        
        //Shutdown:
        private void OnDisable()
        {
            //hooks:
            redoButton.onClick.RemoveListener(HandleRedo);
            acceptButton.onClick.RemoveListener(HandleAccept);
            
            //activate:
            wireframe.gameObject.SetActive(false);
        }
        
        //Event Handlers:
        private void HandleAccept()
        {
            RoomMapper.Instance.Save();
            RoomMapper.Instance.ShowGeometry();
            Next(true);
        }
        
        private void HandleRedo()
        {
            RoomMapper.Instance.DestroyRoom();
            wireframe.positionCount = 0;
            GoTo("LocateWall");
        }
    }
}