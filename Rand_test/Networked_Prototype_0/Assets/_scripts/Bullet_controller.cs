using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_controller : MonoBehaviour
{
	public float lifetime= 3 ;
	public int max_bounce = 5 ;
	public float damage = 1;

	
	// Start is called before the first frame update
	void Start()
	{
		Debug.Log("start of bullet  ");
		StartCoroutine(death_delay() );   
	}

	// Update is called once per frame
	void Update()
	{
		
	}
	private IEnumerator death_delay()
	{
		float start = Time.time ;
		while (true)
		{
			yield return new WaitForEndOfFrame() ;
			if (start + lifetime < Time.time)
				Destroy(gameObject);
		}
		      
	}
	  
	
	private void OnCollisionEnter2D(Collision2D other)
	{
		//Debug.Log("naem :" + collision.tag);
		if (other.gameObject.tag != "Bullet")
		{
			if (--max_bounce == 0)
			{
				Destroy(gameObject);
			}
		}
	}
	public void set_bullet( float lf, int bounce, float dmg, float scale)
	{
		lifetime += lf;
		max_bounce += bounce;
		damage += dmg;
		transform.localScale *= scale;
	}

}
