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
        Vector3 new_roo_dir = transform.position - transform.parent.parent.position;
        new_roo_dir.Normalize();
        //Debug.Log("Direction of the room is= "+new_roo_dir);
        int x = (int)new_roo_dir.x;
        int y = (int)new_roo_dir.y;
		Room_controller.instance.load_room("Default_room", x, y);
		
        
        GameObject confiner_object = Room_controller.instance.loaded_rooms.Find("Cam_collider");
        PolygonCollider2D confiner_collider = confiner_object.GetComponent<PolygonCollider2D>();
        Camera_controller.load_new_boundry(confiner_collider);
        Room_controller.instance.current_room = gameObject.GetComponent<Room>();  // Room_controller.instance.loaded_rooms[0];
		
    }
}
