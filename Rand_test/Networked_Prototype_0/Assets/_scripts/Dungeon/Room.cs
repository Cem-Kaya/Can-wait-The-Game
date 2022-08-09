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

    public Room_info rooms_info;



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
        //client's room info will not be up to date keep this in mind

        if (IsServer)
        {

            rooms_info = Room_controller.instance.load_room_queue.Dequeue();

		    
            room_name = rooms_info.room_name;
            name = rooms_info.room_name;
            x = rooms_info.x;
            y = rooms_info.y;
            name = rooms_info.world_name + "-" + rooms_info.room_name + " " + rooms_info.x + ", " + rooms_info.y;


            Room_controller.instance.register_room(this);
        }


        //make sure we start in right scene
        if (Room_controller.instance == null) 
        {
            Debug.Log("Pressed play in wrong scene");
            return; 
        }

        Room_controller.Room_registered = true;
        
        var lplayer = NetworkManager.Singleton.LocalClient.PlayerObject;
        CinemachineVirtualCamera vcam = GameObject.Find("CM_vcam").GetComponent<CinemachineVirtualCamera>();
        vcam.Follow = lplayer.transform;
      

       

        GameObject room_object = gameObject;
        GameObject confiner_object = room_object.transform.Find("Cam_collider").gameObject;
        PolygonCollider2D confiner_collider = confiner_object.GetComponent<PolygonCollider2D>();
        //Debug.Log(confiner_collider);
        Camera_controller.load_new_boundry(confiner_collider);
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
