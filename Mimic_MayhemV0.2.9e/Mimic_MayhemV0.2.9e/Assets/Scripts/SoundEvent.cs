using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEvent : MonoBehaviour
{
    List<bool> heardBy;//whether each npc has heard this sound. This way, they only hear each sound once
    public enum SoundType
    {
        None,
        SprintBounce,
        Transforming,
        EatItem,
        EatNPC 
    } //might consider EatVillager and EatGuard separately
    public SoundType thisSoundType = SoundType.None;

    //soundRadius will be decided by the type of event being called, and current will be constantly changing to meet that value.
    public float soundRadius = 1.0f;
    float currentSoundRadius;
    
	// Use this for initialization
	void Start ()
    {
        currentSoundRadius = 0.0f;

        heardBy = new List<bool>();
        for (int i = 0; i < FindObjectsOfType<Listener>().Length; i++)
            heardBy.Add(false);
	}
	
	// Update is called once per frame
	void Update ()
    {
        const float SOUND_RADIUS_INCREASE_RATE = 2.5f;

        currentSoundRadius += Time.deltaTime * SOUND_RADIUS_INCREASE_RATE * Mathf.Sin(Mathf.PI * (currentSoundRadius / soundRadius) / 2) + 0.1f;

        transform.localScale = new Vector3(currentSoundRadius, 0.1f, currentSoundRadius);//only increasing on X and Z axis

        Listener[] listeners = FindObjectsOfType<Listener>();

        int listenerIndex = 0;
        foreach(Listener listener in listeners)
        {
            Vector3 soundToListener = listener.transform.position - transform.position;
            if(soundToListener.magnitude < currentSoundRadius + listener.ListeningRadius && !heardBy[listenerIndex])
            {
                heardBy[listenerIndex] = true;
                listener.OnHeardSound(thisSoundType, transform.position);
            }
            listenerIndex++;
        }


        if (currentSoundRadius >= soundRadius)
        {
            Destroy(gameObject);
        }
	}
}
