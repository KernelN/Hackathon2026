using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Hackathon.Audio;
using Hackathon.Game;

namespace Hackathon.UI
{
    [RequireComponent(typeof(UIDocument))]
    public class InGameSettingsController : MonoBehaviour
    {
        [SerializeField] UIDocument uiDoc;
        VisualElement outsideDarkener;
        VisualElement settingsContainer;
        Button openBtn;
        Button closeBtn;

        Slider musicSlider;
        Button musicToggleBtn;
        Slider sfxSlider;
        Button sfxToggleBtn;

        bool isOpen = false;
        public bool IsOpen => isOpen;

        [SerializeField] UnityEngine.UI.Button uguiConfigButton;

        bool isInitialized = false;

        void Awake()
        {
        }

        void OnEnable()
        {
            EnsureInitialized();
        }

        void Start()
        {
            EnsureInitialized();
            if (uguiConfigButton != null)
            {
                uguiConfigButton.onClick.RemoveAllListeners();
                uguiConfigButton.onClick.AddListener(Open);
            }

            if (MapManager.inst != null)
            {
                MapManager.inst.OnMapEnabledChanged += OnMapEnabledChanged;
                OnMapEnabledChanged(MapManager.inst.IsMapEnabled);
            }
        }

        void Update()
        {
            if (!isInitialized)
            {
                EnsureInitialized();
            }
        }

        void OnMapEnabledChanged(bool mapEnabled)
        {
            if (uguiConfigButton != null)
            {
                // When screen is darkened (mapEnabled is false, during dialogues), hide the config button completely!
                uguiConfigButton.gameObject.SetActive(mapEnabled && !isOpen);
            }
        }

        void EnsureInitialized()
        {
            if (isInitialized) return;
            if (uiDoc == null || uiDoc.rootVisualElement == null) return;
            var root = uiDoc.rootVisualElement;

            outsideDarkener = root.Q<VisualElement>("outside-darkener");
            settingsContainer = root.Q<VisualElement>("ingame-settings-container");
            openBtn = root.Q<Button>("btn-open-settings");
            closeBtn = root.Q<Button>("btn-close-settings");

            musicSlider = root.Q<Slider>("slider-music-volume");
            musicToggleBtn = root.Q<Button>("btn-toggle-music");
            sfxSlider = root.Q<Slider>("slider-sfx-volume");
            sfxToggleBtn = root.Q<Button>("btn-toggle-sfx");

            // Ensure closed initially
            if (outsideDarkener != null) outsideDarkener.AddToClassList("hidden");
            if (settingsContainer != null) settingsContainer.AddToClassList("hidden");
            if (openBtn != null) openBtn.style.display = DisplayStyle.None;
            isOpen = false;

#if UNITY_EDITOR
            ApplyVisualStyles(root);
#endif

            WireAudioControls();

            if (AudioManager.inst != null)
                AudioManager.inst.OnAudioSettingsChanged += SyncAudioValues;

            if (openBtn != null)
            {
                openBtn.clicked += Open;
            }

            if (closeBtn != null)
            {
                closeBtn.clicked += Close;
                closeBtn.RegisterCallback<PointerEnterEvent>(_ =>
                {
                    if (AudioManager.inst != null) AudioManager.inst.PlayButtonHover();
                });
            }

            // Clicking on the outside room darkener closes settings
            if (outsideDarkener != null)
            {
                outsideDarkener.RegisterCallback<PointerDownEvent>(evt =>
                {
                    if (evt.target == outsideDarkener)
                    {
                        Close();
                    }
                });
            }

            // Escape key closes settings
            root.RegisterCallback<KeyDownEvent>(evt =>
            {
                if (evt.keyCode == KeyCode.Escape && isOpen)
                {
                    Close();
                    evt.StopPropagation();
                }
            });

            isInitialized = true;
        }

        void OnDisable()
        {
            if (Time.timeScale == 0f)
                Time.timeScale = 1f;

            if (AudioManager.inst != null)
                AudioManager.inst.OnAudioSettingsChanged -= SyncAudioValues;

            if (MapManager.inst != null)
                MapManager.inst.OnMapEnabledChanged -= OnMapEnabledChanged;
        }

        void OnDestroy()
        {
            if (MapManager.inst != null)
                MapManager.inst.OnMapEnabledChanged -= OnMapEnabledChanged;
        }

