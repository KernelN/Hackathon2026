using UnityEngine;

namespace Hackathon.Game.Clients
{
    [System.Serializable]
    public struct LocationRequest
    {
        [SerializeField] LocationSO location;
        [SerializeField] LocationSO.Tag locationTag;
        [SerializeField] float minDistance;
        [SerializeField] float maxDistance;

        public bool IsLocationGood(Location targetLocation, float distanceFromPrev)
        {
            //If is beyond distance range, place is not valid
            //(possibly error of the previous place)
            if(!IsWithinRange(distanceFromPrev, minDistance, maxDistance))
                return false;
            
            //If Request asks for specific location, tour needs to have it
            if(location) 
            {
                if(targetLocation.SO != location)
                    return false;
            }
            //If location does not match request tag, tour is not valid
            else if (!targetLocation.SO.HasTag(locationTag)) return false;
            
            
            return true;
        }
        
        static bool IsWithinRange(float val, float a, float b)
        {
            //Only check if b is 0, as distance will never be less than 0
            if(b == 0) return val >= a;
            return val >= a && val <= b;
        }
    }
}
