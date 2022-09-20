using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;
using TMPro;
using UnityEngine.UIElements;




public class box_mover : NetworkBehaviour
{
	public GameObject bullet_prefab; 
	public float speed ;   
	public Player_input_actions control ;
	public TextMeshProUGUI coin_text;
	public TextMeshProUGUI health_text;
	
	private ulong  last_firesd ;
	private ulong timer;
	public uint fdelay; // 0.01 sec is 1  
	private float terminal_velocity ;

	Rigidbody2D rb;
	private int moving;
	private Vector2 movement_direction;
	bool can_move;
	private int fireing ;
	private Vector2 fire_direction;

	[SerializeField] private Transform _spawner; // Netcode 

	int bullet_bounce;
	float bullet_lf; 	
	float bullet_dmg; 
	float bullet_scale;
	float bullet_speed;

	public override void OnNetworkSpawn()
	{
		//if (!IsOwner) Destroy(this);
	}


	public void Awake()
	{
		can_move = true;
		control = new Player_input_actions();
		terminal_velocity = 50 ;
		control.player.move.started += ctx => start_move(ctx.ReadValue<Vector2>());// gets input too early cant read multipress // register to the system with contect ctx 
		control.player.move.performed += ctx => mid_move(ctx.ReadValue<Vector2>()); // register to the system with contect ctx 
		control.player.move.canceled += ctx => end_move(ctx.ReadValue<Vector2>()); // register to the system with contect ctx 

		control.player.fire.started += ctx => start_fire(ctx.ReadValue<Vector2>());// gets input too early cant read multipress // register to the system with contect ctx 
		control.player.fire.performed += ctx => mid_fire(ctx.ReadValue<Vector2>()); // register to the system with contect ctx 
		control.player.fire.canceled += ctx => end_fire(ctx.ReadValue<Vector2>()); // register to the system with contect ctx 
																				   //Debug.Log("end of awake ");
		bullet_bounce=0;
		bullet_lf=0;
		bullet_dmg=0;
		bullet_scale =1 ;
		bullet_speed = 7 ;
	}
	
	public void Start()
	{
		
		last_firesd = 0;
		timer = 0;
		fdelay = 25 ;
		movement_direction = new Vector2(0, 0);
		moving = 0;
		speed = 7 ;
		rb = GetComponent<Rigidbody2D>();

		//coin_text =  GameObject.Find("Coin Text").GetComponent<TextMeshProUGUI>() ;
		//health_text = GameObject.Find("Health Text").GetComponent<TextMeshProUGUI>();
	}

	public void Update()
	{
	}

	public void FixedUpdate()
	{
		if (!IsOwner) return;
		rb.velocity = Vector3.ClampMagnitude(rb.velocity, terminal_velocity);
		//Debug.Log("V : "+ rb.velocity);
		fire();
		if (moving>0 && can_move ) {			
			rb.velocity = new Vector2(movement_direction.x , movement_direction.y );
			rb.velocity *= speed;
		}
		//Debug.Log("moving is :" + moving);

	}

	public void start_move(Vector2 input_diraction)
	{
		moving++;
	}

	public void mid_move(Vector2 input_diraction)
	{
		movement_direction = input_diraction;
		//Debug.Log("start move :" + input_diraction);
	}
		
	public void end_move(Vector2 input_diraction)
	{
		moving--;
		//Debug.Log("moveing: "+ moving);
	} 

	public void start_fire(Vector2 input_diraction)
	{
		fireing++;
	}

	public void mid_fire(Vector2 input_diraction)
	{
		fire_direction = input_diraction;
		//Debug.Log("start move :" + input_diraction);
	}

	public void end_fire(Vector2 input_diraction)
	{
		fireing--;
		//Debug.Log("moveing: "+ moving);
	}

	[ServerRpc]  //  (RequireOwnership = false) the function which runs on the server which will make some code run on the clients 	
	private void request_fire_ServerRpc(Vector3 fire_dir,  float in_bullet_lf, int in_bullet_bounce, float in_bullet_dmg, float in_bullet_scale, float in_bullet_speed )
	{
		//Debug.Log("send server rpc ");
		fire_ClientRpc(fire_dir, in_bullet_lf, in_bullet_bounce, in_bullet_dmg, in_bullet_scale, in_bullet_speed);
		
	}

