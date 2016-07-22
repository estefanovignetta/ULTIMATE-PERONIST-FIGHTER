using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Scene_Manager : MonoBehaviour {

    private Fading fd;

    void Awake()
    {
        fd = GetComponent<Fading>();
    }
	// Use this for initialization
	void Start () {
        if(SceneManager.GetActiveScene().name == "Splash_Screen") StartCoroutine(Delay_SplashScreen());
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void BeginGame()
    {
        StartCoroutine(Delay_MainMenu());
    }
    public void BeginScene01()
    {
        StartCoroutine(Delay_Intro());
    }

    IEnumerator Delay_SplashScreen()
    {
        yield return new WaitForSeconds(3);
        float fadetime = fd.BeginFade(1);
        yield return new WaitForSeconds(1+fadetime);
        SceneManager.LoadScene("Main_Menu");
    }
    IEnumerator Delay_MainMenu()
    {
        yield return new WaitForSeconds(1);
        float fadetime = fd.BeginFade(1);
        yield return new WaitForSeconds(1 + fadetime);
        SceneManager.LoadScene("Intro");
    }
    IEnumerator Delay_Intro()
    {
        //yield return new WaitForSeconds(2);
        float fadetime = fd.BeginFade(1);
        yield return new WaitForSeconds(fadetime);
        GameObject.Find("Loading Screen").GetComponent<Canvas>().enabled = true;
        GameObject.Find("Canvas").GetComponent<Canvas>().enabled = false;
        fd.BeginFade(-1);
        yield return new WaitForSeconds(1+fadetime);
        fd.BeginFade(1);
        yield return new WaitForSeconds(fadetime);
        SceneManager.LoadScene("SC_01");
    }
}
