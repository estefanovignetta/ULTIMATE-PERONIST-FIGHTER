using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Game_Manager : MonoBehaviour {

    public GameObject player;
    private Movement_Manager mm;
    public Transform[] checkpoints;
    public Transform[] spawningPointsWave_01, spawningPointsWave_02, spawningPointsWave_03;
    public GameObject[] enemies;
    public Object[] spawnedEnemies;
    public AudioClip waveComplete;
    private AudioSource auso;
    public Canvas menu;
    public GameObject[] trash_containers;

    private bool wave01_01, wave01_02, wave01_03;
    // Use this for initialization
    void Awake () {
        //Cursor.visible = false;
        Time.timeScale = 1;
        mm = player.GetComponent<Movement_Manager>();
        auso = GetComponent<AudioSource>();
        spawnedEnemies = new Object[3];
        wave01_01 = wave01_02 = wave01_03 = false;
    }
	
	// Update is called once per frame
	void Update ()
    {
        //CHECKING KEYBOARD INPUT
        if (Input.GetKeyDown(KeyCode.E)) Application.Quit();
        if (Input.GetKeyDown(KeyCode.R)) ReloadLevel();
        if (Input.GetKeyDown(KeyCode.Escape)) SwitchMenu();

        if (player == null) return;

        if (!wave01_01)
            if (player.transform.position.x >= checkpoints[0].position.x)
            {
                wave01_01 = true;
                if (mm != null) mm.SendMessage("setLockdown", true);
                trash_containers[0].GetComponent<Animator>().SetTrigger("Open");
                Invoke("StartWave01_01", 1);
            }

        if (!wave01_02)
            if (player.transform.position.x >= checkpoints[1].position.x)
            {
                wave01_02 = true;
                if (mm != null) mm.SendMessage("setLockdown", true);
                trash_containers[1].GetComponent<Animator>().SetTrigger("Open");
                Invoke("StartWave01_02", 1);
            }

        if (!wave01_03)
            if (player.transform.position.x >= checkpoints[2].position.x)
            {
                wave01_03 = true;
                if (mm != null) mm.SendMessage("setLockdown", true);
                Invoke("StartWave01_03", 1);
            }
    }

    public void StartWave01_01()
    {
        spawnedEnemies[0] = Instantiate(enemies[0], spawningPointsWave_01[0].position, Quaternion.identity);
        spawnedEnemies[1] = Instantiate(enemies[0], spawningPointsWave_01[1].position, Quaternion.identity);
        spawnedEnemies[2] = Instantiate(enemies[0], spawningPointsWave_01[2].position, Quaternion.identity);
    }

    public void StartWave01_02()
    {
        Instantiate(enemies[0], spawningPointsWave_02[0].position, Quaternion.identity);
        Instantiate(enemies[0], spawningPointsWave_02[1].position, Quaternion.identity);
        Instantiate(enemies[0], spawningPointsWave_02[2].position, Quaternion.identity);
        Instantiate(enemies[0], spawningPointsWave_02[3].position, Quaternion.identity);
    }

    public void StartWave01_03()
    {
        spawnedEnemies[0] = Instantiate(enemies[0], spawningPointsWave_03[0].position, Quaternion.identity);
        spawnedEnemies[1] = Instantiate(enemies[0], spawningPointsWave_03[1].position, Quaternion.identity);
        spawnedEnemies[2] = Instantiate(enemies[0], spawningPointsWave_03[2].position, Quaternion.identity);
    }

    public void CheckWave01_01()
    {
        //if(spawnedEnemies.Length==0)
        /*if (spawnedEnemies[0] == null && spawnedEnemies[1] == null && spawnedEnemies[2] == null)
        {
            auso.clip = waveComplete;
            auso.Play();
            mm.SendMessage("setLockdown", false);
        }*/
        auso.clip = waveComplete;
        auso.Play();
        if (mm != null) mm.SendMessage("setLockdown", false);
    }

    public void AllEnemiesKilled()
    {
        auso.clip = waveComplete;
        auso.Play();
        if(mm != null) mm.SendMessage("setLockdown", false);
    }

    public void SwitchMenu()
    {
		if (menu == null)
			return;
        if (!menu.enabled)
        {
            menu.enabled = true;
            Time.timeScale = 0;
            Cursor.visible = true;
        }
        else
        {
            menu.enabled = false;
            Time.timeScale = 1;
            Cursor.visible = false;
        }
    }

    public void ReloadLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
