using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class rconfig
{
	//represents the total grid not an individual room, think of them as max
	public const int rx = 5;
    public const int ry = 3;
}

public class Inner_layout_manager : NetworkBehaviour
{
    // Start is called before the first frame update
    Floor inside;
    bool created;

    public GameObject rock_prefab;

	private void Awake()
    {
		created = false;

	}
    void Start()
    {		
        // if (!IsServer) Destroy(gameObject);
		inside = new Floor( 42 , rconfig.rx, rconfig.ry );
        StartCoroutine(gen_layout());
        StartCoroutine(lay_out_layout());
	}

	// Update is called once per frame
	void Update()
    {
        
    }


    IEnumerator gen_layout()
    {
        //Debug.Log("clietn got  gen_map rpc");
        while (true)
        {
			inside.start_collapse();
            while (inside.next_collapse())
            {
                yield return new WaitForSeconds(0.0001f);
            }
            if (inside.validate())
            {
                break;
            }
			inside.reset_floor();
        }   
        created = true;
        inside.print_status();
    }

    //lays out the layout aka put stuf in place ! 
    IEnumerator lay_out_layout()
    {
        while (!created)
        {
            yield return new WaitForEndOfFrame();
        }
        for(int i = 0; i < rconfig.rx; i++)
        {
            for(int j = 0; j < rconfig.ry; j++)
            {
                GameObject boulder = Instantiate(rock_prefab, new Vector3(i,j,0) , Quaternion.identity);
            }
		}
		yield return new WaitForSeconds(0.0001f);
	}
}



}
