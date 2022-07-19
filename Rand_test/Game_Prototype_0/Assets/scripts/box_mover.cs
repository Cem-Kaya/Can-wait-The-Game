//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
	
public class box_mover : MonoBehaviour
{
    public float speed ;   
    public Player_input_actions control ;

	Rigidbody2D rb;
	private int moving;
	private Vector2 movement_direction;

	public void Awake()
	{		
		control = new Player_input_actions();
		
		control.player.move.started += ctx => start_move(ctx.ReadValue<Vector2>());// gets input too early cant read multipress // register to the system with contect ctx 
		control.player.move.performed += ctx => mid_move(ctx.ReadValue<Vector2>()); // register to the system with contect ctx 
		control.player.move.canceled += ctx => end_move(ctx.ReadValue<Vector2>()); // register to the system with contect ctx 

		//Debug.Log("end of awake ");
	}
	public void Start()
	{
		movement_direction = new Vector2(0, 0);
		moving = 0;
		speed = 5;
		rb = GetComponent<Rigidbody2D>();
	}
	public void Update()
	{
		
	}

	public void FixedUpdate()
	{
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

	private void OnEnable()
	{
		control.Enable();	
	}
	private void OnDisable()
	{
		control.Disable();
	}
}
