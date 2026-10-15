using UnityEngine;
using UnityEngine.UIElements;
using Hackathon.Audio;

namespace Hackathon.UI
{
    public class SettingsModalController : MonoBehaviour
    {
        VisualElement overlay;
        Slider musicSlider;
        Toggle musicToggle;
        Slider sfxSlider;
        Toggle sfxToggle;
        Button closeBtn;

        bool isInitialized = false;

        public event System.Action OnOpened;
        public event System.Action OnClosed;

        public bool IsOpen => overlay != null && !overlay.ClassListContains("hidden") && overlay.style.display != DisplayStyle.None;

        public void Initialize(VisualElement root)
        {
            if (root == null) return;

            // Support both naming schemes
            overlay = root.Q<VisualElement>("settings-modal") ?? root.Q<VisualElement>("settings-overlay");
            musicSlider = root.Q<Slider>("slider-music-volume") ?? root.Q<Slider>("music-slider");
            musicToggle = root.Q<Toggle>("toggle-music-mute");
            sfxSlider = root.Q<Slider>("slider-sfx-volume") ?? root.Q<Slider>("sfx-slider");
            sfxToggle = root.Q<Toggle>("toggle-sfx-mute");
            closeBtn = root.Q<Button>("btn-close-settings") ?? root.Q<Button>("close-btn");

            if (musicSlider != null)
            {
                musicSlider.RegisterValueChangedCallback(evt =>
                {
                    float val = musicSlider.highValue > 1.5f ? evt.newValue / 100f : evt.newValue;
                    if (AudioManager.inst != null)
                        AudioManager.inst.SetMusicVolume(val);
                });
            }

            if (musicToggle != null)
            {
                musicToggle.RegisterValueChangedCallback(evt =>
                {
                    if (AudioManager.inst != null)
                    {
                        AudioManager.inst.SetMusicMuted(evt.newValue);
                        AudioManager.inst.PlayButtonClick();
                    }
                });
            }

            if (sfxSlider != null)
            {
                sfxSlider.RegisterValueChangedCallback(evt =>
                {
                    float val = sfxSlider.highValue > 1.5f ? evt.newValue / 100f : evt.newValue;
                    if (AudioManager.inst != null)
                        AudioManager.inst.SetSfxVolume(val);
                });
            }

            if (sfxToggle != null)
            {
                sfxToggle.RegisterValueChangedCallback(evt =>
                {
                    if (AudioManager.inst != null)
                    {
                        AudioManager.inst.SetSfxMuted(evt.newValue);
                        AudioManager.inst.PlayButtonClick();
                    }
                });
            }

            if (closeBtn != null)
            {
                closeBtn.clicked += Close;
            }

            // Click outside card on overlay backdrop to close
            if (overlay != null)
            {
                overlay.RegisterCallback<PointerDownEvent>(evt =>
                {
                    if (evt.target == overlay)
                    {
                        Close();
                    }
                });
            }

            // Escape key handler
            root.RegisterCallback<KeyDownEvent>(evt =>
            {
                if (evt.keyCode == KeyCode.Escape && IsOpen)
                {
                    Close();
                    evt.StopPropagation();
                }
            });

            isInitialized = true;
            UpdateAll();
        }

        public void Open()
        {
            if (!isInitialized) return;
            UpdateAll();

            if (overlay != null)
            {
                overlay.RemoveFromClassList("hidden");
                overlay.RemoveFromClassList("settings-overlay--hidden");
                overlay.style.display = DisplayStyle.Flex;
            }

            if (AudioManager.inst != null)
                AudioManager.inst.PlayPopupOpen();

            OnOpened?.Invoke();
        }

        public void Close()
        {
            if (overlay != null)
            {
                overlay.AddToClassList("hidden");
                overlay.AddToClassList("settings-overlay--hidden");
                overlay.style.display = DisplayStyle.None;
            }

            if (AudioManager.inst != null)
                AudioManager.inst.PlayButtonClick();

            OnClosed?.Invoke();
        }

        public void Toggle()
        {
            if (IsOpen) Close();
            else Open();
        }

        public void UpdateAll()
        {
            if (AudioManager.inst == null) return;

            if (musicSlider != null)
            {
                float targetVal = musicSlider.highValue > 1.5f ? AudioManager.inst.MusicVolume * 100f : AudioManager.inst.MusicVolume;
                musicSlider.SetValueWithoutNotify(targetVal);
            }

            if (musicToggle != null)
            {
                musicToggle.SetValueWithoutNotify(AudioManager.inst.MusicMuted);
            }

            if (sfxSlider != null)
            {
                float targetVal = sfxSlider.highValue > 1.5f ? AudioManager.inst.SfxVolume * 100f : AudioManager.inst.SfxVolume;
                sfxSlider.SetValueWithoutNotify(targetVal);
            }

            if (sfxToggle != null)
            {
                sfxToggle.SetValueWithoutNotify(AudioManager.inst.SfxMuted);
            }
        }
    }
}