	[ClientRpc]
	private void fire_ClientRpc(Vector3 fire_dir , float in_bullet_lf, int in_bullet_bounce, float in_bullet_dmg, float in_bullet_scale , float in_bullet_speed )
	{
		//Debug.Log("send client rpc ");
		execute_fire(fire_dir , in_bullet_lf, in_bullet_bounce, in_bullet_dmg, in_bullet_scale , in_bullet_speed );
	}
	
	private void execute_fire(Vector2 fire_dir , float in_bullet_lf, int in_bullet_bounce, float in_bullet_dmg,float in_bullet_scale, float in_bullet_speed )
	{
		//Debug.Log("timer: " + timer + " last_firesd" + last_firesd);
		GameObject bullet = Instantiate(bullet_prefab, _spawner.position + new Vector3(fire_dir.x, fire_dir.y, 0), Quaternion.identity);

		
		bullet.GetComponent<Bullet_controller>().set_bullet(in_bullet_lf, in_bullet_bounce, in_bullet_dmg, in_bullet_scale);
		bullet.GetComponent<Rigidbody2D>().velocity = fire_dir * in_bullet_speed ;
		last_firesd = timer;		

	}
	
	private void fire()		
	{		
		//Debug.Log("fireing"+ fireing.ToString() + "timer: " + timer + " last_firesd" + last_firesd);		
		if (timer++ > last_firesd + fdelay  &&  fireing > 0 ) {

			request_fire_ServerRpc(fire_direction, bullet_lf, bullet_bounce, bullet_dmg, bullet_scale, bullet_speed);
						
			//GameObject bullet = Instantiate(bullet_prefab, transform.position + new Vector3(fire_direction.x, fire_direction.y , 0) , Quaternion.identity);			
			//bullet.GetComponent<Rigidbody2D>().velocity = fire_direction * 10;
			last_firesd = timer;
		}
		
	}

	public void teleport_to(Vector2 to)
	{
		
		rb.velocity = new Vector2(0, 0);
		transform.position = to;
		
	}
	
	public void hide()
	{
		// get the sprite renderer component of this object
		GetComponent<SpriteRenderer>().enabled = false;
		can_move = false;
	}
	
	public void show()
	{
		foreach (var a in NetworkManager.Singleton.ConnectedClients)
		{
			if (a.Value.PlayerObject != null)
			{
				a.Value.PlayerObject.GetComponent<box_mover>().show() ;
			}
			// Debug.Log("teleported player " + a.Value.PlayerObject.NetworkObjectId + "  Y: " + a.Value.PlayerObject.transform.position.y);
		}
		// get the sprite renderer component of this object
		GetComponent<SpriteRenderer>().enabled = true;
		can_move = true ;
	}

	[ClientRpc]
	public void teleport_to_ClientRpc(Vector2 to)
	{
		if (IsOwner)
		{
			rb.velocity = new Vector2(0, 0);
			transform.position = to;
			hide();
		}
	}

	private void OnEnable()
	{
		control.Enable();	
	}
	private void OnDisable()
	{
		control.Disable();
	}
	public void dec_fire_rate_delay(uint delay)
	{		
		if ((int )fdelay - (int )delay < 0 )
		{
			fdelay = 0;
		}
		else
		{
			fdelay -= delay; 
		}
	}

	
	public void inc_speed(int spped_up)
	{
		int max_speed = 35;
		speed += spped_up; 
		if (speed> max_speed)
		{
			speed = max_speed;
		}
	}
	public void inc_bullet_att (float in_bullet_lf, int in_bullet_bounce, float in_bullet_dmg, float in_bullet_scale, float in_bullet_speed)
	{
		bullet_lf += in_bullet_lf;
		bullet_bounce += in_bullet_bounce;
		bullet_dmg += in_bullet_dmg;
		bullet_scale += in_bullet_scale;
		bullet_speed += in_bullet_speed;
	}
	public void inc_max_health_bm (int mx_health)
	{
		if(IsOwner){
			GameObject.Find("Player_controller").GetComponent<Player_controller>().inc_max_health(mx_health);
		}
	}
	public void inc_health_bm(int in_health)
	{
		if (IsOwner)
		{
			GameObject.Find("Player_controller").GetComponent<Player_controller>().inc_health(in_health);
		}
	}
	public void dec_coin_num(int price)
	{	
		GameObject.Find("Player_controller").GetComponent<Player_controller>().decrease_coin_num(price);
			
	}

}