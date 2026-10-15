using System.Collections.Generic;
using UnityEngine;

namespace Hackathon.Game
{
    public class Location : MonoBehaviour
    {
        [SerializeField] LocationSO so;

        [SerializeField] UnityEngine.UI.Image pinImage;
        [SerializeField] UnityEngine.UI.Image[] tagCircles;
        [SerializeField] GameObject stopBadge;
        [SerializeField] TMPro.TextMeshProUGUI stopNumberText;

        public System.Action<Location> OnSelected;
        public List<LocationSO.Tag> Tags => so.Tags;
        public string Name => so.Name;
        public LocationSO SO => so;
        bool isSelected = false;
        public bool IsSelected => isSelected;
        int tourIndex = -1;
        public int TourIndex => tourIndex;

        void Awake()
        {
            SetGameObject();

            if (pinImage != null)
                pinImage.raycastPadding = new Vector4(-8, -8, -8, -8);
        }

        public void SelectLocation() => OnSelected?.Invoke(this);

        public void SetTourIndex(int index)
        {
            tourIndex = index;
            isSelected = index >= 0;

            if (stopBadge != null)
            {
                stopBadge.SetActive(isSelected);
                if (isSelected && stopNumberText != null)
                    stopNumberText.text = (tourIndex + 1).ToString();
            }
        }

        public void SetSelected(bool selected)
        {
            if (!selected)
                SetTourIndex(-1);
            else if (tourIndex < 0)
                SetTourIndex(0);
        }
        public float SqrDist(Location otherLocation) 
            => (transform.position - otherLocation.transform.position).sqrMagnitude;

        public float Dist(Location otherLocation) 
            => (transform.position - otherLocation.transform.position).magnitude;

        [ContextMenu("Set Game Object")]
        void SetGameObject()
        {
            if (!so) return;
            
            gameObject.name = so.name;

            //Set circles
            //If it has less tags than circles, loop through circles
            int i = 0;
            while (i < tagCircles.Length)
            {
#if UNITY_EDITOR
                UnityEditor.EditorUtility.SetDirty(gameObject);
                UnityEditor.Undo.RecordObject(tagCircles[i], "Set " + so.name + " color");
#endif
                tagCircles[i].color = LocationSO.TagColor(so.Tags[i % so.Tags.Count]);
                
                i++;
            }
        }
    }
}