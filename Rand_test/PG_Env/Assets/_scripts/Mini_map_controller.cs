using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using Unity.Netcode;

public class Mini_map_controller : NetworkBehaviour
{
    IEnumerator wait_for_map()
    {
        while (true)
        {
            yield return new WaitForEndOfFrame();
            if (Dungeon_controller.instance.created && Room_controller.instance.current_room_info != null)
            {
                break;
            }
            else
            {
                //Debug.Log("waiting for map fail ");
            }
        }
        //GetComponent<RawImage>().material.mainTexture =  Dungeon_controller.instance.texture ;
        if (IsServer)
        {
            //Debug.Log("is server sent rpc ");
            draw_texture_ClientRpc(Room_controller.instance.current_room_info.x, Room_controller.instance.current_room_info.y);
        }
        //when the gameobject is disabled the courotine is also removed from scheduler but the code in the middle of execution
        //continues until the end;
    }


    IEnumerator wait_for_map_client_fix(int room_x, int room_y)
    {
        while (true)
        {           
            if (Dungeon_controller.instance.created )
            {
                break;
            }
			yield return new WaitForEndOfFrame();
		}
		GetComponent<RawImage>().material.mainTexture = Dungeon_controller.instance.draw_player_copy_texture(room_x, room_y);
		this.gameObject.SetActive(false);
		this.gameObject.SetActive(true);
		this.gameObject.GetComponent<CanvasRenderer>().SetAlpha(0);
	}

	[ClientRpc]
	void draw_texture_ClientRpc(int room_x, int room_y)
	{
        //Debug.Log("was in rpc fro minimap");
        StartCoroutine(wait_for_map_client_fix(room_x, room_y));
	}

	// Start is called before the first frame update
	void Start()
    {
        StartCoroutine(wait_for_map());

    }

    public void OnNetworkSpawn()
    {
    }


    // Update is called once per frame
    void Update()
    {

    }
}