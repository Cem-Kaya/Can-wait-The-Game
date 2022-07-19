using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Colection_script : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	private void OnTriggerEnter2D(Collider2D collision)
	{
        //Debug.Log("naem :" + collision.tag);
		if(collision.tag == "Player")
		{
            box_mover.coin_num++;
            Destroy(gameObject);
		}
	}

}
