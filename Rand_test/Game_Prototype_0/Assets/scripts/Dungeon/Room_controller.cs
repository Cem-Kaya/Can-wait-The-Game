using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

class Room_info
{
    public string name;
    public int x;
    public int y;

}
// Jungle 

public class Room_controller : MonoBehaviour
{

    public static Room_controller instance;

    string current_world_name = "Jungle";

    Room_info current_loading_room_data;

    Queue<Room_info> load_room_queue = new Queue<Room_info>();

    public List<Room> loaded_rooms = new List<Room>();

    bool is_loading_room = false;

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
		
	}
	// Start is called before the first frame update
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool  does_room_exist(int in_x, int in_y)
    {
        return loaded_rooms.Find(item => item.x == in_x && item.y == in_y ) != null;
    }
}
