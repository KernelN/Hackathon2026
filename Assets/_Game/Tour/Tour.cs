using System.Collections.Generic;
using UnityEngine;

namespace Hackathon.Game
{
    [System.Serializable]
    public class Tour
    {
        [SerializeField] internal List<Location> locations;
        [SerializeField] List<float> distances;

        public Tour()
        {
            locations = new List<Location>();
            distances = new List<float>();
        }
        
        public List<float> Distances => distances;

        public bool ContainsLocation(Location location)
            => locations.Contains(location);
        public void RemoveLocation(int index, bool updateData = true)
        {
            locations.RemoveAt(index);
            distances.RemoveAt(index);
            
            if(updateData)
                UpdateLocationsData();
        }
        public void RemoveLocation(Location location)
        {
            int index = locations.IndexOf(location);
            RemoveLocation(index, false);
        }
        public void AddLocation(Location location)
        {
            locations.Add(location);
                
            //Add location dist
            distances.Add(locations.Count > 1 ? location.Dist(locations[^2]) : 0);
        }
        public void MoveLocationIndex(int index, int newIndex)
        {
            Location loc = locations[index];
            RemoveLocation(index, false);
            locations.Insert(newIndex, loc);
            
            UpdateLocationsData();
        }
        void UpdateLocationsData()
        {
            distances.Clear();
            distances.Add(0);
            for (int i = 1; i < locations.Count; i++)
            {
                //Cache locations
                Location cLoc = locations[i];
                Location pLoc = locations[i - 1];
                
                //Update location dist
                distances.Add(cLoc.Dist(pLoc));
            }
        }
        // public bool HasLocation(LocationSO location)
        // {
        //     bool hasLocation = false;
        //     foreach (TourPoint p in locations)
        //     {
        //         p.
        //     }
        //
        //     return hasLocation;
        // }
        // /// <summary>
        // /// Returns the location that best matches tags, with match percentage
        // /// </summary>
        // /// <param name="tags"></param>
        // /// <param name="bestLocation"></param>
        // /// <param name="bestMatch"></param>
        // public void HasLocationWithTags(List<LocationSO.Tag> tags, 
        //     out LocationSO bestLocation, out float bestMatch)
        // {
        //     bestLocation = locations[0];
        //     bestMatch = 0;
        //     for (int i = 0; i < locations.Count; i++)
        //     {
        //         // float cMatch = locations[i].HasSameTags(tags);
        //         // if(cMatch < bestMatch) continue;
        //         // bestLocation = locations[i];
        //         // bestMatch = cMatch;
        //     }
        // }
    }
}