using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class tmp_spawner : NetworkBehaviour
{
    public GameObject PrefabToSpawn;
    public bool DestroyWithSpawner;
    private GameObject m_PrefabInstance;
    private NetworkObject m_SpawnedNetworkObject;

    private IEnumerator timed_spawner()
	{
		while (true)
		{
			yield return new WaitForSeconds(10);
            			

			// Instantiate the GameObject Instance
			m_PrefabInstance = Instantiate(PrefabToSpawn);

			// Optional, this example applies the spawner's position and rotation to the new instance
			m_PrefabInstance.transform.position = transform.position;
			m_PrefabInstance.transform.rotation = transform.rotation;

			// Get the instance's NetworkObject and Spawn
			m_SpawnedNetworkObject = m_PrefabInstance.GetComponent<NetworkObject>();
			m_SpawnedNetworkObject.Spawn();


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
        if (!enabled || PrefabToSpawn == null)
        {
            return;
        }
        //StartCoroutine(timed_spawner());
    }

    public override void OnNetworkDespawn()
    {
        if (IsServer && DestroyWithSpawner && m_SpawnedNetworkObject != null && m_SpawnedNetworkObject.IsSpawned)
        {
            m_SpawnedNetworkObject.Despawn();
        }
        base.OnNetworkDespawn();
    }
}




