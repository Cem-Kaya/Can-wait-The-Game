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

    public void load_room(string name, int x, int y)
    {
        //we want to grab our room info and we will assign it to new room info
        Room_info new_room_data = new Room_info();
        new_room_data.name = name;
        new_room_data.x = x;
        new_room_data.y = y;

        //we want to be able to enqueue up our room for the scene manager to load for us, so
        load_room_queue.Enqueue(new_room_data);

    }

    IEnumerator load_room_routine(Room_info info)
    {
        //scenes won't load instantly, they'll take some time, depending on items so we want to load it up
        //before next scene starts so gameplay will be fluid.
        string room_name = current_world_name + info.name;

        //setting additive makes scenes overlap and its important cuz we want all rooms in same scene       
        AsyncOperation load_room = SceneManager.LoadSceneAsync(room_name, LoadSceneMode.Additive);
        
        //this makes courotine happy
        while (!load_room.isDone)
        {
            yield return null; //what does this do??? why return null?
        }
       
    }

    public void register_room(Room room)
    {
        //this will set our room within our scene in right coordinates
        room.transform.position = new Vector3 (current_loading_room_data.x * room.width, current_loading_room_data.y * room.height, 0 );
        room.x = current_loading_room_data.x;
        room.y = current_loading_room_data.y;
        room.name = current_world_name + "-" + current_loading_room_data.name + " " + room.x + ", " + room.y;
        room.transform.parent = transform;

        is_loading_room = false;

        loaded_rooms.Add(room);
    }


    public bool does_room_exist(int in_x, int in_y)
    {
        return loaded_rooms.Find(item => item.x == in_x && item.y == in_y ) != null;
    }

    
}
