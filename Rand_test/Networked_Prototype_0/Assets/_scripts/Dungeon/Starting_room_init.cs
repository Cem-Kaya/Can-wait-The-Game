using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Unity.Netcode;
using UnityEngine.UI;

public class Starting_room_init : NetworkBehaviour
{
	static bool first = true;
	public GameObject confiner_object;

	public PolygonCollider2D confiner_collider;
	private void Awake()
	{

		//Room_controller.instance.GetComponent<NetworkObject>().Spawn();


		once = true;
		if (IsClient) Destroy(this);
		confiner_object = GameObject.Find("Cam_collider");
		confiner_collider = confiner_object.GetComponent<PolygonCollider2D>();
		Camera_controller.load_new_boundry(confiner_collider);

	}
	Tile start_room_tile;
	IEnumerator wait_for_map()
	{
		while (true)
		{			
			if (Dungeon_controller.instance.created)
			{	
				break;
			}
			yield return new WaitForEndOfFrame();
		}
		start_room_tile = Dungeon_controller.instance.current_floor.any_node_from_max_tree;
		
		if (Room_controller.instance.load_room_queue.Count == 0 && once)
		{
			once = false;
			Room_info tmp_inf = new Room_info("Starting room", Room_controller.instance.current_world_name, start_room_tile.x_cord, start_room_tile.y_cord );
			Room_controller.instance.load_room_queue.Enqueue(tmp_inf);
		}
	}


	IEnumerator wait_untill_rc_not_null()
	{
		while (Room_controller.instance == null)
		{
			yield return new WaitForEndOfFrame();
		}
		//Debug.Log("room 36" + (Room_controller.instance == null).ToString());
		Room_controller.instance.current_room_info = new Room_info(gameObject.GetComponent<Room>());  //runtime error     Room_controller.instance.loaded_rooms[0];
	}

	void Start()
	{
		StartCoroutine(wait_untill_rc_not_null());
		var lplayer = NetworkManager.Singleton.LocalClient.PlayerObject;
		CinemachineVirtualCamera vcam = GameObject.Find("CM_vcam").GetComponent<CinemachineVirtualCamera>();
		vcam.Follow = lplayer.transform;

	}

	bool once = true;
	public void init_start()
	{
        StartCoroutine(wait_for_map());
    }
    // Update is called once per frame
    void Update()
	{

	}
}
