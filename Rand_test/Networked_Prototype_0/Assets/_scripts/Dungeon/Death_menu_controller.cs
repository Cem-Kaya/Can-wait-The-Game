
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Death_menu_controller : MonoBehaviour
{
    private Player_input_actions player_input_actions;
    private InputAction menu;

    [SerializeField] private GameObject pause_ui;
    [SerializeField] private bool is_paused;

    private void Awake()
    {
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

    public void Start_game()
    {
        SceneManager.LoadScene("Main_scene", LoadSceneMode.Single);
    }

    void Activate_menu()
    {

    }
    public void Main_menu()
    {
        SceneManager.LoadScene("Start_menu", LoadSceneMode.Single);
        Player_controller.Health = 10; //farkli bir yolu vardir belki birde save game olayini halletmeliyiz
    }

    public void End_game()
    {
        Application.Quit();
    }

    public void Restart_game()
    {
        SceneManager.LoadScene("Main_scene", LoadSceneMode.Single);
        Player_controller.Health = 10; //farkli bir yolu vardir belki birde save game olayini halletmeliyiz
    }
}
