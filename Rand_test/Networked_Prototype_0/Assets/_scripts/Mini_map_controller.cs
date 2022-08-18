using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class Mini_map_controller : MonoBehaviour
{
    IEnumerator wait_for_map()
    {
        while (true)
        {
            yield return new WaitForEndOfFrame();
            if (Dungeon_controller.instance.created)
            {
                break;
            }
            else
            {
                Debug.Log("waiting for map fail ");
            }
        }
        GetComponent<RawImage>().material.mainTexture = Dungeon_controller.instance.texture;
    }
	
	// Start is called before the first frame update
	void Start()
    {
        StartCoroutine(wait_for_map() );
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
