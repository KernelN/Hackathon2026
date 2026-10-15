using UnityEngine;
using UnityEngine.UI;

namespace Hackathon.Game
{
    public class LocationInfoPanel : MonoBehaviour
    {
        [SerializeField] Universal.FadeController fadeController;
        [SerializeField] RectTransform rectTransform;
        
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

        public void Disappear()
        {
            fadeController.FadeOut(true);
        }

        public void SetPositionAndClamp(Vector3 targetWorldPos)
        {
            transform.position = targetWorldPos;
            if (rectTransform == null) return;

            Vector3[] corners = new Vector3[4];
            rectTransform.GetWorldCorners(corners);

            Vector3 offset = Vector3.zero;
            float margin = 15f;
            float minX = margin;
            float maxX = Screen.width - margin;
            float minY = margin;
            float maxY = Screen.height - margin;

            if (corners[0].x < minX) offset.x += (minX - corners[0].x);
            if (corners[2].x > maxX) offset.x -= (corners[2].x - maxX);
            if (corners[0].y < minY) offset.y += (minY - corners[0].y);
            if (corners[2].y > maxY) offset.y -= (corners[2].y - maxY);

            transform.position += offset;
        }
    }
}
