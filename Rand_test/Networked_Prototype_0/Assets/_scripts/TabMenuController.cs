using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class TabMenuController : MonoBehaviour
{
    private Player_input_actions player_input_actions;
    private InputAction menu;
    
    [SerializeField] private GameObject tab_ui;

    [SerializeField] private bool tab_ui_on;

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
        menu = player_input_actions.Menu.TabMenu;
        menu.Enable();
        menu.performed += TabMenuOpen; //whenever this action is performed Pause function is called
    }

    private void OnDisable()
    {
        menu.Disable();
    }

    void TabMenuOpen(InputAction.CallbackContext context)
    {
        tab_ui_on = !tab_ui_on;

        if (tab_ui_on)
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
        tab_ui.SetActive(true);
    }
    public void Deactivate_menu()
    {
        Time.timeScale = 1;
        AudioListener.pause = false;
        tab_ui.SetActive(false);
        tab_ui_on = false;
    }

    public void Go_to_start_menu()
    {
        Time.timeScale = 1;
        AudioListener.pause = false;
        tab_ui.SetActive(false);
        tab_ui_on = false;
        SceneManager.LoadScene("Start_menu", LoadSceneMode.Single);
        Player_controller.instance.health = 10; //farkli bir yolu vardir belki birde save game olayini halletmeliyiz
    }

    public void End_game()
    {
        Application.Quit();
    }

}

