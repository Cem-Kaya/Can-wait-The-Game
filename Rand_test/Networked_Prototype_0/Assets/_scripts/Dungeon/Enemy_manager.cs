using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Enemy_manager : NetworkBehaviour
{
    public List<GameObject> spawnable_prefabs = new List<GameObject>();    

    public bool DestroyWithSpawner;

    private List<GameObject> spawned_enemies = new List<GameObject>();
    
       
    private IEnumerator timed_spawner()
    {
        while (true)
        {
            yield return new WaitForSeconds(3);


            //// Instantiate the GameObject Instance
            //m_PrefabInstance = Instantiate(PrefabToSpawn);
            
           
            foreach (GameObject spawn in spawnable_prefabs)
            {
                GameObject prefab_inst = Instantiate(spawn);
                prefab_inst.transform.position = transform.position;
                prefab_inst.transform.rotation = transform.rotation;
                prefab_inst.GetComponent<NetworkObject>().Spawn();
                spawned_enemies.Add(prefab_inst);
                
            }


            //// Optional, this example applies the spawner's position and rotation to the new instance
            //m_PrefabInstance.transform.position = transform.position;
            //m_PrefabInstance.transform.rotation = transform.rotation;
      

            //// Get the instance's NetworkObject and Spawn
            //m_SpawnedNetworkObject = m_PrefabInstance.GetComponent<NetworkObject>();
            //m_SpawnedNetworkObject.Spawn();



            /*
            GameObject child_amiba = Instantiate(amiba, transform.position + new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), 1), Quaternion.identity);
            GameObject child_amiba2 = Instantiate(amiba, transform.position + new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), 1), Quaternion.identity);
            child_amiba.transform.localScale = new Vector3(transform.localScale.x * 0.7f, transform.localScale.y * 0.7f, transform.localScale.z);
            child_amiba2.transform.localScale = new Vector3(transform.localScale.x * 0.7f, transform.localScale.y * 0.7f, transform.localScale.z);
            child_amiba.GetComponent<NetworkObject>().Spawn();
            child_amiba2.GetComponent<NetworkObject>().Spawn();
            */


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
                Debug.Log("For somre reason prefabs are null");
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
            
            if (IsServer && DestroyWithSpawner && spawned_object.GetComponent<NetworkObject>() != null && spawned_object.GetComponent<NetworkObject>().IsSpawned)
            {
                spawned_object.GetComponent<NetworkObject>().Despawn();
            }
            base.OnNetworkDespawn();
           

        }

        //if (IsServer && DestroyWithSpawner && m_SpawnedNetworkObject != null && m_SpawnedNetworkObject.IsSpawned)
        //{
        //    m_SpawnedNetworkObject.Despawn();
        //}
        //base.OnNetworkDespawn();
    }
}
