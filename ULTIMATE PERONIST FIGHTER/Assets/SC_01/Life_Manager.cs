using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Life_Manager : MonoBehaviour {

    public int currentHitPoints, maxHitPoints;
    public Image healthBar;
    private Animator anim;
    public Color Healthy, Damaged;

    // Use this for initialization
    void Awake()
    {
        anim = GetComponent<Animator>();
        //healthBar.color = Color.Lerp(Color.red, Color.green, (float)currentHitPoints / maxHitPoints);
        healthBar.color = Color.Lerp(Damaged, Healthy, (float)currentHitPoints / maxHitPoints);
        healthBar.fillAmount = (float)currentHitPoints / maxHitPoints;
    }
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    void ApplyDamage(int dmg)
    {
        currentHitPoints -= dmg;
        healthBar.color = Color.Lerp(Damaged, Healthy, (float)currentHitPoints / maxHitPoints);
        healthBar.fillAmount= (float) currentHitPoints / maxHitPoints;
        anim.SetTrigger("Hit");
    }

    void CheckLife()
    {
        if (currentHitPoints > 0) return;

        transform.Translate(new Vector3(0, 0, 1000));
		Destroy(gameObject,1.0f);
    }
}
