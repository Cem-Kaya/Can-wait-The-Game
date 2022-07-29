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

    public Room current_room;

    Room_info current_loading_room_data;

    Queue<Room_info> load_room_queue = new Queue<Room_info>();

    //public List<Room> loaded_rooms = new List<Room>();
    //public Hashtable  loaded_rooms = new Hashtable();
    public Dictionary<(int,int), Room> loaded_rooms = new Dictionary<(int, int), Room>();

    private bool is_loading_room = false;

    private bool room_registered = true;
    public static bool Room_registered
    {
        get{ return instance.room_registered; }
        set { instance.room_registered = value; }
    }
	
		
	


	private void Awake()
    {      
		if (instance == null)
        {
            instance = this;
        }
        Room_registered = true;
    }
    // Start is called before the first frame update
    void Start()
    {
        load_room("Start_room", 0, 0);

        //load_room("Default_room", 1,0);
        //load_room("Default_room", -1, 0);
        //load_room("Default_room", 0, 1);
        //load_room("Sample_room", 0, -1);


    }

    // Update is called once per frame
    void Update()
    {
		
    }


    public void load_room(string name, int in_x, int in_y)
    {
        //check to make sure room exists before we load a room so we dont load rooms that overlap
        //Debug.Log("Load Room func: " + in_x + in_y);
        if (does_room_exist(in_x, in_y))
        {
            return;
        }

        //we want to grab our room info and we will assign it to new room info
        Room_info new_room_data = new Room_info();
        new_room_data.name = name;
        new_room_data.x = in_x;
        new_room_data.y = in_y;

        //we want to be able to enqueue up our room for the scene manager to load for us, so
            
        StartCoroutine(load_room_routine(new_room_data));


    }

    IEnumerator load_room_routine(Room_info info)
    {
        //scenes won't load instantly, they'll take some time, depending on items so we want to load it up
        //before next scene starts so gameplay will be fluid.
        string room_name = info.name;
        //Debug.Log(room_name);
        //setting additive makes scenes overlap and its important cuz we want all rooms in same scene       
        AsyncOperation load_room = SceneManager.LoadSceneAsync(room_name, LoadSceneMode.Additive);
        Debug.Log("in rutine2 " + info.x+ ","+ info.y );
        //this makes courotine happy
        while (!load_room.isDone)
        {
            yield return new WaitForFixedUpdate();
        }

    }

    public void register_room(Room room)
    {
        //add room to loaded room
             

        //loaded_rooms.Add(room);
        //Debug.Log("Room's x,y" + (room.x, room.y));
        //Debug.Log("current_loading_room_data's x and y" + (current_loading_room_data.x, current_loading_room_data.y));
        loaded_rooms.Add((room.x, room.y), room);
        //Debug.Log("Deploy room one " + loaded_rooms[(room.x, room.y)]);
        //Debug.Log("nonexistent = " + ((loaded_rooms[(6,6)]) == null));
        
    }

  
    public bool does_room_exist(int in_x, int in_y)
    {

        if (loaded_rooms.ContainsKey((in_x, in_y)) == true)
        {
                    return true;
        }

        return false;
        
    }

    public void Debug_print_loaded_rooms()
    {

        string all_rooms = "All loaded rooms in " + current_room.x.ToString() + current_room.y.ToString() + "\n";
        foreach (KeyValuePair<(int,int),Room> r in loaded_rooms)
        {
            Room room = r.Value;
            all_rooms += room.x.ToString() + room.y.ToString() + " ";
        }
        Debug.Log(all_rooms);
    }

}