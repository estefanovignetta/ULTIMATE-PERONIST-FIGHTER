using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Button_Manager : MonoBehaviour {

    private AudioSource auso;
    public AudioClip sound_highlight, sound_click;
	// Use this for initialization
	void Awake () {
        Cursor.visible = true;
        auso = GetComponent<AudioSource>();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    /*public void OnHighlight(int nmb)
    {
        string name = "Highlight_"+ nmb.ToString();
        Image image = GameObject.Find(name).GetComponent<Image>();
        image.enabled = true;
        PlaySoundHighlight();
    }*/

    public void PlaySoundHighlight()
    {
        auso.clip = sound_highlight;
        auso.Play();
    }

    public void PlaySoundClick()
    {
        auso.clip = sound_click;
        auso.Play();
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void Play()
    {
        //
    }
}