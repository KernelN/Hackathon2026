using System.Collections.Generic;
using UnityEngine;

namespace Hackathon.Game
{
    public class Location : MonoBehaviour
    {
        [SerializeField] LocationSO so;

        public System.Action<Location> OnSelected;
        public List<LocationSO.Tag> Tags => so.Tags;
        public string Name => so.Name;

        public void SelectLocation() => OnSelected?.Invoke(this);

        public float SqrDist(Location otherLocation) 
            => (transform.position - otherLocation.transform.position).sqrMagnitude;

        public float Dist(Location otherLocation) 
            => (transform.position - otherLocation.transform.position).magnitude;
    }
}
