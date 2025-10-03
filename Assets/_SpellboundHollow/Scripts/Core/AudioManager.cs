using UnityEngine;

namespace _SpellboundHollow.Scripts.Core
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        private AudioSource _musicSource;
        private AudioSource _sfxSource;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            // DontDestroyOnLoad(gameObject);

            AudioSource[] sources = GetComponents<AudioSource>();
            _musicSource = sources[0]; // Первый источник - для музыки
            _sfxSource = sources[1];   // Второй - для SFX
        }

        public void PlayMusic(AudioClip clip)
        {
            // TODO: Добавить плавный переход (crossfade) в будущем
            _musicSource.clip = clip;
            _musicSource.Play();
        }

        public void PlaySFX(AudioClip clip)
        {
            if (clip != null)
            {
                _sfxSource.PlayOneShot(clip);
            }
        }
    }
}