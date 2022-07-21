using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
	
public class box_mover : MonoBehaviour
{
	public GameObject bullet_prefab; 
    public float speed ;   
    public Player_input_actions control ;
	public Text coin_text;

	private ulong  last_firesd ;
	private ulong timer;
	private uint fdelay; // 0.01 sec is 1  
	
	public static int coin_num; 
	Rigidbody2D rb;
	private int moving;
	private Vector2 movement_direction;
	
	private int fireing ;
	private Vector2 fire_direction;

	public void Awake()
	{		
		control = new Player_input_actions();
		
		control.player.move.started += ctx => start_move(ctx.ReadValue<Vector2>());// gets input too early cant read multipress // register to the system with contect ctx 
		control.player.move.performed += ctx => mid_move(ctx.ReadValue<Vector2>()); // register to the system with contect ctx 
		control.player.move.canceled += ctx => end_move(ctx.ReadValue<Vector2>()); // register to the system with contect ctx 

		control.player.fire.started += ctx => start_fire(ctx.ReadValue<Vector2>());// gets input too early cant read multipress // register to the system with contect ctx 
		control.player.fire.performed += ctx => mid_fire(ctx.ReadValue<Vector2>()); // register to the system with contect ctx 
		control.player.fire.canceled += ctx => end_fire(ctx.ReadValue<Vector2>()); // register to the system with contect ctx 

		


		//Debug.Log("end of awake ");
	}
	public void Start()
	{
		last_firesd = 0;
		timer = 0;
		fdelay = 1;
		movement_direction = new Vector2(0, 0);
		moving = 0;
		coin_num = 0;
		speed = 5;
		rb = GetComponent<Rigidbody2D>();
	}
	public void Update()
	{
		coin_text.text = " coin :" + coin_num;
	}

	public void FixedUpdate()
	{
		fire();
		if (moving>0) {			
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
	private void fire()
	{
		//Debug.Log("timer: " + timer + " last_firesd" + last_firesd);
		if (timer++ > last_firesd + fdelay  &&  fireing > 0 ) {
			GameObject bullet = Instantiate(bullet_prefab, transform.position + new Vector3(fire_direction.x, fire_direction.y , 0) , Quaternion.identity);			
			bullet.GetComponent<Rigidbody2D>().velocity = fire_direction * 10;
			last_firesd = timer;
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
}
