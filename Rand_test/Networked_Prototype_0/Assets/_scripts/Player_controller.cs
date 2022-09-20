using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using Unity.Netcode;

// Singleton
public class Player_controller : NetworkBehaviour
{   
	public static  Player_controller instance;
	
	public   float bullet_speed =10f;
	public int health = 20 ;   
	public int max_health = 20 ; // max health of the player    
	public int fire_delay = 2 ;
	public float move_speed;
	public bool alive;
	public bool i_frame ;
	public float i_frame_sec;
	public NetworkVariable<int> num_dead;
    public NetworkVariable<int> num_at_menu;

    public NetworkVariable<int> coin_num;


	
	void Awake()
	{
		num_at_menu.Value = 0;
		i_frame = false;
		i_frame_sec = 0.5f;
		
		if (instance == null)
		{
			instance = this;
		}
		else
		{
			Destroy(gameObject);
		}
		coin_num.OnValueChanged += update_coin_text_on_change;
	}
	void Start()
	{
		DontDestroyOnLoad(this.gameObject);
		coin_num.Value = 0;
		num_dead.Value = 0;
		
	}

	// Update is called once per frame
	void Update()
	{
		
	}
	private  IEnumerator i_frame_delay()
	{

		yield return new WaitForSeconds(i_frame_sec);
		i_frame = false;
	}
	public   void take_damage(int damage ) {
		if (!i_frame)
		{
			i_frame = true;
			health -= damage;
			instance.StartCoroutine(instance.i_frame_delay());


			if (health < 1 )
			{
				alive = false;
				death();
			}
		}

		UI_Controller.current_instance.update_health_text();
		UI_Controller.current_instance.update_gear_health();
	}
	
	public   bool take_health_up(int health_up)
	{
		if (health >= max_health )
		{
			UI_Controller.current_instance.update_health_text();
			UI_Controller.current_instance.update_gear_health();
			return false;
		}
		else
		{
			health= Mathf.Min(max_health,health_up + health ) ;//clamp
			UI_Controller.current_instance.update_health_text();
			UI_Controller.current_instance.update_gear_health();
			return true;
		}       
	}

	[ClientRpc]
	public void death_ClientRpc()
	{        
		NetworkManager.Singleton.Shutdown();
		Destroy(NetworkManager.Singleton.gameObject);
		Destroy(Room_controller.instance.gameObject);

		SceneManager.LoadScene("Start_menu", LoadSceneMode.Single);
	}


	[ServerRpc(RequireOwnership = false)]
	public void increase_num_dead_ServerRpc()
	{
		num_dead.Value += 1;
	}

    [ServerRpc(RequireOwnership = false)]
    public void increase_num_at_menu_ServerRpc()
    {
        num_at_menu.Value += 1;
    }

    [ServerRpc(RequireOwnership = false)]
	public void check_if_all_dead_ServerRpc()
	{
		if (num_dead.Value == NetworkManager.Singleton.ConnectedClients.Count)
		{
			Debug.Log("Time to go to end screen");
			NetworkManager.Singleton.SceneManager.LoadScene("End_screen", LoadSceneMode.Single);
			//go_to_end_screen_ServerRpc();
		}
	}

	[ServerRpc]
	public void go_to_end_screen_ServerRpc()
	{
		NetworkManager.Singleton.SceneManager.LoadScene("End_screen", LoadSceneMode.Single);
	}

	//this is to kill a server

	[ServerRpc(RequireOwnership = false)]
	public void despawn_player_ServerRpc(ulong clientID)
	{
		NetworkManager.Singleton.ConnectedClients[clientID].PlayerObject.GetComponent<NetworkObject>().Despawn();   
	}

	public void kill_player()
	{

		increase_num_dead_ServerRpc();
		ulong clientID = NetworkManager.Singleton.LocalClientId;
		despawn_player_ServerRpc(clientID);
		check_if_all_dead_ServerRpc();
		//may not need this
		//ulong clientID = NetworkManager.Singleton.LocalClientId;
		//despawn_player_ServerRpc(clientID);
		//foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
		//{
		//	if (player.GetComponent<NetworkObject>().IsOwner)
		//	{
		//		clientID;
		//	}
		//}

		//GameObject.Find("Player(Clone)").GetComponent<NetworkObject>().Despawn(); 

		//    foreach (var a in NetworkManager.Singleton.ConnectedClients)
		//    {

		//        if ((a.Value.PlayerObject.GetComponent<NetworkObject>().IsOwner == true))
		//        {
		//a.Value.PlayerObject.GetComponent<NetworkObject>().Despawn();
		//            //a.Value.PlayerObject.transform.position = new Vector3(999, 999, 0);
		//        }
		//    }

		//gameObject.GetComponent<NetworkObject>().Despawn();

	}

	

	//[ServerRpc(RequireOwnership =false)]
	//public void go_to_main_menu_ServerRpc()
	//{
	//	NetworkManager.Singleton.SceneManager.LoadScene("Start_menu", LoadSceneMode.Single);
	//	go_to_main_menu_ClientRpc();
	//}

		
	[ClientRpc]
	public void go_to_main_menu_ClientRpc()
	{
		//SceneManager.LoadScene("Start_menu", LoadSceneMode.Single);

		NetworkManager.Singleton.SceneManager.LoadScene("Start_menu", LoadSceneMode.Single);
		Debug.Log("Working");
		NetworkManager.Singleton.Shutdown();
		Destroy(NetworkManager.Singleton.gameObject);
		//SceneManager.LoadScene("Start_menu", LoadSceneMode.Single);
		Destroy(Room_controller.instance.gameObject);
		

	}

	public void death()
	{
		health = 20;
		/*
		foreach (var a in NetworkManager.Singleton.ConnectedClients)
		{
			
			if ( ( a.Value.PlayerObject.GetComponent<NetworkObject>().IsOwner == true )&& ! NetworkManager.Singleton.IsServer)
			{
				NetworkManager.Singleton.DisconnectClient(a.Value.ClientId);
				//a.Value.PlayerObject.transform.position = new Vector3(999, 999, 0);
			}
		}*/
		//Debug.Log(a.Value.PlayerObject.transform.position);
		/*
		NetworkManager.Singleton.Shutdown();
		Destroy(NetworkManager.Singleton.gameObject);
		Destroy(Room_controller.instance.gameObject);
		
		NetworkManager.Singleton.SceneManager.LoadScene("Start_menu", LoadSceneMode.Single);
		*/
		//SceneManager.LoadScene("Start_room");
		
		kill_player(); 


	}

   
	private void update_coin_text_on_change(int prev, int next)
	{
		UI_Controller.current_instance.update_coin_text();
	}


	public void increase_coin_num(int i)
	{
		coin_num.Value += i;
	}

 
	
	public void decrease_coin_num(int i)
	{
		decrease_coin_num_ServerRpc(i);
	}

	[ServerRpc (RequireOwnership = false)]
	public void decrease_coin_num_ServerRpc(int i)
	{
		coin_num.Value -= i;
	}

	public void inc_max_health(int mx_health)
	{

		max_health += mx_health;
		UI_Controller.current_instance.update_gear_health();

	}
	public void inc_health(int in_health)
	{
		health += in_health;
		UI_Controller.current_instance.update_health_text();
		UI_Controller.current_instance.update_gear_health();
	}
}

