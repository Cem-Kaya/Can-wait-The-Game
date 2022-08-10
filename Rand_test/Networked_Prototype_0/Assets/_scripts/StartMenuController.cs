
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
    [SerializeField] private bool is_paused;

    private void Awake()
    {
        DebugManager.instance.enableRuntimeUI = false;
        player_input_actions = new Player_input_actions();
    }

    // Start is called before the first frame update
    void Start()
    {

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
        
        NetworkManager.Singleton.StartHost();
        NetworkManager.Singleton.SceneManager.LoadScene("Start_room", LoadSceneMode.Single);
    }

    public void Start_game_as_client()
    {
        //m_NetworkManager.StartHost();
        NetworkManager.Singleton.StartClient();
        //NetworkManager.Singleton.SceneManager.LoadScene("Start_room", LoadSceneMode.Single);
    }

    void Activate_menu()
    {

    }
    public void Deactivate_menu()
    {

    }

    public void End_game()
    {
        Application.Quit();
    }

}
