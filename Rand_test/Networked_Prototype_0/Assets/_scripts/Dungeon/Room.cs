using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Unity.Netcode;

public class Room : NetworkBehaviour
{
    public string room_name;
    public int x;
    public int y;

    public int width; 
    public int height;



    private void Awake()
    {

        
    }


    // Start is called before the first frame update
    void Start()
    {
        if (!Room_controller.instance.start_room_initialized )
        {
            Room_controller.instance.start_room_initialized = true;
            Starting_room_init starting_room = GetComponent<Starting_room_init>();
            starting_room.init_start();
        }
        Room_info inf = Room_controller.instance.load_room_queue.Dequeue();
        room_name = inf.room_name;
		name = inf.room_name;
        x = inf.x;
        y = inf.y;
        name = inf.world_name + "-" + inf.room_name + " " + inf.x + ", " + inf.y;



        Room_controller.instance.register_room(this);



        //make sure we start in right scene
        if (Room_controller.instance == null) 
        {
            Debug.Log("Pressed play in wrong scene");
            return; 
        }

        Room_controller.Room_registered = true;
        
        //Room_controller.instance.Debug_print_loaded_rooms();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(width, height, 0));
    }

	public Vector2 return_room_center()
	{
        return new Vector2(x * width, y * height);
	}
	

}
