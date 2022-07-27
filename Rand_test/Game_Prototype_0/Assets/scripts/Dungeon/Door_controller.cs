using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door_controller : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
	private void OnCollisionEnter2D(Collision2D collision)
	{
        Vector2 new_room_dir = transform.position - transform.parent.parent.position;
        new_room_dir.Normalize();
        //Debug.Log("Direction of the room is= "+new_roo_dir);
        //int x = (int)new_room_dir.x + (int) Room_controller.instance.current_room.x;
        int x = (int)new_room_dir.x ;

        //int y = (int)new_room_dir.y + (int) Room_controller.instance.current_room.y;
        int y = (int)new_room_dir.y;
        //Debug.Log("Currrent room before loading = " + Room_controller.instance.current_room.x + " " + Room_controller.instance.current_room.y);
        Room_controller.instance.current_room.x = x;
        Room_controller.instance.current_room.y = y ;
		
        Room_controller.instance.load_room("Default_room", x, y);
        //Debug.Log("room to be:"+x+"," +y);

        /*
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        //multi oldugunda objects diye al sonra for looptan teleportla bam bum done
		
        player.GetComponent<box_mover>().teleport_to(new Vector2(x - new_room_dir.x *15  , y - new_room_dir.y * 8 ) );
		
        
        Room confiner_room = Room_controller.instance.loaded_rooms.Find(iter => iter.x == x && iter.y == y  ) ;
        //PolygonCollider2D confiner_object = confiner_room.GetComponentInParent<PolygonCollider2D>(); 
        PolygonCollider2D confiner_collider = confiner_room.GetComponentInParent<PolygonCollider2D>();
        Camera_controller.load_new_boundry(confiner_collider);
        Room_controller.instance.current_room = gameObject.GetComponent<Room>();  // Room_controller.instance.loaded_rooms[0];
	    */	
    }
}
