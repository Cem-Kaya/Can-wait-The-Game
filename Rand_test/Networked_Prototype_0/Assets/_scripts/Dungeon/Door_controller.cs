using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Door_controller : NetworkBehaviour
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

 //   [ServerRpc]
 //   private void to_teleport_ServerRpc(float posx, float posy)
 //   {
 //       teleport_to_ClientRpc(posx, posy);
 //   }

 //   [ClientRpc]
 //   private void teleport_to_ClientRpc(float posx, float posy)
	//{
        
 //   }

 
    IEnumerator reset()
    {
        yield return new WaitForEndOfFrame();
        is_colliding = false;
    }

	
	private void OnTriggerExit2D(Collider2D collision)
	{
        //Debug.Log("Exiting !!");
	}

    bool is_colliding = false;
    bool triger_guard = false;

    void OnTriggerEnter2D (Collider2D hitObject)        
    {
        //Debug.Log(" current room is: " + Room_controller.instance.current_room_info.x + " " +Room_controller.instance.current_room_info.y);
		// Debug.Log("IsServer" + IsServer.ToString() + " IsClient" + IsClient.ToString());
		if (! IsServer) return;
		
        if (hitObject.gameObject.layer == 3 )
        {
			if (is_colliding) return;            
            if (triger_guard) return;           
            
            triger_guard = true;
            is_colliding = true;            
            StartCoroutine(reset());

            if (door_cool_down) //door_cool_down
            {
                door_cool_down = true;
                StartCoroutine(cool_down());
                return;
            }
            
            Vector2 new_room_dir = transform.position - transform.parent.position;
            new_room_dir.Normalize();
			
            int x = Mathf.RoundToInt(new_room_dir.x);
            x += (int)Room_controller.instance.current_room_info.x;

            int y = Mathf.RoundToInt(new_room_dir.y);
            y += (int)Room_controller.instance.current_room_info.y;

            Debug.Log("coordinates are seen during collision: " + x + " " + y);

            /*
            if (!Room_controller.instance.does_room_exist(x, y))
            {
                Room_controller.Room_registered = false;
            }
            */

            foreach (var a in NetworkManager.Singleton.ConnectedClients)
            {
                a.Value.PlayerObject.GetComponent<box_mover>().teleport_to_ClientRpc(new Vector2(-transform.position.x + new_room_dir.x * 3.5f , -transform.position.y + new_room_dir.y * 3.5f ));
                // Debug.Log("teleported player " + a.Value.PlayerObject.NetworkObjectId + "  Y: " + a.Value.PlayerObject.transform.position.y);
            }           
           
            //Debug.Log("exist :"+ (confiner_room !=null).ToString() +" ,"  + confiner_room);
                      

            //artik asencron degil ki ?
            //Room_controller.instance.current_room_info = Room_controller.instance.loaded_rooms[(x, y)];
            //Room_controller.instance.current_room_info =
            Room_controller.instance.current_room_info.x = x;
            Room_controller.instance.current_room_info.y = y;
            Debug.Log("coordinates are seen during collision 2: " + Room_controller.instance.current_room_info.x + " " + Room_controller.instance.current_room_info.y);
            Room_controller.instance.current_room_info.room_name = "Defult_room";
			Room_controller.instance.current_room_info.room_name = Room_controller.instance.current_world_name;
			
			//Room_controller.instance.current_room.x = x;  // CURSED COPY BY REFERENCE !!!!
			//Room_controller.instance.current_room.y = y; // CURSED COPY BY REFERENCE !!!! 

			triger_guard = false;
			
            Room_controller.instance.load_room("Default_room", x, y);
        }
    }
}
