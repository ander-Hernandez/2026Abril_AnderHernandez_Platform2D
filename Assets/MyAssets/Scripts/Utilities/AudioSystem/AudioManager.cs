using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Music")]
    [SerializeField] private AudioClip menuMusic;
    [SerializeField] private AudioClip gameplayMusic;

    [Header("Player Sounds")]
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip dashSound;
    [SerializeField] private AudioClip attackSound;
    [SerializeField] private AudioClip damageSound;
    [SerializeField] private AudioClip deathSound;

    [Header("Collectable Sounds")]
    [SerializeField] private AudioClip coinSound;

    [Header("Enemy Sounds")]
    [SerializeField] private AudioClip enemyDamageSound;
    [SerializeField] private AudioClip enemyDeathSound;
    [SerializeField] private AudioClip rootAttackSound;

    [Header("Volume")]
    [Range(0f, 1f)]
    [SerializeField] private float musicVolume = 0.5f;

    [Range(0f, 1f)]
    [SerializeField] private float sfxVolume = 1f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        SetupAudioSources();
    }

    private void Start()
    {
        PlayMusic(menuMusic);
    }

    private void SetupAudioSources()
    {
        if (musicSource == null)
            musicSource = gameObject.AddComponent<AudioSource>();

        if (sfxSource == null)
            sfxSource = gameObject.AddComponent<AudioSource>();

        musicSource.loop = true;
        musicSource.playOnAwake = false;
        musicSource.volume = musicVolume;

        sfxSource.loop = false;
        sfxSource.playOnAwake = false;
        sfxSource.volume = sfxVolume;
    }

    public void PlayGameplayMusic()
    {
        PlayMusic(gameplayMusic);
    }

    public void PlayMusic(AudioClip musicClip)
    {
        if (musicClip == null)
            return;

        if (musicSource.clip == musicClip && musicSource.isPlaying)
            return;

        musicSource.clip = musicClip;
        musicSource.volume = musicVolume;
        musicSource.Play();
    }

    public void StopMusic()
    {
        if (musicSource == null)
            return;

        musicSource.Stop();
    }

    public void PlayJumpSound()
    {
        PlaySfx(jumpSound);
    }

    public void PlayDashSound()
    {
        PlaySfx(dashSound);
    }

    public void PlayAttackSound()
    {
        PlaySfx(attackSound);
    }

    public void PlayDamageSound()
    {
        PlaySfx(damageSound);
    }

    public void PlayDeathSound()
    {
        PlaySfx(deathSound);
    }

    public void PlayCoinSound()
    {
        PlaySfx(coinSound);
    }

    public void PlayEnemyDamageSound()
    {
        PlaySfx(enemyDamageSound);
    }

    public void PlayEnemyDeathSound()
    {
        PlaySfx(enemyDeathSound);
    }

    public void PlayRootAttackSound()
    {
        PlaySfx(rootAttackSound);
    }

    private void PlaySfx(AudioClip sfxClip)
    {
        if (sfxClip == null)
            return;

        if (sfxSource == null)
            return;

        sfxSource.PlayOneShot(sfxClip, sfxVolume);
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);

        if (musicSource != null)
            musicSource.volume = musicVolume;
    }

    public void SetSfxVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);

        if (sfxSource != null)
            sfxSource.volume = sfxVolume;
    }
}