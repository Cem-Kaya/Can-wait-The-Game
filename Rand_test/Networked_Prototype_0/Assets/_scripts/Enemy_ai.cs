using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.Netcode;

public enum Ai_state
{
	idle,
	walk,
	attack,
	die
}


public class Enemy_ai : NetworkBehaviour
{
	public GameObject amiba;
	public Ai_state state = Ai_state.walk;
	public float vision_range = 5;
	public float speed = 3;
	public Vector3 player_position;

	public NetworkVariable<float> health = new NetworkVariable<float>();
	//network variable yaptim ya networkobject component ini koyunca boylece oldu.


	private Quaternion look_dir;
	private Vector2 walking_direction;
	private Rigidbody2D rb;
	private bool once;

	// event for when the ai dies
	//
	public delegate void  on_death_event();
	public static event on_death_event on_death;
	

	// Start is called before the first frame update
	private void Awake()
	{
		state = Ai_state.walk;
		once = false;
		player_position = new Vector3(-999, -999, -999);
	}
	

	
	

	void Start()
	{
		//may need to destroy the script in the future keep this in mind
		rb = GetComponent<Rigidbody2D>();
		StartCoroutine(random_walk());

		if(IsServer) health.Value = 25f;
	}

	// Update is called once per frame
	void Update()
	{

	}
	private void FixedUpdate()
	{
		if (close_enough() && state != Ai_state.attack && state != Ai_state.die)
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

	private void walk()
	{
		//transform.rotation = look_dir;//Quaternion.LookRotation(Vector3.forward, walking_direction);
		rb.velocity = Vector2.ClampMagnitude(walking_direction * speed + new Vector2(0.5f, 0.5f), speed);
	}

	private void attack()
	{
		walking_direction = player_position - transform.position;
		walking_direction.Normalize();
		rb.velocity = Vector2.ClampMagnitude(walking_direction * speed, speed);

	}

	private void die()
	{
		if (IsServer) die_ServerRpc();
	}

	[ServerRpc]
	private void die_ServerRpc()
	{
		if (once == false)
		{
			once = true;
			if (!(transform.localScale.magnitude < (0.9)) && IsServer) // 3.50 is the defult magnatude 
			{
				//Debug.Log(GetInstanceID());

			}
		}
		StartCoroutine(delayed_death());

	}

	private IEnumerator delayed_death()
	{
		on_death?.Invoke();  // event trigerd 
		yield return new WaitForFixedUpdate();		
		gameObject.GetComponent<NetworkObject>().Despawn();
		//estroy(gameObject);		
	}


	private bool close_enough()
	{
		for (int i = 0; i < 36; i++)
		{
			Vector2 pos = new Vector2(transform.position.x, transform.position.y);

			//Debug.DrawLine(pos + new Vector2( Mathf.Sin(2f * Mathf.PI / 36.0f * i), Mathf.Cos(2f * Mathf.PI / 36.0f * i)) * vision_range , pos + new Vector2( Mathf.Sin(2f * Mathf.PI / 36.0f * i+1 ) , Mathf.Cos(2f * Mathf.PI / 36.0f * i + 1) ) * vision_range, Color.black , 0.5f );
		}
		if (IsServer)
		{
			bool ret = false;
			foreach (var a in NetworkManager.Singleton.ConnectedClients)
			{
				ret = Vector2.Distance(transform.position, a.Value.PlayerObject.transform.position) < vision_range;
				if (ret)
				{
					player_position = a.Value.PlayerObject.transform.position;
					break;
				}
				//Debug.Log(a.Value.PlayerObject.transform.position);
			}
			return ret;
		}
		//return Vector2.Distance(transform.position, player.transform.position) < vision_range;
		return false;

	}

	[ServerRpc(RequireOwnership = false)]
	public void take_damage_ServerRpc(float damage)
	{
		health.Value -= damage;
		if (health.Value <= 0)
		{
			state = Ai_state.die;
		}
	}


	private void OnCollisionEnter2D(Collision2D other)
	{
		//Debug.Log("naem :" + other.gameObject.tag);
		if (other.gameObject.tag == "Wall")
		{
			if (state == Ai_state.walk)
			{
				walking_direction *= -1;
			}
		}
		else if (other.gameObject.tag == "Bullet")
		{
			take_damage_ServerRpc(other.gameObject.GetComponent<Bullet_controller>().damage);
			//Debug.Log("health :" + health);			
		}
		else if (other.gameObject.tag == "Player")
		{
			//Debug.Log("Player hit");
			Player_controller.instance.take_damage(1);
		}
	}

	
	
}


