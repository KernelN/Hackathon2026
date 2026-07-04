using System.Collections.Generic;
using UnityEngine;
using Universal;

namespace Hackathon.Game
{
    public class MapManager : Singleton<MapManager>
    {
        [SerializeField] Location[] locations;
        Dictionary<LocationSO.Tag, List<Location>> locationsByTag;
        [SerializeField] Universal.FadeController mapBlockFader;

        new void Awake()
        {
            base.Awake();
            
            if(inst != this) return;
            
            locationsByTag = new Dictionary<LocationSO.Tag, List<Location>>();

            //Fill dictionary with lists of locations ordered by TAG
            foreach (var loc in locations)
                for (int j = 0; j < loc.Tags.Count; j++)
                    if (locationsByTag.TryGetValue(loc.Tags[j], out var locList))
                        locList.Add(loc);
                    else 
                        locationsByTag.Add(loc.Tags[j], new List<Location> { loc });
        }
        public void EnableMap(bool enable)
        {
            if(enable) mapBlockFader.FadeOut();
            else mapBlockFader.FadeIn();
        }
        public List<Location> GetLocationsByTag(LocationSO.Tag tag, Location source = null)
        {
            if(!source)
                return locationsByTag[tag];
            
            List<Location> locationsByCloseness = 
                GetLocationsByCloseness(locationsByTag[tag], source);
            
            return locationsByCloseness;
        }
        List<Location> GetLocationsByCloseness(List<Location> locList, Location source)
        {
            List<Location> result = new List<Location>(locList);
            result.Sort((a, b) =>
            {
                float distA = source.SqrDist(a);
                float distB = source.SqrDist(b);
                return distA < distB ? 1 : -1;
            });
            return result;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tag">Location Tag</param>
        /// <param name="source">If passing location, it will return the list ordered by closeness to said location</param>
        /// <returns></returns>
        #if UNITY_EDITOR
        [ContextMenu("Find Locations")]
        void GetAllLocations() 
            => locations = FindObjectsByType<Location>();
        #endif
    }
}