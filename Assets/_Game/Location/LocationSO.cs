using System.Collections.Generic;
using UnityEngine;

namespace NAMESPACE
{
    [CreateAssetMenu(fileName = "LocationSO", menuName = "Hackathon/LocationSO")]
    public class LocationSO : ScriptableObject
    {
        public enum Tag
        {
            Museo,
            Gastronomia,
            Religion,
            EdificioHistorico,
            Comercio,
            Parque,
            Educacion
        }
        
        [SerializeField] protected List<Tag> locationTags;
        [SerializeField] protected string locationName;
        [SerializeField] protected string description;

        public int HasTags(Tag[] tags)
        {
            int tagCount = 0;
            for (var i = 0; i < tags.Length; i++)
                if (locationTags.Contains(tags[i]))
                    tagCount++;

            return tagCount;
        }
        public bool HasTag(Tag tag) => locationTags.Contains(tag);
        public bool IsSameLocation(LocationSO otherLocation) => otherLocation.name == name;
    }
}