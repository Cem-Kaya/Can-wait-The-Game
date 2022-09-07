
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Unity.Netcode;

using UnityEngine.Rendering;


public class StartMenuController : MonoBehaviour
{
    private Player_input_actions player_input_actions;
    private InputAction menu;

    [SerializeField] private GameObject pause_ui;
    [SerializeField] private GameObject main_menu_layout;
    [SerializeField] private GameObject client_menu;
    [SerializeField] private GameObject host_menu;
    [SerializeField] private bool is_paused;

    private ExampleNetworkDiscoveryHud my_hud;
    private Network_manager_config my_config;

    private void Awake()
    {
        DebugManager.instance.enableRuntimeUI = false;
        player_input_actions = new Player_input_actions();
    }

    // Start is called before the first frame update
    void Start()
    {
        my_hud = NetworkManager.Singleton.GetComponent<ExampleNetworkDiscoveryHud>();
        my_config = NetworkManager.Singleton.GetComponent<Network_manager_config>();
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

    public void Start_game_as_host()
    {
        //m_NetworkManager.StartHost();
        main_menu_layout.SetActive(false);
        host_menu.SetActive(true);
        my_config.enabled = true;
        
    }

    public void Start_game_as_host_button()
    {
       

        NetworkManager.Singleton.StartHost();
        NetworkManager.Singleton.SceneManager.LoadScene("Lobby", LoadSceneMode.Single);
        my_config.enabled = false;
    }

    public void Start_game_as_client()
    {
        //m_NetworkManager.StartHost();

        //NetworkManager.Singleton.StartClient();
        if (client_menu.active)
        {
            my_hud.enabled = false;
        }
        else
        {
            main_menu_layout.SetActive(false);
            my_hud.enabled = true;
            client_menu.SetActive(true);

        }
        //NetworkManager.Singleton.SceneManager.LoadScene("Start_room", LoadSceneMode.Single);
    }

    void Activate_menu()
    {

    }
    public void Deactivate_menu()
    {

    }

    public void Return_to_menu_from_client_menu()
    {
        main_menu_layout.SetActive(true);
        my_hud.enabled = false;
        client_menu.SetActive(false);
    }

    public void Return_to_menu_from_host_menu()
    {
        main_menu_layout.SetActive(true);
        host_menu.SetActive(false);
        my_config.enabled = false;
    }

    public void End_game()
    {
        Application.Quit();
    }

}