        public void Open()
        {
            if (outsideDarkener != null) outsideDarkener.RemoveFromClassList("hidden");
            if (settingsContainer != null) settingsContainer.RemoveFromClassList("hidden");
            if (openBtn != null) openBtn.style.display = DisplayStyle.None;
            if (uguiConfigButton != null) uguiConfigButton.gameObject.SetActive(false);

            isOpen = true;
            Time.timeScale = 0f;

            if (AudioManager.inst != null)
                AudioManager.inst.PlayPopupOpen();

            SyncAudioValues();
        }

        public void Close()
        {
            if (outsideDarkener != null) outsideDarkener.AddToClassList("hidden");
            if (settingsContainer != null) settingsContainer.AddToClassList("hidden");
            if (uguiConfigButton != null)
            {
                uguiConfigButton.gameObject.SetActive(true);
            }
            else if (openBtn != null)
            {
                openBtn.style.display = DisplayStyle.Flex;
            }

            isOpen = false;
            Time.timeScale = 1f;

            if (AudioManager.inst != null)
                AudioManager.inst.PlayButtonClick();
        }

        void WireAudioControls()
        {
            if (musicSlider != null)
            {
                musicSlider.RegisterValueChangedCallback(evt =>
                {
                    float val = musicSlider.highValue > 1.5f ? evt.newValue / 100f : evt.newValue;
                    if (AudioManager.inst != null)
                    {
                        if (AudioManager.inst.MusicMuted && val > 0.01f)
                        {
                            AudioManager.inst.SetMusicMuted(false);
                            UpdateMusicIcon(false);
                        }
                        AudioManager.inst.SetMusicVolume(val);
                    }
                });
            }

            if (musicToggleBtn != null)
            {
                musicToggleBtn.clicked += () =>
                {
                    if (AudioManager.inst != null)
                    {
                        bool newMuted = !AudioManager.inst.MusicMuted;
                        AudioManager.inst.SetMusicMuted(newMuted);
                        UpdateMusicIcon(newMuted);
                        AudioManager.inst.PlayButtonClick();
                    }
                };
            }

            if (sfxSlider != null)
            {
                sfxSlider.RegisterValueChangedCallback(evt =>
                {
                    float val = sfxSlider.highValue > 1.5f ? evt.newValue / 100f : evt.newValue;
                    if (AudioManager.inst != null)
                    {
                        if (AudioManager.inst.SfxMuted && val > 0.01f)
                        {
                            AudioManager.inst.SetSfxMuted(false);
                            UpdateSfxIcon(false);
                        }
                        AudioManager.inst.SetSfxVolume(val);
                    }
                });
            }

            if (sfxToggleBtn != null)
            {
                sfxToggleBtn.clicked += () =>
                {
                    if (AudioManager.inst != null)
                    {
                        bool newMuted = !AudioManager.inst.SfxMuted;
                        AudioManager.inst.SetSfxMuted(newMuted);
                        UpdateSfxIcon(newMuted);
                        AudioManager.inst.PlayButtonClick();
                    }
                };
            }

            SyncAudioValues();
        }

        void UpdateMusicIcon(bool isMuted)
        {
            if (musicToggleBtn == null) return;
            if (isMuted)
            {
                musicToggleBtn.RemoveFromClassList("icon-music-on");
                musicToggleBtn.AddToClassList("icon-music-off");
            }
            else
            {
                musicToggleBtn.RemoveFromClassList("icon-music-off");
                musicToggleBtn.AddToClassList("icon-music-on");
            }
#if UNITY_EDITOR
            var icon = isMuted 
                ? (Resources.Load<Sprite>("UI/Icons/icon_music_off") ?? UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>("Assets/_Game/UI/Icons/icon_music_off.png"))
                : (Resources.Load<Sprite>("UI/Icons/icon_music_on") ?? UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>("Assets/_Game/UI/Icons/icon_music_on.png"));
            if (icon != null)
                musicToggleBtn.style.backgroundImage = new StyleBackground(icon);
#endif
        }

