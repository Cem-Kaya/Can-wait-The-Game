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

	private void OnCollisionEnter2D(Collision2D other)
    {
        //Debug.Log("naem :" + collision.tag);
		if(other.gameObject.tag == "Player")
		{
            box_mover my_box_mover;
            my_box_mover = other.gameObject.GetComponent<box_mover>();
            my_box_mover.coin_num++;
            Destroy(gameObject);
		}
	}

}
