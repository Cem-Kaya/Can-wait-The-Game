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
    public int health = 200 ;   
    public int max_health = 200 ; // max health of the player    
    public int fire_delay = 2 ;
    public float move_speed;
    public bool alive;
    public bool i_frame ;
    public float i_frame_sec;

    public NetworkVariable<int> coin_num;


    void Awake()
    {
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

    public   void death()
    {
        health = 10;
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
		
        if(IsServer) death_ClientRpc();
		
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
        coin_num.Value -= i;
    }


}

