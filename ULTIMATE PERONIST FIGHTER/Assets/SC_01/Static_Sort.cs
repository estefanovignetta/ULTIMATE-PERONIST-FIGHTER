using UnityEngine;
using System.Collections;

public class Static_Sort : MonoBehaviour {

	// Use this for initialization
	void Awake () {
        //sortStatic();
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.y+5);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void sortStatic()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        string[] sortlayernames = {"forefront_20", "forefront_19", "forefront_18", "forefront_17", "forefront_16",
                                  "forefront_15", "forefront_14", "forefront_13", "forefront_12", "forefront_11",
                                  "forefront_10", "forefront_09", "forefront_08", "forefront_07",
                                  "forefront_06", "forefront_05", "forefront_04", "forefront_03",
                                  "forefront_02", "forefront_01"};
        float inpoint = -10f, endpoint = 10f, step=0.5f;
        for (int i = 0; i < endpoint / step; i++)
        {
            if (transform.position.y >= inpoint + step * i && transform.position.y < inpoint + step * (i + 1)) { }
                sr.sortingLayerName = sortlayernames[i];
        }
    }
}
