using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Unity.Netcode;

using UnityEngine.Rendering;
public class LobbyMenuController : NetworkBehaviour
{
    [SerializeField] private GameObject start_game_button;
    private Network_manager_config my_config;
    
    // Start is called before the first frame update
    void Start()
    {
        if (IsServer)
        {
            start_game_button.SetActive(true);
        }

    }
    // Update is called once per frame
    void Update()
    {

    }


    public void Start_room_button()
    {
        NetworkManager.Singleton.SceneManager.LoadScene("Start_room", LoadSceneMode.Single);
        
    }
}

