using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class Text_Manager : MonoBehaviour
{
    private Text myText;
    private AudioSource auso;
    public float letterPause = 0.1f;
    private string message;
    public GameObject sm;

    void Awake ()
    {
        Cursor.visible = false;
        myText = GetComponent<Text>();
        auso = GetComponent<AudioSource>();
        message = myText.text;
        myText.text = "";
        //GameObject.Find("Loading Screen").GetComponent<Canvas>().enabled = true;
    }

    void Start()
    {
        StartCoroutine(TypeText());
        auso.Play();
    }

    IEnumerator TypeText()
    {
        foreach (char letter in message.ToCharArray())
        {
            myText.text += letter;
            yield return new WaitForSeconds(letterPause);
        }
        yield return new WaitForSeconds(8);
        sm.GetComponent<Scene_Manager>().SendMessage("BeginScene01");
    }
} 