        void UpdateSfxIcon(bool isMuted)
        {
            if (sfxToggleBtn == null) return;
            if (isMuted)
            {
                sfxToggleBtn.RemoveFromClassList("icon-sfx-on");
                sfxToggleBtn.AddToClassList("icon-sfx-off");
            }
            else
            {
                sfxToggleBtn.RemoveFromClassList("icon-sfx-off");
                sfxToggleBtn.AddToClassList("icon-sfx-on");
            }
#if UNITY_EDITOR
            var icon = isMuted 
                ? (Resources.Load<Sprite>("UI/Icons/icon_sfx_off") ?? UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>("Assets/_Game/UI/Icons/icon_sfx_off.png"))
                : (Resources.Load<Sprite>("UI/Icons/icon_sfx_on") ?? UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>("Assets/_Game/UI/Icons/icon_sfx_on.png"));
            if (icon != null)
                sfxToggleBtn.style.backgroundImage = new StyleBackground(icon);
#endif
        }

        void SyncAudioValues()
        {
            if (AudioManager.inst == null) return;

            if (musicSlider != null)
            {
                float targetVal = musicSlider.highValue > 1.5f ? AudioManager.inst.MusicVolume * 100f : AudioManager.inst.MusicVolume;
                musicSlider.SetValueWithoutNotify(targetVal);
            }

            UpdateMusicIcon(AudioManager.inst.MusicMuted);

            if (sfxSlider != null)
            {
                float targetVal = sfxSlider.highValue > 1.5f ? AudioManager.inst.SfxVolume * 100f : AudioManager.inst.SfxVolume;
                sfxSlider.SetValueWithoutNotify(targetVal);
            }

            UpdateSfxIcon(AudioManager.inst.SfxMuted);
        }

#if UNITY_EDITOR
        void ApplyVisualStyles(VisualElement root)
        {
            var titleFont = UnityEditor.AssetDatabase.LoadAssetAtPath<Font>("Assets/_Game/MainMenu/Distillery Strong.otf");
            var btnFont = UnityEditor.AssetDatabase.LoadAssetAtPath<Font>("Assets/_Game/Map/CyGrotesk-KeyRegular.otf");
            var buttonBg = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>("Assets/_Game/button_square_line.png");
            var compuBg = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>("Assets/_Game/compu.png");

            var configTitleLbl = root.Q<Label>("lbl-settings-title");
            if (configTitleLbl != null && titleFont != null)
            {
                configTitleLbl.style.unityFont = new StyleFont(titleFont);
                configTitleLbl.style.unityFontDefinition = new StyleFontDefinition(titleFont);
            }

            var titleFrames = root.Query<VisualElement>(className: "title-frame-config").ToList();
            Color cyanColor = new Color(0.801f, 1f, 0.974f, 1f);
            foreach (var tf in titleFrames)
            {
                if (buttonBg != null) tf.style.backgroundImage = new StyleBackground(buttonBg);
                tf.style.unityBackgroundImageTintColor = new StyleColor(cyanColor);
            }

            var compuBezel = root.Q<VisualElement>(className: "compu-bezel");
            if (compuBezel != null && compuBg != null)
            {
                compuBezel.style.backgroundImage = new StyleBackground(compuBg);
            }

            var backBtn = root.Q<Button>("btn-close-settings");
            if (backBtn != null)
            {
                if (btnFont != null)
                {
                    backBtn.style.unityFont = new StyleFont(btnFont);
                    backBtn.style.unityFontDefinition = new StyleFontDefinition(btnFont);
                }
                if (buttonBg != null)
                {
                    backBtn.style.backgroundImage = new StyleBackground(buttonBg);
                }
                backBtn.style.unityBackgroundImageTintColor = new StyleColor(new Color(1f, 0.941f, 0.788f, 1f));
            }

            if (openBtn != null && buttonBg != null)
            {
                openBtn.style.backgroundImage = new StyleBackground(buttonBg);
                openBtn.style.unityBackgroundImageTintColor = new StyleColor(new Color(1f, 0.941f, 0.788f, 1f));
            }

            var labels = root.Query<Label>(className: "settings-label").ToList();
            foreach (var l in labels)
            {
                if (btnFont != null)
                {
                    l.style.unityFont = new StyleFont(btnFont);
                    l.style.unityFontDefinition = new StyleFontDefinition(btnFont);
                }
            }

            var toggles = root.Query<Toggle>(className: "settings-toggle").ToList();
            foreach (var t in toggles)
            {
                if (btnFont != null)
                {
                    t.style.unityFont = new StyleFont(btnFont);
                    t.style.unityFontDefinition = new StyleFontDefinition(btnFont);
                }
            }
        }
#endif
    }
}
