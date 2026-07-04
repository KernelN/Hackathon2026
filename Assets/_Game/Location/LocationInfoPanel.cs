using UnityEngine;
using UnityEngine.UI;

namespace Hackathon.Game
{
    public class LocationInfoPanel : MonoBehaviour
    {
        [SerializeField] Universal.FadeController fadeController;
        
        [SerializeField] Image image;
        [SerializeField] TMPro.TextMeshProUGUI label;
        [SerializeField] TMPro.TextMeshProUGUI description;
        [SerializeField] TMPro.TextMeshProUGUI tag1;
        [SerializeField] Image tag1Bg;
        [SerializeField] TMPro.TextMeshProUGUI tag2;
        [SerializeField] Image tag2Bg;

        public void SetInfo(LocationSO location)
        {
            image.sprite = location.Sprite;
            label.text = location.Name;
            description.text = location.Description;

            tag1.text = TagToText(location.Tags[0]);
            tag1Bg.color = LocationSO.TagColor(location.Tags[0]);

            if (location.Tags.Count == 1)
            {
                tag2Bg.gameObject.SetActive(false);
                return;
            }
            
            tag2Bg.gameObject.SetActive(true);
            tag2.text = TagToText(location.Tags[1]);
            tag2Bg.color = LocationSO.TagColor(location.Tags[1]);
        }
        public static string TagToText(LocationSO.Tag value)
        {
            var stringVal = value.ToString();   
            var bld = new System.Text.StringBuilder();

            //Skip first letter
            bld.Append(stringVal[0]);
            for (var i = 1; i < stringVal.Length; i++)
            {
                if (char.IsUpper(stringVal[i])) bld.Append(" ");

                bld.Append(stringVal[i]);
            }

            return bld.ToString();
        }
        
        public void Appear(LocationSO location = null)
        {
            if(location) SetInfo(location);
            fadeController.FadeIn(true);
        }

        public void Disappear() => fadeController.FadeOut(true);
    }
}
