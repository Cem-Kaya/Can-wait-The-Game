using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


//my chunk represents my

public class rconfig
{
	//represents the total grid not an individual room, think of them as max
    //corresponds to number of tiles on axis
	public const int rx = 5;
    public const int ry = 3;
}

public class Inner_layout_manager : NetworkBehaviour
{
    //here is the thing, chunks are 3*5, grids represent all blocks of a chunk. a picture explaining it is
    //in the folder check that photo as you can see each chunk when divided to 9 parts represents one tile and can be used
    //to make all tiles
    // Start is called before the first frame update
    Floor inside;
    float room_len_x;
    float room_len_y;
	float grid_len_x;
    float grid_len_y;
    bool created;
    Dictionary<(int, int), bool> grid= new Dictionary<(int, int), bool> () ;
    public GameObject rock_prefab;

	private void Awake()
    {	
        created = false;
		room_len_x = 29f;
		room_len_y = 15f;
        //grid len corresponds to length of one tile
        //grid's coordinates are in world coordinates, chunk in tile coordinates, 
        grid_len_x = room_len_x / rconfig.rx;
        grid_len_y = room_len_y / rconfig.ry;

        for (int i = 0; i < rconfig.rx*3 ; i++)
        {
			for (int j = 0; j < rconfig.ry*3 ; j++)
			{
                grid[(i, j)] = false;
			}
		}
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
        //tile coordinates' world coordinates. if we wanted world coordinates we would've multiplied by 3
        for(int i = 0; i < rconfig.rx ; i++)
        {
            for(int j = 0; j < rconfig.ry ; j++)
            {
				add_section(i, j, inside.floor_data[(i, j)].value);
			}	
		}
        draw_grid();
	}

    private void draw_grid ()
    {
		for (int i = 0; i < rconfig.rx * 3; i++)
		{
			for (int j = 0; j < rconfig.ry* 3; j++)
			{				
				if (grid[(i,j)]) //ch.Value
				{
					float tmp_x = i * grid_len_x/3  - room_len_x / 2;
					float tmp_y = j * grid_len_y/3 - room_len_y / 2;
					GameObject boulder = Instantiate(rock_prefab, new Vector3(tmp_x, tmp_y, 0), Quaternion.identity);
					boulder.transform.localScale = new Vector3(grid_len_x / 3, grid_len_y / 3, 1);
				} 
			}
		}
	}

    private void add_section(int xpos , int ypos, door_dir type  )
    {
		Dictionary<(int, int), bool> chunk = new Dictionary<(int, int), bool>();
        for(int i = 0; i < 3; i++)
        {
			for (int j = 0; j < 3; j++)
			{
				chunk[(i, j)] = false;
			}
		}
		if ( ! inside.up_connection.Contains(type))
        {
			chunk[(1, 2)] = true;
		}
		if (! inside.down_connection.Contains(type))
		{
			chunk[(1, 0)] = true;
		}
		if (! inside.left_connection.Contains(type))
		{
			chunk[(0, 1)] = true;
		}
		if ( ! inside.right_connection.Contains(type))
		{
			chunk[(2, 1)] = true;
		}
		foreach(var ch in chunk)
        {
			grid[ ( 3 * xpos + ch.Key.Item1, 3 * ypos + ch.Key.Item2)  ] = true;
		}
	}
}




