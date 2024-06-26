using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.Netcode;




public class Enemy_ai3_boss_2 : NetworkBehaviour
{
	public GameObject enemy_bulet_prefab;
	public Ai_state state = Ai_state.walk;
	public float vision_range = 3;
	public float speed = 1;
	public ulong fdelay;


	public Vector3 player_position;
	public NetworkVariable<float> health = new NetworkVariable<float>();
	public int health_for_setting;

	private ulong timer;
	private ulong last_firesd;
	private Quaternion look_dir;
	private Vector2 walking_direction;
	private Rigidbody2D rb;
	
	private bool once;
	
	// event for when the ai dies
	//
	public delegate void on_death_event();
	public static event on_death_event on_death;



	// Start is called before the first frame update
	private void Awake()
	{
		state = Ai_state.walk;
		timer = 0;
		last_firesd = 0;
		health.Value = 50.0f;
		player_position = new Vector3(-999, -999, -999);
		once = false;
	}

	void Start()
	{
		//fdelay = 200;

		rb = GetComponent<Rigidbody2D>();
		StartCoroutine(random_walk());
		if (IsServer) health.Value = health_for_setting;
	}

	void Update()
	{

	}


	private void FixedUpdate()
	{
		timer++;
		if (close_enough() && state != Ai_state.attack && state != Ai_state.die)
		{
			state = Ai_state.attack;
		}
		else if (state != Ai_state.die)
		{
			state = Ai_state.walk;
		}
		if (IsServer)
		{
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
		if (rb != null)
		{
			rb.velocity = Vector2.ClampMagnitude(walking_direction * speed + new Vector2(0.5f, 0.5f), speed);
		}
	}


	[ClientRpc]
	private void show_attack_ClientRpc(Vector3 fire_dir)
	{
		GameObject bullet = Instantiate(enemy_bulet_prefab, transform.position, Quaternion.identity);
		fire_dir.Normalize();		
		bullet.GetComponent<Rigidbody2D>().velocity = fire_dir * 10;

	}

	private void attack()
	{
		if (timer++ > last_firesd + fdelay)
		{
			Vector3 fire_direction = player_position - transform.position;
			show_attack_ClientRpc(fire_direction);
			//GameObject bullet = Instantiate(enemy_bulet_prefab, transform.position , Quaternion.identity);
			//fire_direction.Normalize();
			//bullet.GetComponent<Rigidbody2D>().velocity = fire_direction * 10;
			last_firesd = timer;
		}
	}
	private void die()
	{
		//Debug.Log("die " + name +"server ? : " + IsServer.ToString() );
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
		//on_death?.Invoke();
		yield return new WaitForFixedUpdate();
		//gameObject.GetComponent<NetworkObject>().Despawn();
		Destroy(gameObject);		
	}


	private bool close_enough()
	{
		//NetworkManager.Singleton.ConnectedClients;


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
				if (a.Value.PlayerObject != null)
				{
					ret = Vector2.Distance(transform.position, a.Value.PlayerObject.transform.position) < vision_range;
					if (ret)
					{
						player_position = a.Value.PlayerObject.transform.position;
						break;
					}
					//Debug.Log(a.Value.PlayerObject.transform.position);
				}
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
		//Debug.Log("health :" + health);
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
			//Debug.Log("took damage " + health.Value); 
		}
		else if (other.gameObject.tag == "Player")
		{
			//Debug.Log("Player hit");
			if (other.gameObject.GetComponent<NetworkObject>().IsOwner)
			{
				//Debug.Log("should take damage from enemy ai 1");
				Player_controller.instance.take_damage(1); //GameObject.Find("Player_controller").GetComponent<Player_controller>().take_damage(1);

			}
		}
	}
}


