using System.Collections.Generic;
using UnityEngine;

namespace Hackathon.Game
{
    public class MapManager : MonoBehaviour
    {
        [SerializeField] Location[] locations;
        Dictionary<LocationSO.Tag, List<Location>> locationsByTag;

        void Awake()
        {
            locationsByTag = new Dictionary<LocationSO.Tag, List<Location>>();

            //Fill dictionary with lists of locations ordered by TAG
            foreach (var location in locations)
                for (int j = 0; j < location.Tags.Count; j++)
                    if (locationsByTag.TryGetValue(location.Tags[j], out var locList))
                        locList.Add(location);
                    else 
                        locationsByTag.Add(location.Tags[j], new List<Location> { location });
        }
        
        [ContextMenu("Find Locations")]
        public void GetAllLocations() 
            => locations = FindObjectsByType<Location>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tag">Location Tag</param>
        /// <param name="source">If passing location, it will return the list ordered by closeness to said location</param>
        /// <returns></returns>
        public List<Location> GetLocationsByTag(LocationSO.Tag tag, Location source = null)
        {
            if(!source)
                return locationsByTag[tag];
            
            List<Location> locationsByCloseness = locationsByTag[tag];
            locationsByCloseness.Sort((a, b) =>
            {
                float distA = source.SqrDist(a);
                float distB = source.SqrDist(b);
                return distA < distB ? 1 : -1;
            });
            
            return locationsByCloseness;
        }
    }
}