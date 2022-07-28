using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Door_controller : MonoBehaviour
{
    // Start is called before the first frame update
    static bool door_cool_down = false;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private IEnumerator coll_down()
    {
        yield return new WaitForSeconds(1f);
        door_cool_down = false;
    }


    IEnumerator wait_for_loading(int x , int y , Vector2 new_room_dir )
    {
        while ( ! Room_controller.Room_deployed)
        {
            yield return new WaitForFixedUpdate();
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        //multi oldugunda objects diye al sonra for looptan teleportla bam bum done

        player.GetComponent<box_mover>().teleport_to(new Vector2(transform.position.x + new_room_dir.x * 5, transform.position.y + new_room_dir.y * 5));
        //var confiner_room = new Room();
        Debug.Log("x = " + x + " y = " + y);
        Room confiner_room = Room_controller.instance.loaded_rooms[(x, y)] as Room;


        Debug.Log("name : " + confiner_room);
        ///////////////////////////////////////////////////////////////////////////
        //Debug.Log("Room" + confiner_room.x + confiner_room.y);
        GameObject room_object = confiner_room.gameObject;
        //PolygonCollider2D confiner_object = confiner_room.GetComponentInParent<PolygonCollider2D>(); 
        GameObject confiner_object = room_object.transform.Find("Cam_collider").gameObject;
        PolygonCollider2D confiner_collider = confiner_object.GetComponent<PolygonCollider2D>();
        //Debug.Log(confiner_collider);
        Camera_controller.load_new_boundry(confiner_collider);
    }
	
    private void OnCollisionEnter2D(Collision2D collision)
	{

        if ( collision.gameObject.layer == 3)
        {
            if (!door_cool_down)
            {
                door_cool_down = true;
                StartCoroutine(coll_down());

                Vector2 new_room_dir = transform.position - transform.parent.parent.position;
                new_room_dir.Normalize();
                //Debug.Log("Direction of the room is= " + new_room_dir);
                int x = Mathf.RoundToInt(new_room_dir.x);
                x += (int)Room_controller.instance.current_room.x;
                //int x =  Mathf.RoundToInt( new_room_dir.x );

                int y = Mathf.RoundToInt(new_room_dir.y);
                y += (int)Room_controller.instance.current_room.y;
                //int y = Mathf.RoundToInt(new_room_dir.y );
                //Debug.Log("Current room to be given= " + x + " " + y);

                //Debug.Log("Current room before loading = " + Room_controller.instance.current_room.x + " " + Room_controller.instance.current_room.y);



                Room_controller.instance.load_room("Default_room", x, y);
                Room_controller.instance.current_room.x = x;
                Room_controller.instance.current_room.y = y;

                Room_controller.Room_deployed = false;
                StartCoroutine(wait_for_loading(x , y , new_room_dir ));
			}
        }
    }
}
