using UnityEngine;
using UnityEngine.EventSystems;

namespace Hackathon.Game
{
    public class LoctionTooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] Location location;
        [SerializeField] LocationInfoPanel locationInfoPanel;
        [SerializeField] Transform infoPanelPivot;
        
        void Awake()
        {
            if (locationInfoPanel == null)
                locationInfoPanel = Object.FindAnyObjectByType<LocationInfoPanel>();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (locationInfoPanel != null)
            {
                if (infoPanelPivot != null)
                    locationInfoPanel.SetPositionAndClamp(infoPanelPivot.position);
                else
                    locationInfoPanel.SetPositionAndClamp(transform.position + new Vector3(0, 50, 0));

                if (location != null)
                    locationInfoPanel.Appear(location.SO);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (locationInfoPanel != null)
                locationInfoPanel.Disappear();
        }
    }
}
