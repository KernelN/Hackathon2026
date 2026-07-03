using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Universal.UI
{
    //Heavily inspired by:
    //https://github.com/dipen-apptrait/Vertical-drag-drop-listview-unity.git
    public class DragController : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        public RectTransform currentTransform;
        GameObject mainContent;
        Vector3 currentPossition;

        int totalChild;
        int cIndex;

        /// <summary>
        /// 0 = old index, 1 = new index
        /// </summary>
        public UnityEvent<int[]> OnLayoutIndexChanged;

        public void OnPointerDown(PointerEventData eventData)
        {
            currentPossition = currentTransform.position;
            mainContent = currentTransform.parent.gameObject;
            totalChild = mainContent.transform.childCount;
            cIndex = transform.GetSiblingIndex();
        }

        public void OnDrag(PointerEventData eventData)
        {
            currentTransform.position =
                new Vector3(currentTransform.position.x, eventData.position.y, currentTransform.position.z);

            for (int i = 0; i < totalChild; i++)
            {
                if (i != currentTransform.GetSiblingIndex())
                {
                    Transform otherTransform = mainContent.transform.GetChild(i);
                    int distance = (int) Vector3.Distance(currentTransform.position,
                        otherTransform.position);
                    if (distance <= 10)
                    {
                        Vector3 otherTransformOldPosition = otherTransform.position;
                        otherTransform.position = new Vector3(otherTransform.position.x, currentPossition.y,
                            otherTransform.position.z);
                        currentTransform.position = new Vector3(currentTransform.position.x, otherTransformOldPosition.y,
                            currentTransform.position.z);
                        currentTransform.SetSiblingIndex(otherTransform.GetSiblingIndex());
                        currentPossition = currentTransform.position;
                    }
                }
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            currentTransform.position = currentPossition;
            
            int nIndex = transform.GetSiblingIndex();
            
            if(nIndex == cIndex) return;
            
            OnLayoutIndexChanged?.Invoke(new int[] {cIndex, nIndex});
            
            cIndex = nIndex;
        }
    }
}
