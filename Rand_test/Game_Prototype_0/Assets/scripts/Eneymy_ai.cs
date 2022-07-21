using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum Ai_state
{
	idle,
	walk,
	attack,
	die
}


public class Eneymy_ai : MonoBehaviour
{
    public Ai_state state = Ai_state.walk;
    public float vision_range = 10 ;
	public float speed = 3 ;
    public GameObject player;
	public float health = 100.0f;

	private Quaternion look_dir; 
	private Vector2 walking_direction;
	private Rigidbody2D rb;
	// Start is called before the first frame update
	private void Awake()
	{
		
	}
	void Start()
    {
		rb = GetComponent<Rigidbody2D>();
		StartCoroutine(random_walk());
		player = GameObject.FindGameObjectWithTag("Player");
	}

	// Update is called once per frame
	void Update()
    {
        
    }
	private void FixedUpdate()
	{	
		if (close_enough() && state != Ai_state.attack && state != Ai_state.die )
		{
			state = Ai_state.attack;
		}
		else if (state != Ai_state.die)
		{
			state = Ai_state.walk;
		}
		switch (state)
		{
			case Ai_state.idle:				
				break;
			case Ai_state.walk:
				walk();
				break;
			case Ai_state.attack:
				attack();
				break;
			case Ai_state.die:
				die();
				break;
		}
	}
	

	private IEnumerator random_walk()
	{
		while (true)
		{
			yield return new WaitForSeconds(Random.Range(2, 3));
			if (state == Ai_state.walk)
			{
				
				walking_direction = new Vector2(Random.Range(-1, 1), Random.Range(-1, 1));
				walking_direction.Normalize();
			}
			
		}
	}

	private IEnumerator interp_look()
	{
		while (true)
		{
			for (int i = 0; i< 100 ;i++) { 
				//look_dir= Vector3.Lerp(startMarker.position, endMarker.position, i){
				yield return null;

			}


		}
	}
	private void walk()
	{
		transform.rotation =  Quaternion.LookRotation(Vector3.forward, walking_direction);
		rb.velocity = Vector2.ClampMagnitude(walking_direction * speed, speed);
	}
	
	private void attack()
	{
		walking_direction = player.transform.position - transform.position * 2;		
		rb.velocity = Vector2.ClampMagnitude(walking_direction * speed , speed)  ;
		
	}
	private void die()
	{
		Destroy(gameObject);
	}
	private bool close_enough()
	{
		for(int i = 0; i< 36; i++)
		{
			Vector2 pos = new Vector2(transform.position.x, transform.position.y );
						
			Debug.DrawLine(pos + new Vector2( Mathf.Sin(2f * Mathf.PI / 36.0f * i), Mathf.Cos(2f * Mathf.PI / 36.0f * i)) * vision_range , pos + new Vector2( Mathf.Sin(2f * Mathf.PI / 36.0f * i+1 ) , Mathf.Cos(2f * Mathf.PI / 36.0f * i + 1) ) * vision_range, Color.black , 0.5f );
		}
		return Vector2.Distance(transform.position, player.transform.position) < vision_range;
	}

	
	private void OnCollisionEnter2D(Collision2D other)
	{		
		//Debug.Log("naem :" + other.gameObject.tag);
		if (other.gameObject.tag == "Wall")
		{
			if (state == Ai_state.walk)
			{
				walking_direction *= -1 ;
			}
		}
		else if (other.gameObject.tag == "Bullet")
		{
			health -= other.gameObject.GetComponent<Bullet_controller>().damage;
			//Debug.Log("health :" + health);
			if (health <= 0)
			{
				state = Ai_state.die;
			}
		}		
	}
}
