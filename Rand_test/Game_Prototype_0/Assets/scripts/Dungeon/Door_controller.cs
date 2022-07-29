using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Door_controller : MonoBehaviour
{
    // Start is called before the first frame update
    static bool door_cool_down = false;


    private void Awake()
    {
     
    }
    void Start()
    {

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


    IEnumerator wait_for_loading(int x, int y, Vector2 new_room_dir)
    {
        Debug.Log("in rutine "+ new_room_dir + "---" + x + "," + y);
        Room_controller.instance.load_room("Default_room", x, y);
        while (!Room_controller.Room_registered)
        {
            yield return new WaitForEndOfFrame();
        }



        GameObject player = GameObject.FindGameObjectWithTag("Player");
        //multi oldugunda objects diye al sonra for looptan teleportla bam bum done

        player.GetComponent<box_mover>().teleport_to(new Vector2(transform.position.x + new_room_dir.x * 5, transform.position.y + new_room_dir.y * 5));

        //Debug.Log("x = " + x + " y = " + y);
        Room confiner_room = Room_controller.instance.loaded_rooms[(x, y)];

        //Debug.Log("exist :"+ (confiner_room !=null).ToString() +" ,"  + confiner_room);

        GameObject room_object = confiner_room.gameObject;
        GameObject confiner_object = room_object.transform.Find("Cam_collider").gameObject;
        PolygonCollider2D confiner_collider = confiner_object.GetComponent<PolygonCollider2D>();
        //Debug.Log(confiner_collider);
        Camera_controller.load_new_boundry(confiner_collider);


        Room_controller.instance.current_room = Room_controller.instance.loaded_rooms[(x, y)];
        //Room_controller.instance.current_room.x = x;  // CURSED COPY BY REFERENCE !!!!
        //Room_controller.instance.current_room.y = y; // CURSED COPY BY REFERENCE !!!!
        Room_controller.instance.Debug_print_loaded_rooms();
        Room_controller.instance.loaded_rooms[(x, y)].deploy_room(x,y,"w","r");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        Debug.Log("Entered collusion !");
        if (collision.gameObject.layer == 3 )
        {
            
            if (false) //door_cool_down
            {
                door_cool_down = true;
                StartCoroutine(cool_down());
                return;
            }




            Vector2 new_room_dir = transform.position - transform.parent.parent.position;
            new_room_dir.Normalize();
            //Debug.Log("Direction of the room is= " + new_room_dir);
            int x = Mathf.RoundToInt(new_room_dir.x);
            x += (int)Room_controller.instance.current_room.x;

            int y = Mathf.RoundToInt(new_room_dir.y);
            y += (int)Room_controller.instance.current_room.y;
            Debug.Log(new_room_dir + "---"+x + "," + y);






            if (!Room_controller.instance.does_room_exist(x, y))
            {
                Room_controller.Room_registered = false;
            }

            StartCoroutine(wait_for_loading(x, y, new_room_dir));
            
        }
    }
}
