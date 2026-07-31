using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager instance;

    [Header("Combat SFX")]
    [SerializeField] private AudioClip fireSFX;
    [SerializeField] private AudioClip ricochetSFX;
    [SerializeField] private AudioClip hitSFX;
    [SerializeField] private AudioClip destroyedSFX;

    [Header("Volume")]
    [SerializeField][Range(0f, 1f)] private float fireVolume = 0.5f;
    [SerializeField][Range(0f, 1f)] private float ricochetVolume = 0.5f;
    [SerializeField][Range(0f, 1f)] private float hitVolume = 0.5f;
    [SerializeField][Range(0f, 1f)] private float destroyedVolume = 0.5f;

    public float FireVolume => fireVolume;
    public float RicochetVolume => ricochetVolume;
    public float HitVolume => hitVolume;
    public float DestroyedVolume => destroyedVolume;
    

    [Header("Output Limit")]
    [SerializeField][Range(-20f, 0f)] private float maxOutputDb = -3f;

    private AudioSource audioSource;
    private float outputLimit;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = false;

        fireVolume = PlayerPrefs.GetFloat("FireVolume", fireVolume);
        ricochetVolume = PlayerPrefs.GetFloat("RicochetVolume", ricochetVolume);
        hitVolume = PlayerPrefs.GetFloat("HitVolume", hitVolume);
        destroyedVolume = PlayerPrefs.GetFloat("DestroyedVolume", destroyedVolume);

        outputLimit = Mathf.Pow(10f, maxOutputDb / 20f);
    }

    private void PlaySound(AudioClip clip, float clipVolume)
    {
        if (clip == null)
        {
            return;
        }
        audioSource.PlayOneShot(clip, clipVolume);
    }

    public void PlayFire()
    {
        PlaySound(fireSFX,fireVolume);
    }

    public void PlayRicochet()
    {
        PlaySound(ricochetSFX,ricochetVolume);

    }

    public void PlayHit()
    {
        PlaySound(hitSFX,hitVolume);

    }

    public void PlayDestroyed()
    {
        PlaySound(destroyedSFX,destroyedVolume);
    }

    public void SetFireVolume(float value)
    {
        fireVolume = Mathf.Clamp01(value);
        PlayerPrefs.SetFloat("FireVolume", fireVolume);
    }

    public void SetRicochetVolume(float value)
    {
        ricochetVolume = Mathf.Clamp01(value);
        PlayerPrefs.SetFloat("RicochetVolume", ricochetVolume);
    }

    public void SetHitVolume(float value)
    {
        hitVolume = Mathf.Clamp01(value);
        PlayerPrefs.SetFloat("HitVolume", hitVolume);
    }

    public void SetDestroyedVolume(float value)
    {
        destroyedVolume = Mathf.Clamp01(value);
        PlayerPrefs.SetFloat("DestroyedVolume", destroyedVolume);
    }

    // 오디오 필터
    private void OnAudioFilterRead(float[] data, int channels)
    {
        for(int i=0; i<data.Length; i++)
        {
            data[i] = Mathf.Clamp(data[i],-outputLimit,outputLimit);
        }
    }



}
