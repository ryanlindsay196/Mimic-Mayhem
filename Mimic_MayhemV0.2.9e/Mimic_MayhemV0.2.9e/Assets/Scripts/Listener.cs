using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Listener : MonoBehaviour
{

    public float ListeningRadius = 1.0f;
    public bool inEarShot = false;
    Animator mimicAnim;
    GameObject mimic;

    public Vector3 lastSoundHeardPosition;

    // Use this for initialization
	void Start ()
    {
        //mimic = GameObject.FindGameObjectWithTag("Player");
        //mimicAnim = mimic.GetComponent<Animator>();	
	}
	
	// Update is called once per frame
	void Update ()
    {
        //inEarShot = false;
	}

    public void OnHeardSound(SoundEvent.SoundType soundType, Vector3 soundPosition)
    {
        inEarShot = true;
        lastSoundHeardPosition = soundPosition;
        //Debug.Log("I HEARD A " + soundType.ToString());
    }
}
