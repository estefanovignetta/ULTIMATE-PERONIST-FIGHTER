using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Button_Manager_game : MonoBehaviour {

    private AudioSource auso;
    public AudioClip sound_highlight, sound_click;
    public Game_Manager gm;
	// Use this for initialization
	void Awake () {
        Cursor.visible = true;
        gm = GameObject.Find("Game Manager").GetComponent<Game_Manager>();
        auso = GetComponent<AudioSource>();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

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
        gm.SendMessage("SwitchMenu");
    }

    public void Restart()
    {
        gm.SendMessage("ReloadLevel");
    }
}