using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class Room_info
{
    public string room_name;
    public string world_name;
    public int x;
    public int y;



    public Room_info(string in_room_name, string in_world_name, int in_x, int in_y)
    {
        room_name = in_room_name;
        world_name = in_world_name;
        x = in_x;
        y = in_y;
    }

    public Room_info(Room room_in )
    {
        room_name = room_in.room_name;
        world_name = Room_controller.instance.current_world_name;
        x = room_in.x;
        y = room_in.y;
    }

    public Room_info()
    {
        room_name = "default constructor ? ";
        world_name = "default constructer ? ";
        x = 0;
        y = 0;
    }

}
// Jungle 

public class Room_controller : NetworkBehaviour
{

    public static Room_controller instance;

    public string current_world_name = "Jungle";

    public Room_info current_room_info ;

    public Queue<Room_info> load_room_queue = new Queue<Room_info>();

    //public List<Room> loaded_rooms = new List<Room>();
    //public Hashtable  loaded_rooms = new Hashtable();
    public Dictionary<(int,int), Room_info> loaded_rooms = new Dictionary<(int, int), Room_info>();
    public Dictionary <(int, int), bool> cleared_rooms = new Dictionary<(int, int), bool>();
       
    private bool room_registered = true;

    public bool start_room_initialized = false;

    public static bool Room_registered
    {
        get{ return instance.room_registered; }
        set { instance.room_registered = value; }
    }
	
		
	


	private void Awake()
    {
        if (! IsServer) Destroy(this);
		
		if (instance == null)
        {
            //Debug.Log("creating room controller ");
            instance = this;
            DontDestroyOnLoad(this.gameObject);
            Room_registered = true;
        }
		
        
    }
    // Start is called before the first frame update
    void Start()
    {
        //load_room("Start_room", 1, 0);
        //load_room("Default_room", 1,0);
        //load_room("Default_room", -1, 0);
        //load_room("Default_room", 0, 1);
        //load_room("Sample_room", 0, -1);
    }
	
	


	// Update is called once per frame
	void Update()
    {
		
    }


    public void load_room(string in_name, int in_x, int in_y)
    {
        //check to make sure room exists before we load a room so we dont load rooms that overlap
        Debug.Log("Load Room func: " + in_x + in_y);
        
        if (does_room_exist(in_x, in_y))
        {
            Debug.Log("the room that you are trying to load already exists ! ");
            return;
        }
        
        //we want to grab our room info and we will assign it to new room info
        Room_info new_room_data = new Room_info();
        new_room_data.room_name = in_name;
        new_room_data.x = in_x;
        new_room_data.y = in_y;
		new_room_data.world_name = current_world_name;
		//we want to be able to enqueue up our room for the scene manager to load for us, so

		//StartCoroutine(load_room_routine(new_room_data));
		//my changes are from here /////////////////////////
		load_room_queue.Enqueue(new_room_data);
        string room_name = new_room_data.room_name;
        
		Debug.Log("isServer : " + IsServer.ToString() + " is client : " + IsClient.ToString() + " is host : " + IsHost.ToString() + " is localplayer : " + IsLocalPlayer.ToString() + " is owner : " +  IsOwner.ToString() + " is spawned : " + IsSpawned.ToString());
        if (true)
        {
            var load_room = NetworkManager.Singleton.SceneManager.LoadScene(room_name, LoadSceneMode.Single);

        }

    }


    
    public void register_room(Room room)
    {
        //add room to loaded room

                
        loaded_rooms.Add((room.x, room.y), new  Room_info(room.room_name, Room_controller.instance.current_world_name , room.x, room.y) );
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

        string all_rooms = "All loaded rooms in " + current_room_info.x.ToString() + current_room_info.y.ToString() + "\n";
        foreach (KeyValuePair<(int,int),Room_info> r in loaded_rooms)
        {
            Room_info room = r.Value;
            all_rooms += room.x.ToString() + room.y.ToString() + " ";
        }
        Debug.Log(all_rooms);
    }

}