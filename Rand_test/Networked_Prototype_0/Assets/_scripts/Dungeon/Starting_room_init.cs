using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Unity.Netcode;
public class Starting_room_init : NetworkBehaviour
{
    public GameObject confiner_object;

    public PolygonCollider2D confiner_collider;
    private void Awake()
	{
        if (IsClient) Destroy(this);
        confiner_object = GameObject.Find("Cam_collider");
        confiner_collider = confiner_object.GetComponent<PolygonCollider2D>();
        Camera_controller.load_new_boundry(confiner_collider);
        Room_controller.instance.current_room =  gameObject.GetComponent<Room>() ;  // Room_controller.instance.loaded_rooms[0];
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
