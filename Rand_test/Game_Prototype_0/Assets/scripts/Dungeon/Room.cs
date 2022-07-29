using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Room : MonoBehaviour
{
    public string room_name;
    public int x;
    public int y;

    public int width; 
    public int height;

  

    private void Awake()
    {
        Room_info inf = Room_controller.instance.load_room_queue.Dequeue();
        room_name = inf.name;
        x = inf.x;
        y = inf.y;

        Room_controller.instance.register_room(this);
       
    }
    public void deploy_room(int in_x , int in_y, string world_name , string room_name )
	{
        //transform.position = new Vector3(current_loading_room_data.x * room.width, current_loading_room_data.y * room.height, 0);
        transform.position = new Vector3(in_x * width, in_y * height, 0);

        x = in_x;
        y = in_y;
        name = world_name + "-" + room_name + " " + in_x + ", " + in_y;
        transform.parent = transform;

    }

    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector2(-999*width , -999*height );

        //make sure we start in right scene
        if (Room_controller.instance == null) //then we pressed play in the wrong scene
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
