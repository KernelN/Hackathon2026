using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Hackathon.Audio;

namespace Hackathon.UI
{
    [RequireComponent(typeof(UIDocument))]
    public class MainMenuController : MonoBehaviour
    {
        [SerializeField] string playSceneName = "_Tutorials";
        [SerializeField] string creditsSceneName = "_Credits";

        [SerializeField] UIDocument uiDoc;
        VisualElement pageMain;
        VisualElement pageSettings;

        Slider musicSlider;
        Button musicToggleBtn;
        Slider sfxSlider;
        Button sfxToggleBtn;

        static void DeactivateLegacyMenu(Scene scene)
        {
            foreach (var root in scene.GetRootGameObjects())
            {
                if (root.name == "Canvas")
                {
                    for (int i = 0; i < root.transform.childCount; i++)
                    {
                        var child = root.transform.GetChild(i);
                        if (child.name.Contains("Button") || child.name == "Title" || child.name == "Image")
                        {
                            child.gameObject.SetActive(false);
                        }
                        else if (child.name == "Map Panel")
                        {
                            // Keep Map Panel active so the computer monitor bezel and desk background remain intact.
                            // Only deactivate the legacy title and button children inside it.
                            DeactivateLegacyChildren(child);
                        }
                    }
                }
                else if (root.name == "Title" || root.name == "Image" || root.name.Contains("Button"))
                {
                    root.SetActive(false);
                }
            }
        }

        static void DeactivateLegacyChildren(Transform parent)
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                var child = parent.GetChild(i);
                if (child.name.Contains("Button") || child.name == "Title" || child.name == "Image")
                {
                    child.gameObject.SetActive(false);
                }
                else
                {
                    DeactivateLegacyChildren(child);
                }
            }
        }

        void Awake()
        {
            DeactivateLegacyMenu(gameObject.scene);
        }

        void OnEnable()
        {
            if (uiDoc == null || uiDoc.rootVisualElement == null) return;
            var root = uiDoc.rootVisualElement;

            pageMain = root.Q<VisualElement>("page-main");
            pageSettings = root.Q<VisualElement>("page-settings");

            // Ensure Main page is visible and Settings page is hidden initially
            if (pageMain != null) pageMain.RemoveFromClassList("hidden");
            if (pageSettings != null) pageSettings.AddToClassList("hidden");

#if UNITY_EDITOR
            ApplyVisualStyles(root);
#endif

            // Main menu buttons
            var playBtn = root.Q<Button>("btn-play") ?? root.Q<Button>("play-btn");
            var settingsBtn = root.Q<Button>("btn-settings") ?? root.Q<Button>("settings-btn");
            var creditsBtn = root.Q<Button>("btn-credits") ?? root.Q<Button>("credits-btn");
            var exitBtn = root.Q<Button>("btn-exit") ?? root.Q<Button>("exit-btn");
            var backBtn = root.Q<Button>("btn-back-settings");

            // Settings controls
            musicSlider = root.Q<Slider>("slider-music-volume") ?? root.Q<Slider>("music-slider");
            musicToggleBtn = root.Q<Button>("btn-toggle-music");
            sfxSlider = root.Q<Slider>("slider-sfx-volume") ?? root.Q<Slider>("sfx-slider");
            sfxToggleBtn = root.Q<Button>("btn-toggle-sfx");

            WireSettingsControls();

            if (AudioManager.inst != null)
                AudioManager.inst.OnAudioSettingsChanged += SyncSettingsValues;

            // Hover sound on all menu buttons
            var menuButtons = root.Query<Button>(className: "menu-btn").ToList();
            foreach (var b in menuButtons)
            {
                b.RegisterCallback<PointerEnterEvent>(_ =>
                {
                    if (AudioManager.inst != null)
                        AudioManager.inst.PlayButtonHover();
                });
            }

            if (playBtn != null)
            {
                playBtn.clicked += () =>
                {
                    if (AudioManager.inst != null) AudioManager.inst.PlayButtonClick();
                    if (Time.timeScale == 0) Time.timeScale = 1;
                    SceneManager.LoadSceneAsync(playSceneName);
                };
            }

            if (settingsBtn != null)
            {
                settingsBtn.clicked += () =>
                {
                    if (AudioManager.inst != null) AudioManager.inst.PlayButtonClick();
                    if (pageMain != null) pageMain.AddToClassList("hidden");
                    if (pageSettings != null) pageSettings.RemoveFromClassList("hidden");
                    SyncSettingsValues();
                };
            }

            if (backBtn != null)
            {
                backBtn.clicked += () =>
                {
                    if (AudioManager.inst != null) AudioManager.inst.PlayButtonClick();
                    if (pageSettings != null) pageSettings.AddToClassList("hidden");
                    if (pageMain != null) pageMain.RemoveFromClassList("hidden");
                };
            }

            if (creditsBtn != null)
            {
                creditsBtn.clicked += () =>
                {
                    if (AudioManager.inst != null) AudioManager.inst.PlayButtonClick();
                    if (Time.timeScale == 0) Time.timeScale = 1;
                    SceneManager.LoadSceneAsync(creditsSceneName);
                };
            }

            if (exitBtn != null)
            {
                exitBtn.clicked += () =>
                {
                    if (AudioManager.inst != null) AudioManager.inst.PlayButtonClick();
                    Application.Quit();
#if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
#endif
                };
            }
        }

        void OnDisable()
        {
            if (AudioManager.inst != null)
                AudioManager.inst.OnAudioSettingsChanged -= SyncSettingsValues;
        }

        void WireSettingsControls()
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

            SyncSettingsValues();
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

        void SyncSettingsValues()
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

            var titleLbl = root.Q<Label>("lbl-title");
            if (titleLbl != null && titleFont != null)
            {
                titleLbl.style.unityFont = new StyleFont(titleFont);
                titleLbl.style.unityFontDefinition = new StyleFontDefinition(titleFont);
            }

            var configTitleLbl = root.Q<Label>("lbl-settings-title");
            if (configTitleLbl != null && titleFont != null)
            {
                configTitleLbl.style.unityFont = new StyleFont(titleFont);
                configTitleLbl.style.unityFontDefinition = new StyleFontDefinition(titleFont);
            }

            var titleFrames = root.Query<VisualElement>(className: "title-frame").ToList();
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

            var menuButtons = root.Query<Button>(className: "menu-btn").ToList();
            Color buttonColor = new Color(1f, 0.941f, 0.788f, 1f);
            foreach (var b in menuButtons)
            {
                if (btnFont != null)
                {
                    b.style.unityFont = new StyleFont(btnFont);
                    b.style.unityFontDefinition = new StyleFontDefinition(btnFont);
                }
                if (buttonBg != null)
                {
                    b.style.backgroundImage = new StyleBackground(buttonBg);
                }
                b.style.unityBackgroundImageTintColor = new StyleColor(buttonColor);
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
