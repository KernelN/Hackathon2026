using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Hackathon.Game
{
    [CreateAssetMenu(fileName = "LocationSO", menuName = "Hackathon/LocationSO")]
    //Could use LocationSO<T> and implement different tag enums for different locations
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
        
        [FormerlySerializedAs("locationTags")] 
        [SerializeField] protected List<Tag> tags;
        [SerializeField] protected string locationName;
        [SerializeField] protected string description;
        
        public List<Tag> Tags => tags;
        public string Name => locationName;
        public string Description => description;

        /// <summary>
        /// Checks whether the location tags match the received tag array or not 
        /// </summary>
        /// <param name="tags"></param>
        /// <returns>Match percentage</returns>
        public float HasSameTags(List<Tag> tags)
        {
            int tagCount = 0;
            for (var i = 0; i < tags.Count; i++)
                if (this.tags.Contains(tags[i]))
                    tagCount++;

            return (float)tagCount / tags.Count;
        }
        public bool HasTag(Tag tag) => tags.Contains(tag);
        public bool IsSameLocation(LocationSO otherLocation) => otherLocation.name == name;
    }
}