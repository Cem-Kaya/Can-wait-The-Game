using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Unity.Netcode;
public class Starting_room_init : NetworkBehaviour
{
    public GameObject confiner_object;

    public PolygonCollider2D confiner_collider;
    private void Awake()
	{
        Room_controller.instance.GetComponent<NetworkObject>().Spawn();
        once = true;
        if (IsClient) Destroy(this);
        confiner_object = GameObject.Find("Cam_collider");
        confiner_collider = confiner_object.GetComponent<PolygonCollider2D>();
        Camera_controller.load_new_boundry(confiner_collider);
        
    }
    
    void Start()
    {
        Room_controller.instance.current_room_info = new Room_info( gameObject.GetComponent<Room>()) ;  // Room_controller.instance.loaded_rooms[0];

    }

    bool once = true;
    public void init_start()
    {
        if (Room_controller.instance.load_room_queue.Count == 0 && once) 
        { 			
            once = false;
            Room_info tmp_inf = new Room_info("Starting room", Room_controller.instance.current_world_name, 0, 0);
            Room_controller.instance.load_room_queue.Enqueue(tmp_inf);
        }
    }
	// Update is called once per frame
	void Update()
	{

	}
}
