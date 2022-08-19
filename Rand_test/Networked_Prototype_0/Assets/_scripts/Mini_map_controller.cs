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
                //Debug.Log("waiting for map fail ");
            }
        }
        GetComponent<RawImage>().material.mainTexture = Dungeon_controller.instance.texture;
        //when the gameobject is disabled the courotine is also removed from scheduler but the code in the middle of execution
        //continues until the end;
        this.gameObject.SetActive(false);
        this.gameObject.SetActive(true);
        this.gameObject.GetComponent<CanvasRenderer>().SetAlpha(0);

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
