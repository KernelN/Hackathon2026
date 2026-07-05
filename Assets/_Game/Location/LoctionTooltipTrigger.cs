using UnityEngine;
using UnityEngine.EventSystems;

namespace Hackathon.Game
{
    public class LoctionTooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] Location location;
        [SerializeField] LocationInfoPanel locationInfoPanel;
        [SerializeField] Transform infoPanelPivot;
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            if(infoPanelPivot)
                locationInfoPanel.transform.position = infoPanelPivot.position;
            if(location)
                locationInfoPanel.Appear(location.SO);
            //Debug.Log("Pointer entered: " + location.Name);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            //Debug.Log("Pointer exited: " + location.Name);
            locationInfoPanel.Disappear();
        }
    }
}
