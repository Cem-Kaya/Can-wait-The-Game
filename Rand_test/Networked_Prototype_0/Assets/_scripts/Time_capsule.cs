using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class Time_capsule : NetworkBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    [ServerRpc(RequireOwnership = false)]
    public void despawn_all_players_ServerRpc()
    {
        {
            foreach (var a in NetworkManager.Singleton.ConnectedClientsList)
            {
                if (a.PlayerObject != null)
                {
                    a.PlayerObject.GetComponent<NetworkObject>().Despawn();
                }
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void go_to_win_screen_ServerRpc()
    {
        {
            foreach (var a in NetworkManager.Singleton.ConnectedClientsList)
            {
                NetworkManager.Singleton.SceneManager.LoadScene("Win_Screen", LoadSceneMode.Single);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Entered collision");
        if (other.tag == "Player")
        {
            Debug.Log("inif");
            despawn_all_players_ServerRpc();
            go_to_win_screen_ServerRpc();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
