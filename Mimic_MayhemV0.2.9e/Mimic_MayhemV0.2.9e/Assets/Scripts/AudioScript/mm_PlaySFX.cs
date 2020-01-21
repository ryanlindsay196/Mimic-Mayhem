
using UnityEngine;

public class mm_PlaySFX : MonoBehaviour {

    [SerializeField]
    private AudioClip[] sprintClips;
    [SerializeField]
    private AudioClip[] walkClips;
    [SerializeField]
    private AudioClip[] lockClips;
    [SerializeField]
    private AudioClip[] creakClips;
    [SerializeField]
    private AudioClip[] LidSlamClip;
    [SerializeField]
    private AudioClip[] handleClips;

    private AudioSource audioSource;



    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
    
    private void MM_Handle ()
    {
        AudioClip clip = handleClips[UnityEngine.Random.Range(0, handleClips.Length)];
        audioSource.PlayOneShot(clip);
    }

    private void MM_Sprint()
    {
        AudioClip clip = sprintClips[UnityEngine.Random.Range(0, sprintClips.Length)];
        audioSource.PlayOneShot(clip);
    }
    private void MM_Walk()
    {
        //AudioClip clip = GetRandomClip(walkClips);
        AudioClip clip = walkClips[UnityEngine.Random.Range(0, walkClips.Length)];
        audioSource.PlayOneShot(clip);
    }
    private void MM_Lock()
    {
        //AudioClip clip = GetRandomClip(lockClips);
        AudioClip clip = lockClips[UnityEngine.Random.Range(0, lockClips.Length)];
        audioSource.PlayOneShot(clip);
    }
    private void MM_Creak()
    {
        //AudioClip clip = GetRandomClip(creakClips);
        AudioClip clip = creakClips[UnityEngine.Random.Range(0, creakClips.Length)];
        audioSource.PlayOneShot(clip);
    }

    private void MM_LidSlam()
    {
        //AudioClip clip = GetRandomClip(creakClips);
        AudioClip clip = LidSlamClip[UnityEngine.Random.Range(0, LidSlamClip.Length)];
        audioSource.PlayOneShot(clip);
    }

    /*private AudioClip GetRandomClip ()
    {
        return clips[UnityEngine.Random.Range(0, clips.Length)];
    }*/
}
