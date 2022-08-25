using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Colection_script : NetworkBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("naem :" + other.tag +  "Am I server?" + IsServer.ToString());
        
        if (other.tag == "Player")
        {
            Debug.Log("I am here right?");
            if (IsServer)
            {
                Player_controller.instance.increase_coin_num(1);
                //my_box_mover = other.gameObject.GetComponent<box_mover>();
                //my_box_mover.coin_num++;
                //Destroy(gameObject);
                
                Debug.Log(gameObject.GetComponent<NetworkObject>());
                gameObject.GetComponent<NetworkObject>().Despawn();
            }

        }
    }
    
}
