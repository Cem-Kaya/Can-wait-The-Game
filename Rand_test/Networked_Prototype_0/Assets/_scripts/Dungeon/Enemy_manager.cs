using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Enemy_manager : NetworkBehaviour
{
    public List<GameObject> spawnable_prefabs = new List<GameObject>();    

    public bool DestroyWithSpawner = true ;

    private List<GameObject> spawned_enemies = new List<GameObject>();
    
       
    private IEnumerator timed_spawner()
    {
        while (true)
        {
            yield return new WaitForSeconds(6);


            //// Instantiate the GameObject Instance
            //m_PrefabInstance = Instantiate(PrefabToSpawn);
            
           
            foreach (GameObject spawn in spawnable_prefabs)
            {
                GameObject prefab_inst = Instantiate(spawn);
                prefab_inst.transform.position = new Vector3 ( Random.Range(-12,12), Random.Range(-5, 5), 0);
				
                prefab_inst.transform.rotation = transform.rotation;
                prefab_inst.GetComponent<NetworkObject>().Spawn();
                spawned_enemies.Add(prefab_inst);
                
            }




        }
    }

    public override void OnNetworkSpawn()
    {
        // Only the server spawns, clients will disable this component on their side
        
        
        enabled = IsServer;
        foreach (var spawn in spawnable_prefabs)
        {
            if (!enabled || spawn == null)
            {
                if (IsServer) Debug.Log("For somre reason prefabs are null");
                return;
            }

        }

        
        StartCoroutine(timed_spawner());
    }

    public void Start()
    {       
             
              
        //StartCoroutine(timed_spawner());
    }         
              
    public override void OnNetworkDespawn()
    {       
        foreach (GameObject spawned_object in spawned_enemies)
        {   
            //Debug.Log("Got in OnNetworkDespawn");
            
            if (IsServer && DestroyWithSpawner &&(spawned_object != null ) && spawned_object.GetComponent<NetworkObject>() != null && spawned_object.GetComponent<NetworkObject>().IsSpawned)
            {
                //Debug.Log("Got in OnNetworkDespawn's if clause");
                spawned_object.GetComponent<NetworkObject>().Despawn();
            }
            base.OnNetworkDespawn();
            
        }   
    }
}
