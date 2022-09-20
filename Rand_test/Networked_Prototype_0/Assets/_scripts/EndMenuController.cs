
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Unity.Netcode;

using UnityEngine.Rendering;


public class EndMenuController : NetworkBehaviour
{
    private Player_input_actions player_input_actions;
    private InputAction menu;


    [SerializeField] private GameObject pause_ui;
    [SerializeField] private GameObject main_menu_layout;

    private IEnumerator network_destroyer_delay()
    {
        while (Player_controller.instance.num_at_menu.Value != NetworkManager.Singleton.ConnectedClients.Count-1)
        {
            Debug.Log(Player_controller.instance.num_at_menu.Value);
            yield return null;
        }
        Debug.Log(Player_controller.instance.num_at_menu.Value);
        

        NetworkManager.Singleton.Shutdown();
        Destroy(NetworkManager.Singleton.gameObject);
        Destroy(Room_controller.instance.gameObject);
        
    }


    private void Awake()
    {
       
        DebugManager.instance.enableRuntimeUI = false;
        player_input_actions = new Player_input_actions();
        
    }

    private IEnumerator network_manager_shutdown()
    {
        Player_controller.instance.increase_num_at_menu_ServerRpc();


        yield return new WaitForSeconds(3);
        NetworkManager.Singleton.Shutdown();
        Destroy(NetworkManager.Singleton.gameObject); 
        Destroy(Room_controller.instance.gameObject);

    }

    // Start is called before the first frame update
    void Start()
    {
        
        
        if (!IsHost) StartCoroutine(network_manager_shutdown());
        if (IsServer)
        {
            StartCoroutine(network_destroyer_delay());
        }
        

    }

    // Update is called once per frame
    void Update()
    {

    }


    private void OnEnable()
    {

    }

    private void OnDisable()
    {
        //menu.Disable();
    }



    public void Return_to_main_menu()
    {
        //go to main menu
        SceneManager.LoadScene("Start_menu", LoadSceneMode.Single);
    }

    

    public void End_game()
    {
        Application.Quit();
    }

    public void OnDestroy()
    {
        GameObject destroyable= GameObject.Find("Room_controller");
        if(destroyable != null)
        {
            Destroy(destroyable);
        }
    }

}
