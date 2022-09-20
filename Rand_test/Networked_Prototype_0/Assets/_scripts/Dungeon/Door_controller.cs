using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Door_controller : NetworkBehaviour
{
	// Start is called before the first frame update
	static bool door_cool_down = false;
	GameObject left_door;
	GameObject right_door;

	bool locked ;
	private void Awake()
	{
		locked = true;
		Starting_room_init.on_no_enemy += unlock_doors;
		Inner_layout_manager.on_no_enemy += unlock_doors;
		left_door = transform.GetChild(0).gameObject;
		right_door = transform.GetChild(1).gameObject;
	}

	IEnumerator door_mover()
	{
		for (int i = 0; i < 100; i++)
		{			
			yield return new WaitForFixedUpdate();
			yield return new WaitForFixedUpdate();
			left_door.transform.localPosition = new Vector3 ( left_door.transform.localPosition.x -  0.00225f, left_door.transform.localPosition.y , left_door.transform.position.z);
			right_door.transform.localPosition = new Vector3 (right_door.transform.localPosition.x + 0.00225f  , right_door.transform.localPosition.y, right_door.transform.position.z);
			yield return null ;

		}
		//Debug.Log("door is done ");
	}
	public void unlock_doors()
	{
		//Debug.Log("unlocking doors");
		locked = false;
		StartCoroutine(door_mover());

	}

	IEnumerator wait_for_map()
	{
		while (true)
		{
			yield return new WaitForSeconds(0.0001f);
			if (Dungeon_controller.instance.created && Room_controller.instance !=null &&Room_controller.instance.start_room_initialized && Room_controller.instance.current_room_info != null)
			{
				break;
			}           
		}

		
		(int, int) pos = (Room_controller.instance.current_room_info.x, Room_controller.instance.current_room_info.y);
		door_dir room_type = Dungeon_controller.instance.current_floor.floor_data[pos].value;
		if (IsServer)
		{
			if ( transform.position.y > 7 && !Dungeon_controller.instance.current_floor.up_connection.Contains(room_type))
			{
				GetComponent<NetworkObject>().Despawn();
			}
			else if (transform.position.y < -7 && !Dungeon_controller.instance.current_floor.down_connection.Contains(room_type))
			{
				GetComponent<NetworkObject>().Despawn();
			}
			else if (transform.position.x > 13 && !Dungeon_controller.instance.current_floor.right_connection.Contains(room_type))
			{
				GetComponent<NetworkObject>().Despawn();
			}
			else if (transform.position.x < -13 && !Dungeon_controller.instance.current_floor.left_connection.Contains(room_type))
			{
				GetComponent<NetworkObject>().Despawn();
			}
			else
			{
				activate_doors_ClientRpc();

			}
		}

	}

	[ClientRpc]
	public void activate_doors_ClientRpc()
	{
		GetComponent<PolygonCollider2D>().enabled = true;
		foreach (Transform child in transform)
		{
			child.gameObject.SetActive(true);
		}
	}

	void Start()		
	{ 
		
		StartCoroutine(wait_for_map());
	}

	// Update is called once per frame
	void FixedUpdate()
	{


	}

	private void Update()
	{
	   
	}

	private IEnumerator cool_down()
	{
		yield return new WaitForSeconds(0.25f);
		door_cool_down = false;
	}

 //   [ServerRpc]
 //   private void to_teleport_ServerRpc(float posx, float posy)
 //   {
 //       teleport_to_ClientRpc(posx, posy);
 //   }

 //   [ClientRpc]
 //   private void teleport_to_ClientRpc(float posx, float posy)
	//{
		
 //   }

 
	IEnumerator reset()
	{
		yield return new WaitForEndOfFrame();
		is_colliding = false;
	}

	
	private void OnTriggerExit2D(Collider2D collision)
	{
		//Debug.Log("Exiting !!");
	}

	bool is_colliding = false;
	bool triger_guard = false;


	[ServerRpc (RequireOwnership =false )]
	public void change_room_ServerRpc (Vector2 new_room_dir)
	{

		if (locked)
		{
			return;
		}

		if (is_colliding) return;
		if (triger_guard) return;

		triger_guard = true;
		is_colliding = true;
		StartCoroutine(reset());

		if (door_cool_down) //door_cool_down
		{
			door_cool_down = true;
			StartCoroutine(cool_down());
			return;
		}
		//Debug.Log("coordinates are seen during collision: " + x + " " + y);
		int x = Mathf.RoundToInt(new_room_dir.x);
		x += (int)Room_controller.instance.current_room_info.x;

		int y = Mathf.RoundToInt(new_room_dir.y);
		y += (int)Room_controller.instance.current_room_info.y;

		foreach (var a in NetworkManager.Singleton.ConnectedClients)
		{
			if (a.Value.PlayerObject != null) {
				a.Value.PlayerObject.GetComponent<box_mover>().teleport_to_ClientRpc(new Vector2(-transform.position.x + new_room_dir.x * 3.5f, -transform.position.y + new_room_dir.y * 3.5f));
			
			}
			
			// Debug.Log("teleported player " + a.Value.PlayerObject.NetworkObjectId + "  Y: " + a.Value.PlayerObject.transform.position.y);
		}

		//artik asencron degil ki ?
		//Room_controller.instance.current_room_info = Room_controller.instance.loaded_rooms[(x, y)];
		//Room_controller.instance.current_room_info =
		Room_controller.instance.current_room_info.x = x;
		Room_controller.instance.current_room_info.y = y;

		Room_controller.instance.current_room_info.room_name = "Defult_room";
		Room_controller.instance.current_room_info.room_name = Room_controller.instance.current_world_name;

		//Room_controller.instance.current_room.x = x;  // CURSED COPY BY REFERENCE !!!!
		//Room_controller.instance.current_room.y = y; // CURSED COPY BY REFERENCE !!!! 

		triger_guard = false;

		Room_controller.instance.load_room("Default_room", x, y);
	}



	void OnTriggerEnter2D (Collider2D hitObject)        
	{
		//Debug.Log(" current room is: " + Room_controller.instance.current_room_info.x + " " +Room_controller.instance.current_room_info.y);
		// Debug.Log("IsServer" + IsServer.ToString() + " IsClient" + IsClient.ToString());
		
		if (hitObject.gameObject.layer == 3 )
		{
			Vector2 new_room_dir = transform.position - transform.parent.position;
			new_room_dir.Normalize();
			change_room_ServerRpc(new_room_dir);

		}
	}
	
	public override void OnNetworkDespawn()
	{
		Inner_layout_manager.on_no_enemy -= unlock_doors;
		Starting_room_init.on_no_enemy -= unlock_doors;
	}
}
