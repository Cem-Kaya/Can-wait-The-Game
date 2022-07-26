using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public string room_name;
    public int x;
    public int y;

    public int width; 
    public int height;

    // Start is called before the first frame update
    void Start()
    {
        //make sure we start in right scene
        if(Room_controller.instance == null) //then we pressed play in the wrong scene
        {
            Debug.Log("Pressed play in wrong scene");
            return; 
        }
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
	public void load_room() {
		
    }

}
