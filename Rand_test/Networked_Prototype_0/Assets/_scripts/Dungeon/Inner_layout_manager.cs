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
    float room_len_x;
    float room_len_y;
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
        room_len_x = 29f;
		room_len_y = 15f;

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
				draw_section(i, j, room_len_x / rconfig.rx, room_len_y / rconfig.ry, inside.floor_data[(i, j)].value);
			}		
			yield return new WaitForSeconds(0.0001f);
		}
	}

    private void draw_section(int xpos , int ypos, float grid_len_x , float grid_len_y , door_dir type  )
    {
		Dictionary<(int, int), bool> chunk = new Dictionary<(int, int), bool>();
        for(int i = 0; i < 3; i++)
        {
			for (int j = 0; j < 3; j++)
			{
				chunk[(i, j)] = false;
			}
		}
		if (  inside.up_connection.Contains(type))
        {
			chunk[(1, 2)] = true;
		}
		if (inside.down_connection.Contains(type))
		{
			chunk[(1, 0)] = true;
		}
		if (inside.left_connection.Contains(type))
		{
			chunk[(0, 1)] = true;
		}
		if (inside.right_connection.Contains(type))
		{
			chunk[(2, 1)] = true;
		}
		foreach(var ch in chunk)
        {
            if (true) //ch.Value
            {
                float tmp_x = xpos * grid_len_x + ch.Key.Item1 * grid_len_x / 3 - room_len_x / 2;
                float tmp_y = ypos * grid_len_y + ch.Key.Item2 * grid_len_y / 3 - room_len_y / 2;
				GameObject boulder = Instantiate(rock_prefab, new Vector3(tmp_x, tmp_y, 0), Quaternion.identity);
				boulder.transform.localScale  = new Vector3(grid_len_x/3, grid_len_y/3, 1);
			}
		}
	}
}




