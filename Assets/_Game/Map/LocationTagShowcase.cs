using UnityEngine;
using UnityEngine.UI;

namespace Hackathon.Game
{
    public class LocationTagShowcase : MonoBehaviour
    {
        [SerializeField] new LocationSO.Tag tag;
        [SerializeField] Image img;
        [SerializeField] TMPro.TextMeshProUGUI label;

        void Start() => SetShowcase();
        [ContextMenu("Set Showcase")]
        void SetShowcase()
        {
            img.color = LocationSO.TagColor(tag);
            label.text = LocationInfoPanel.TagToText(tag);
        }
    }
}
