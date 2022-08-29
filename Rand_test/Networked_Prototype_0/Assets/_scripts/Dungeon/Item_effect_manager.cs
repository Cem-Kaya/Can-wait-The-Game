using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
	
public class Item_effect_manager : NetworkBehaviour
{



	// Start is called before the first frame update
	void Start()
	{
		
	}

	// Update is called once per frame
	void Update()
	{
		
	}
	
	void OnTriggerEnter2D(Collider2D hitObject)
	{		
		if (hitObject.gameObject.layer == 3)
		{
			hitObject.gameObject.GetComponent<box_mover>().fdelay = 20; 

		}
		
	}


}
