using UnityEngine;

namespace Hackathon.Audio
{
    public class AudioManager : Universal.Singleton<AudioManager>
    {
        internal override bool DoNotDestroyOnLoad => true;

        const string KeyMusicVol = "AUDIO_MUSIC_VOL";
        const string KeyMusicMuted = "AUDIO_MUSIC_MUTED";
        const string KeySfxVol = "AUDIO_SFX_VOL";
        const string KeySfxMuted = "AUDIO_SFX_MUTED";

        [Header("SFX Player")]
        [SerializeField] AudioSource sfxSource;

        [Header("Default UI Sounds")]
        [SerializeField] AudioClip defaultButtonClick;
        [SerializeField] AudioClip defaultButtonHover;
        [SerializeField] AudioClip defaultPopupOpen;

        float musicVolume = 1f;
        bool musicMuted = false;
        float sfxVolume = 1f;
        bool sfxMuted = false;

        public float MusicVolume => musicVolume;
        public bool MusicMuted => musicMuted;
        public float SfxVolume => sfxVolume;
        public bool SfxMuted => sfxMuted;

        public System.Action OnAudioSettingsChanged;

        internal override void Awake()
        {
            base.Awake();
            if (inst != this) return;

            LoadSettings();
            SetupSfxSource();
        }

        void Start()
        {
            ApplyMusicSettings();
        }

        void SetupSfxSource()
        {
            if (sfxSource != null)
            {
                sfxSource.playOnAwake = false;
                sfxSource.loop = false;
            }

            // Load default clips if missing
            if (defaultButtonClick == null)
                defaultButtonClick = Resources.Load<AudioClip>("Audio/SFX_ButtonClick") ?? Resources.Load<AudioClip>("SFX_ButtonClick");
            if (defaultButtonHover == null)
                defaultButtonHover = Resources.Load<AudioClip>("Audio/SFX_ButtonHover") ?? Resources.Load<AudioClip>("SFX_ButtonHover");
            if (defaultPopupOpen == null)
                defaultPopupOpen = Resources.Load<AudioClip>("Audio/SFX_PopupOpen") ?? Resources.Load<AudioClip>("SFX_PopupOpen");
#if UNITY_EDITOR
            if (defaultButtonClick == null)
                defaultButtonClick = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/_Game/Audio/SFX_ButtonClick.wav");
            if (defaultButtonHover == null)
                defaultButtonHover = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/_Game/Audio/SFX_ButtonHover.wav");
            if (defaultPopupOpen == null)
                defaultPopupOpen = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/_Game/Audio/SFX_PopupOpen.wav");
#endif
        }

        void LoadSettings()
        {
            musicVolume = PlayerPrefs.GetFloat(KeyMusicVol, 1f);
            musicMuted = PlayerPrefs.GetInt(KeyMusicMuted, 0) == 1;
            sfxVolume = PlayerPrefs.GetFloat(KeySfxVol, 1f);
            sfxMuted = PlayerPrefs.GetInt(KeySfxMuted, 0) == 1;
        }

        void SaveSettings()
        {
            PlayerPrefs.SetFloat(KeyMusicVol, musicVolume);
            PlayerPrefs.SetInt(KeyMusicMuted, musicMuted ? 1 : 0);
            PlayerPrefs.SetFloat(KeySfxVol, sfxVolume);
            PlayerPrefs.SetInt(KeySfxMuted, sfxMuted ? 1 : 0);
            PlayerPrefs.Save();
        }

        public void SetMusicVolume(float volume)
        {
            musicVolume = Mathf.Clamp01(volume);
            SaveSettings();
            ApplyMusicSettings();
            OnAudioSettingsChanged?.Invoke();
        }

        public void SetMusicMuted(bool muted)
        {
            musicMuted = muted;
            SaveSettings();
            ApplyMusicSettings();
            OnAudioSettingsChanged?.Invoke();
        }

        public void SetSfxVolume(float volume)
        {
            sfxVolume = Mathf.Clamp01(volume);
            SaveSettings();
            OnAudioSettingsChanged?.Invoke();
        }

        public void SetSfxMuted(bool muted)
        {
            sfxMuted = muted;
            SaveSettings();
            OnAudioSettingsChanged?.Invoke();
        }

        void ApplyMusicSettings()
        {
            MusicManager mm = MusicManager.inst;
            if (mm != null)
            {
                mm.SetVolume(musicVolume);
                mm.SetMuted(musicMuted);
            }
        }

        public void PlaySFX(AudioClip clip, float volumeScale = 1f)
        {
            if (sfxMuted || clip == null) return;
            if (sfxSource == null) SetupSfxSource();

            float finalVol = Mathf.Clamp01(sfxVolume * volumeScale);
            if (finalVol > 0.001f)
            {
                sfxSource.PlayOneShot(clip, finalVol);
            }
        }

        public void PlayButtonClick()
        {
            if (defaultButtonClick != null)
                PlaySFX(defaultButtonClick, 0.85f);
        }

        public void PlayButtonHover()
        {
            if (defaultButtonHover != null)
                PlaySFX(defaultButtonHover, 0.4f);
        }

        public void PlayPopupOpen()
        {
            if (defaultPopupOpen != null)
                PlaySFX(defaultPopupOpen, 0.8f);
        }
    }
}
