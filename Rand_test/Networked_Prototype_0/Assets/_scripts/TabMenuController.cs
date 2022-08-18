using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class TabMenuController : MonoBehaviour
{
    private CanvasRenderer rend;
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
        rend = tab_ui.GetComponent<CanvasRenderer>();
        rend.SetAlpha(0);
        Debug.Log(rend.ToString());
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
            Debug.Log("Turned on");
        }
        else
        {
            Deactivate_menu();
            Debug.Log("Turned off");
        }
    }

    void Activate_menu()
    {
        //tab_ui.SetActive(true);
        rend.SetAlpha(1);
    }
    public void Deactivate_menu()
    {
        rend.SetAlpha(0);
        //tab_ui.SetActive(false);
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

