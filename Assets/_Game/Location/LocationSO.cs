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

        public static Color TagColor(Tag tag)
        {
            switch (tag)
            {
                case Tag.Museo: return new Color32(255, 81, 81, 255);
                case Tag.Gastronomia: return new Color32(153, 0, 255, 255);
                case Tag.Religion: return new Color32(255, 154, 235, 255);
                case Tag.EdificioHistorico: return new Color32(125, 255, 255, 255);
                case Tag.Comercio: return new Color32(111, 168, 220, 255);
                case Tag.Parque: return new Color32(117, 196, 82, 255);
                case Tag.Educacion: return new Color32(255, 217, 102, 255);
            }
            return Color.white;
        }
        
        [SerializeField] protected Sprite sprite;
        [FormerlySerializedAs("locationTags")] 
        [SerializeField] protected List<Tag> tags;
        [SerializeField] protected string locationName;
        [SerializeField] protected string description;
        
        public Sprite Sprite => sprite;
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