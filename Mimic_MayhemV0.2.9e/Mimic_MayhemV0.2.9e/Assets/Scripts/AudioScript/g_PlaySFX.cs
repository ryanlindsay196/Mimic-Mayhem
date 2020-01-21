using UnityEngine.Audio;
using UnityEngine;

public class g_PlaySFX : MonoBehaviour
{
        
    [SerializeField]
    private AudioClip[] walkClips;
    [SerializeField]
    private AudioClip[] swordSwing;
    [SerializeField]
    private AudioClip[] swordScrape;
    [SerializeField]
    private AudioClip[] g_SuspiciousCLips;
    [SerializeField]
    private AudioClip[] g_AlertClips;

    private AudioSource audioSource;



    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
      
    private void G_Walk()
    {
        //AudioClip clip = GetRandomClip(walkClips);
        AudioClip clip = walkClips[UnityEngine.Random.Range(0, walkClips.Length)];
        audioSource.PlayOneShot(clip, 0.4f);
    }
    private void G_SwordSwing()
    {
        //AudioClip clip = GetRandomClip(lockClips);
        AudioClip clip = swordSwing[UnityEngine.Random.Range(0, swordSwing.Length)];
        audioSource.PlayOneShot(clip, 1f);
    }
    private void G_SwordScrape()
    {
        //AudioClip clip = GetRandomClip(creakClips);
        AudioClip clip = swordScrape[UnityEngine.Random.Range(0, swordScrape.Length)];
        audioSource.PlayOneShot(clip, 1f);
    }

    public void G_Suspicious()
    {
        //AudioClip clip = GetRandomClip(creakClips);
        AudioClip clip = g_SuspiciousCLips[UnityEngine.Random.Range(0, g_SuspiciousCLips.Length)];
        audioSource.PlayOneShot(clip, 1f);
    }

    public void G_Alert()
    {
        //AudioClip clip = GetRandomClip(creakClips);
        AudioClip clip = g_AlertClips[UnityEngine.Random.Range(0, g_AlertClips.Length)];
        audioSource.PlayOneShot(clip, 1f);
    }

    /*private AudioClip GetRandomClip ()
    {
        return clips[UnityEngine.Random.Range(0, clips.Length)];
    }*/
}
