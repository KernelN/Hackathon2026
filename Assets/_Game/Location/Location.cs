using System.Collections.Generic;
using UnityEngine;

namespace Hackathon.Game
{
    public class Location : MonoBehaviour
    {
        [SerializeField] LocationSO so;

        [SerializeField] UnityEngine.UI.Image[] tagCircles;

        public System.Action<Location> OnSelected;
        public List<LocationSO.Tag> Tags => so.Tags;
        public string Name => so.Name;
        public LocationSO SO => so;

        void Awake()
        {
            SetGameObject();
        }
        public void SelectLocation() => OnSelected?.Invoke(this);
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
                UnityEditor.EditorUtility.SetDirty(gameObject);
                UnityEditor.Undo.RecordObject(tagCircles[i], "Set "+so.name+" color");
                tagCircles[i].color = LocationSO.TagColor(so.Tags[i % so.Tags.Count]);
                
                i++;
            }
        }
    }
}