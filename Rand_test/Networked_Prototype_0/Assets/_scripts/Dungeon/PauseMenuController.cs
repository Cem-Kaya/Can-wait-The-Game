using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
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
        menu = player_input_actions.Menu.Escape;
        menu.Enable();
        menu.performed += Pause; //whenever this action is performed Pause function is called
    }

    private void OnDisable()
    {
        menu.Disable();
    }

    void Pause(InputAction.CallbackContext context)
    {
        is_paused = !is_paused;

        if (is_paused)
        {
            Activate_menu();
        }
        else
        {
            Deactivate_menu();
        }
    }

    void Activate_menu()
    {
        Time.timeScale = 0;
        AudioListener.pause = true;
        pause_ui.SetActive(true);
    }
    public void Deactivate_menu()
    {
        Time.timeScale = 1;
        AudioListener.pause = false;
        pause_ui.SetActive(false);
        is_paused = false;
    }

    public void Go_to_start_menu()
    {
        Time.timeScale = 1;
        AudioListener.pause = false;
        pause_ui.SetActive(false);
        is_paused = false;
        SceneManager.LoadScene("Start_menu", LoadSceneMode.Single);
        Player_controller.instance.health = 10; //farkli bir yolu vardir belki birde save game olayini halletmeliyiz
    }

    public void End_game()
    {
        Application.Quit();
    }

}

