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
        Room_controller.instance.deploy_room(this);
       

    }


    // Start is called before the first frame update
    void Start()
    {

        //make sure we start in right scene
        if (Room_controller.instance == null) //then we pressed play in the wrong scene
        {
            Debug.Log("Pressed play in wrong scene");
            return; 
        }

        Room_controller.Room_deployed = true; 


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
