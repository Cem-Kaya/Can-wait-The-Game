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

    IEnumerator wait_untill_rc_not_null()
    {
        while (Room_controller.instance == null)
        {
            yield return new WaitForEndOfFrame();
        }
		
        //Debug.Log("room 36" + (Room_controller.instance == null).ToString());
        if (!Room_controller.instance.start_room_initialized) // runtime error 
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
            yield break;// return 
        }
        Room_controller.Room_registered = true;

        var lplayer = NetworkManager.Singleton.LocalClient.PlayerObject;
        CinemachineVirtualCamera vcam = GameObject.Find("CM_vcam").GetComponent<CinemachineVirtualCamera>();
        vcam.Follow = lplayer.transform;

        GameObject room_object = gameObject;
        float screen_w = Screen.width;
        float screen_h = Screen.height;
        float as_ratio = screen_w / screen_h;
        GameObject confiner_object = room_object.transform.Find("Cam_collider").gameObject;
        if (as_ratio > 16.0f / 9.0f + 0.02f)
        {
            confiner_object = room_object.transform.Find("Cam_collider_ultrawide(0.875)").gameObject;
        }
        
        
        PolygonCollider2D confiner_collider = confiner_object.GetComponent<PolygonCollider2D>();
        //Debug.Log(confiner_collider);
        Camera_controller.load_new_boundry(confiner_collider);
    }

    void Start()
    {
        StartCoroutine(wait_untill_rc_not_null() );
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
