using UnityEngine;
using System.Collections;

public class Sound_Manager : MonoBehaviour {

    //public AudioClip _punch01, _punch02, _throw;
    //private AudioSource auso;
    private AudioSource[] auso;
    private int nmb_touse;

    // Use this for initialization
    void Awake () {
        auso = GetComponents<AudioSource>();
        nmb_touse = 0;
    }
	
	// Update is called once per frame
	void Update ()
    {
        //print(auso.Length);
	}

    public void PlayOutLoud(AudioClip clip)
    {
        /*auso[0].clip = clip;
        auso[0].Play();*/
        /*for (int i = 0; i < auso.Length; i++)
        {
            if (!auso[i].isPlaying)
            {
                auso[i].clip = clip;
                auso[i].Play();
                return;
            }
        }*/
        auso[nmb_touse].clip = clip;
        auso[nmb_touse].Play();
        nmb_touse++;
        if (nmb_touse >= auso.Length) nmb_touse = 0;
    }
}
