using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


//my chunk represents my chunk coordinates (it is 3*5) my grid represents my grid coordinates' world coordinate values
//they are not exactly world coordinates but carry more information, if you recall in texture atlas each chunk was made up of
//3x3 pixels, hence to get all the information needed a grid is needed 3*3 * 5*3 as you recall a 3x3 pixel is needed to convey
//which direction a door is. also our chunk system is 3*3 not 3*5 keep this in mind

//ultimate comment = you see our canvas is 3*5 as it was when generatingthe world. since each of our chunks are made up
//of 3*3 sections, and our grid is 3*3 * 5*3. chunk represents only 1 tile, while grid represents the entire canvas with
//all its possible blocks. my chunks are my tiles

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
    Dictionary<(int, int), bool> grid = new Dictionary<(int, int), bool>();
    public GameObject rock_prefab;
    private System.Random rng = new System.Random();
    Room_info tmp_inf = new Room_info();
    door_dir my_type = new door_dir();




    private void Awake()
    {
		created = false;
        room_len_x = 29f;
        room_len_y = 15f;
        //grid len corresponds to length of one tile
        //grid's coordinates are in world coordinates, chunk in tile coordinates, 
        grid_len_x = room_len_x / rconfig.rx;
        grid_len_y = room_len_y / rconfig.ry;

        for (int i = 0; i < rconfig.rx * 3; i++)
        {
            for (int j = 0; j < rconfig.ry * 3; j++)
            {
                grid[(i, j)] = false;
            }
        }
    }
    void Start()
    {
        // if (!IsServer) Destroy(gameObject);
        tmp_inf = Room_controller.instance.current_room_info;
		rng = new System.Random(Dungeon_controller.instance.init_vector * tmp_inf.x*42 + Dungeon_controller.instance.init_vector * tmp_inf.y* 68 );
		my_type = Dungeon_controller.instance.current_floor.floor_data[(tmp_inf.x, tmp_inf.y)].value;
		
		if ( ! Dungeon_controller.instance.special.ContainsKey((tmp_inf.x, tmp_inf.y)))
        {
            inside = new Floor(rng.Next(), rconfig.rx, rconfig.ry);
            StartCoroutine(gen_layout());
            StartCoroutine(lay_out_layout());
        }


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
        for (int i = 0; i < rconfig.rx; i++)
        {
            for (int j = 0; j < rconfig.ry; j++)
            {
                //info of 9 pixels and it is called 15 times
                add_section(i, j, inside.floor_data[(i, j)].value);
            }
        }
        cleanup_grid();
		draw_grid();
    }
    private void cleanup_grid()
    {
        Dictionary<(int, int), int> table = new Dictionary<(int, int), int>();
        for (int i = 0; i < rconfig.rx * 3; i++)
        {
            for (int j = 0; j < rconfig.ry * 3; j++)
            {
                if (grid[(i, j)])
                {
                    table[(i, j)] = 1;
                }
                else
                {
                    table[(i, j)] = 0;
                }
            }
        }
		if (Dungeon_controller.instance.current_floor.up_connection.Contains(my_type))
		{
			for (int i = 0; i < 3; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					//fixes rocks in front of doors
					int tmpx = i + (int)Mathf.Floor(rconfig.rx / 2.0f) * 3;
					int tmpy = rconfig.ry * 3 - j;
                    table[(tmpx, tmpy)] = 0;
                    //Debug.Log($"{tmpx} {tmpy}");
                }
            }
		}
		if (Dungeon_controller.instance.current_floor.right_connection.Contains(my_type))
		{
			for (int i = 0; i < 3; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					//fixes rocks in front of doors
					int tmpx = rconfig.rx * 3 - i;
					int tmpy = j + (int)Mathf.Floor(rconfig.ry / 2.0f) * 3;
                    table[(tmpx, tmpy)] = 0;
                    //Debug.Log($"{tmpx} {tmpy}");
                }
            }
		}
		if (Dungeon_controller.instance.current_floor.down_connection.Contains(my_type))
		{
			for (int i = 0; i < 3; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					//fixes rocks in front of doors
					int tmpx = i + (int)Mathf.Floor(rconfig.rx / 2.0f) * 3;
					int tmpy = j;
                    table[(tmpx, tmpy)] = 0;
                    //Debug.Log($"{tmpx} {tmpy}");
                }
            }
		}
		if (Dungeon_controller.instance.current_floor.left_connection.Contains(my_type))
		{
			for (int i = 0; i < 3; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					//fixes rocks in front of doors
					int tmpx = i;
					int tmpy = j + (int)Mathf.Floor(rconfig.ry / 2.0f) * 3;
					table[(tmpx, tmpy)] = 0;
					//Debug.Log($"{tmpx} {tmpy}");
				}
			}
		}

		for (int i = 0; i < rconfig.rx * 3; i++)
		{
            //initially there were cases where groups of rocks had gaps, this fixes it 
			for (int j = 0; j < rconfig.ry * 3; j++)
			{
				if (table.ContainsKey((i, j)) && 
                    table.ContainsKey((i+1 , j)) && 
                    table.ContainsKey((i-1, j)) && 
                    table.ContainsKey((i, j+1)) && 
                    table.ContainsKey((i, j-1)) && 
                    table[(i, j)] == 0 && 
                    table[(i+1, j)] >= 1 &&  
                    table[(i -1, j)] >= 1 && 
                    table[(i , j+1)] >= 1 && 
                    table[(i , j-1 )] >= 1)
                {
                    Debug.Log((i, j));
					table[(i, j)] = 1 ;
					grid[(i, j)] = true;
				}

				
			}
		}



		for (int i = 0; i < rconfig.rx * 3; i++)
        {
            for (int j = 0; j < rconfig.ry * 3; j++)
            {
                Dictionary<(int, int), bool> seen = new Dictionary<(int, int), bool>();
                Stack<(int, int)> st = new Stack<(int, int)>();
                if (table[(i, j)] > 0)
                {
                    st.Push((i, j));
                }
                while (st.Count > 0)
                {
                    //i take an element, if it is not seen before and in table there should be something, 
                    //the blocks around the current node are also pushed into the stack, i pop them and check if 
                    //its adjacent nodes are suspposed to have rocks. at the end table will be a matrix that represents
                    //which parts of the grid have rocks and how many rocks are adjacefnt to each other.
                    (int, int) cur = st.Pop();
                    if (!seen.ContainsKey(cur) && table.ContainsKey(cur) && table[cur] > 0)
                    {
                        seen[cur] = true;
                        table[(i, j)] += 1;
                        st.Push((cur.Item1 - 1, cur.Item2));
                        st.Push((cur.Item1 + 1, cur.Item2));
                        st.Push((cur.Item1, cur.Item2 - 1));
                        st.Push((cur.Item1, cur.Item2 + 1));
                    }
                }
				if (table[(i, j)] > 0)
				{
					table[(i, j)] -- ;
				}
			}
        }
        for (int j = rconfig.ry * 3 -1 ; j >= 0 ; j--)
        {
            string tmp_string = $"r {j-1 }: ";
            for (int i = 0; i < rconfig.rx * 3; i++)
            {
                tmp_string += $" { table[(i, j)]  } ";
			}
			//Debug.Log(tmp_string);
		}		
		//rocks with less than 2 adjacent rocks (so rocks with less than a total of 3 connected rocks) will be deleted
		for (int i = 0; i < rconfig.rx * 3; i++)
		{
			for (int j = 0; j < rconfig.ry * 3; j++)
			{
				if (table[(i, j)] < 3 )
				{
					grid[(i , j)] = false;
				}				
			}
		}
		//  door infront clean up 
		
		
    }
 
    private void draw_grid ()
    {
		for (int i = 0; i < rconfig.rx * 3; i++)
		{
			for (int j = 0; j < rconfig.ry* 3; j++)
			{				
				if (grid[(i,j)]) 
				{
					float tmp_x = i * grid_len_x/3  - room_len_x / 2;
					float tmp_y = j * grid_len_y/3 - room_len_y / 2;
					GameObject boulder = Instantiate(rock_prefab, new Vector3(tmp_x, tmp_y, 0), Quaternion.identity);
					boulder.transform.localScale = new Vector3(grid_len_x / 3, grid_len_y / 3, 1);
				} 
			}
		}		

	}

    private void add_section(int xpos, int ypos, door_dir type)
    {
        Dictionary<(int, int), bool> chunk = new Dictionary<(int, int), bool>();
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                chunk[(i, j)] = false;
            }
        }

        float NW_prob = 100;
        float NE_prob = 100;
        float SE_prob = 100; 
        float SW_prob = 100;
			
		if (!inside.up_connection.Contains(type))
        {
            chunk[(1, 2)] = true;
        }
        else
        {
            //if there is a connection to the upper room, the chances of upper right and upper left of the room
            //having a boulder is lowered same with others
			NW_prob -= 40 ;
			NE_prob -= 40 ;
		}
		
        if (!inside.down_connection.Contains(type))
        {
            chunk[(1, 0)] = true;
        }else
        {
			SW_prob -= 40;
			SE_prob -= 40;
		}
		if (!inside.left_connection.Contains(type))
        {
            chunk[(0, 1)] = true;
        }
	    else
        {
			NW_prob -= 40;
			SW_prob -= 40;
		}
        if (!inside.right_connection.Contains(type))
        {
            chunk[(2, 1)] = true;
        }
		else
		{
			NE_prob -= 40;
			SE_prob -= 40;
		}

		chunk[(0, 0)] =  rng.Next(0,101)  >= NW_prob ?true :false  ;
        chunk[(2, 0)] =  rng.Next(0, 101) >= NE_prob ?true :false  ;
        chunk[(0, 2)] =  rng.Next(0, 101) >= SE_prob ?true :false  ;
        chunk[(2, 2)] =  rng.Next(0, 101) >= SW_prob ?true :false  ;
	

		foreach (var ch in chunk)
        {
            if (ch.Value) {
                grid[(3 * xpos + ch.Key.Item1, 3 * ypos + ch.Key.Item2)] = true;
            }
        }
	}
}




