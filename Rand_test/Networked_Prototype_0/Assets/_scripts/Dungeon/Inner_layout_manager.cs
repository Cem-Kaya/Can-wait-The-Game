using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.VisualScripting;
using System.Drawing;


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
	Dictionary<(int, int), string> type_grid = new Dictionary<(int, int), string>();

    public GameObject dirt_prefab;
    public GameObject grass_prefab;
	public GameObject rock_prefab;
	
    public GameObject coin_prefab;
	
	private System.Random rng = new System.Random();
    //rng for priorityqueue in sorter
	private System.Random rng_sorter = new System.Random();

	Room_info tmp_inf = new Room_info();
    door_dir my_type = new door_dir();

    private int reduction_amount;


    private void Awake()
    {
        reduction_amount = 90;
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
        
        tmp_inf = Room_controller.instance.current_room_info;
		rng = new System.Random(Dungeon_controller.instance.init_vector * tmp_inf.x*42 + Dungeon_controller.instance.init_vector * tmp_inf.y* 68 );
		rng_sorter = new System.Random(Dungeon_controller.instance.init_vector * tmp_inf.x * 42 + Dungeon_controller.instance.init_vector * tmp_inf.y * 68);
		my_type = Dungeon_controller.instance.current_floor.floor_data[(tmp_inf.x, tmp_inf.y)].value;
		
		if ( ! Dungeon_controller.instance.special.ContainsKey((tmp_inf.x, tmp_inf.y)))
        {
            inside = new Floor(rng.Next(), rconfig.rx, rconfig.ry);
			//this and clientrpc allows the host and client to be synchronized
			if (IsServer) StartCoroutine(lay_out_layout());
        }


    }

    // Update is called once per frame
    void Update()
    {

    }


   

	private float add_small_rand()
    {
        return rng_sorter.Next(-1000000000, 1000000000) / 100000000000.0f;
	}
    //lays out the layout aka put stuf in place ! This is inner room layout by the way
    IEnumerator lay_out_layout()
    {        
		while (true)
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
			//inside.print_status();
			yield return new WaitForSeconds(0.0001f);


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
            if (validate_grid())
            {
				draw_grid();
				break;
            }			
            reset_grid();
			yield return new WaitForEndOfFrame();
		}

        // do coin stuff here
        layout_coins();
	}


    private void layout_coins() //literally the opposite of the table in clean grid function
    { 
		Dictionary<(int, int), int> table = new Dictionary<(int, int), int>();
		for (int i = 0; i < rconfig.rx * 3; i++)
		{
			for (int j = 0; j < rconfig.ry * 3; j++)
			{
				if (grid[(i, j)])
				{
					table[(i, j)] =0;
				}
				else
				{
					table[(i, j)] = 1;
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
					//i take an element from my grid, if it is not seen before and in table there should be something in my table, 
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
					table[(i, j)]--;
				}
			}
		}
		/*
        //printing out the table
		for (int j = rconfig.ry * 3 - 1; j >= 0; j--)
		{
			string tmp_string = $"r {j - 1}: ";
			for (int i = 0; i < rconfig.rx * 3; i++)
			{
				tmp_string += $" { table[(i, j)]  } ";
			}
			Debug.Log(tmp_string);
		}
		*/
		List<(int, int)> tmp_list = new List<(int, int)>();
        int cur_max = -1;
		for (int i = 0; i < rconfig.rx * 3; i++)
		{			
			for (int j = 0; j < rconfig.ry * 3; j++)
			{
				if (table[(i, j)] > cur_max)
				{
					cur_max = table[(i, j)];
					tmp_list = new List<(int, int)>();
					tmp_list.Add((i, j));
				}
				else if (table[(i, j)] ==cur_max)
                {
                    tmp_list.Add((i, j));
                }
			}
		}
        /////////////////////////////////

		int num_coins = rng.Next(0, 5);

        for (int i = 0; i < num_coins; i++)
        {
            (int, int) coord = tmp_list[rng.Next(0, tmp_list.Count)] ; 
            float tmp_x = coord.Item1 * grid_len_x / 3 - room_len_x / 2;
            float tmp_y = coord.Item2 * grid_len_y / 3 - room_len_y / 2;
            layout_coins_ClientRpc(tmp_x, tmp_y);
        }
	}


    [ClientRpc]
    public void layout_coins_ClientRpc(float tmp_x , float tmp_y )
    {       
        GameObject coin = Instantiate(coin_prefab, new Vector3(tmp_x, tmp_y, 0), Quaternion.identity);
        coin.GetComponent<NetworkObject>().Spawn();
    }
	
    public void reset_grid()
    {
		rng.Next();
        rng_sorter.Next();
		inside.reset_floor();
		for (int i = 0; i < rconfig.rx * 3; i++)
		{
			for (int j = 0; j < rconfig.ry * 3; j++)
			{
				grid[(i, j)] = false;
			}
		}
		
	}

    private bool astar((int, int) start, (int, int) end)
    {
        //this is used to make sure one can access all doors int the room, that rocks wont prevent players from going to certain places
        //PriorityQueue<string, int> queue = new PriorityQueue<string, int>();
        //in case of 2 coordinates with same cost+heuristics, a random float is added and this will act as the randomness as well, since
        //one will be 5.000000001 and other will be 5.00000002 and other be 4.99999999
        SortedList<float, (int, int)> slist = new SortedList<float, (int, int)>();
        Dictionary<(int, int), bool> scene = new Dictionary<(int, int), bool>();

        (int, int) current = start;
        slist.Add(Mathf.Abs(end.Item1 - current.Item1 + end.Item2 - current.Item2) + add_small_rand(), current);
        int step = 0;
		
		if(!scene.ContainsKey(current))
		{
			scene.Add(current, true);
		}

		while (slist.Count > 0)
        {
            step++;
            current = slist.Values[0];
            slist.RemoveAt(0);
            //Debug.Log(step + ": " + current);
            if (current == end)
            {
                return true;
            }

            (int, int) up = (current.Item1, current.Item2 + 1), down = (current.Item1, current.Item2 - 1), left = (current.Item1 - 1, current.Item2), right = (current.Item1 + 1, current.Item2);
            //as these new nodes are added to the priority queue the lowest during the while loop the algorithm will always continue until
            //in the lowest cost path the current node is equal to the end node. we also check for !grid[up] since player can't move through rocks

            if (grid.ContainsKey(up) && !grid[up])
            {
                if (!scene.ContainsKey(up))
                {
                    scene.Add(up, true);
                    slist.Add(Mathf.Abs(end.Item1 - up.Item1 + end.Item2 - up.Item2) + add_small_rand(), up);
                }
            }
            if (grid.ContainsKey(left) && !grid[left])
            {
                if (!scene.ContainsKey(left))
                {
                    scene.Add(left, true);
                    slist.Add(Mathf.Abs(end.Item1 - left.Item1 + end.Item2 - left.Item2) + add_small_rand(), left);
                }
            }
            if (grid.ContainsKey(down) && !grid[down])
            {
                if (!scene.ContainsKey(down))
                {
                    scene.Add(down, true);
                    slist.Add(Mathf.Abs(end.Item1 - down.Item1 + end.Item2 - down.Item2) + add_small_rand(), down);
                }
            }
            if (grid.ContainsKey(right) && !grid[right])
            {
                if (!scene.ContainsKey(right))
                {
                    scene.Add(right, true);
                    slist.Add(Mathf.Abs(end.Item1 - right.Item1 + end.Item2 - right.Item2) + add_small_rand(), right);
                }
            }

        }

        return false;

    }

    private bool validate_grid()
    {
        //checks if doors are connected to each other with astar algorithm, if not the grid will be reset
        List<(int, int)> doors = new List<(int, int)>();
        //start_x and start_y represent coordinaes for the door on center top
        if (Dungeon_controller.instance.current_floor.up_connection.Contains(my_type)) {
            int start_x = (int)Mathf.Floor(rconfig.rx / 2.0f) * 3;
            int start_y = rconfig.ry * 3 - 1;
            doors.Add((start_x, start_y));
        }
        if (Dungeon_controller.instance.current_floor.down_connection.Contains(my_type))
        {
            int start_x = (int)Mathf.Floor(rconfig.rx / 2.0f) * 3;
            int start_y = 0;
            doors.Add((start_x, start_y));
        }
        if (Dungeon_controller.instance.current_floor.right_connection.Contains(my_type))
        {
            int start_x = rconfig.rx * 3 - 1;
            int start_y = (int)Mathf.Floor(rconfig.ry / 2.0f) * 3;
            doors.Add((start_x, start_y));
        }
        if (Dungeon_controller.instance.current_floor.left_connection.Contains(my_type))
        {
            int start_x = 0;
            int start_y = (int)Mathf.Floor(rconfig.ry / 2.0f) * 3;
            doors.Add((start_x, start_y));
        }

        for (int i = 1; i < doors.Count; i++)
        {
            if (!astar(doors[0], doors[i]))
            {
                Debug.Log("validation for door connections failed inner layout is reset");
                return false;
            }
        }  		
		return true;
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
					int tmpy = rconfig.ry * 3 - j-1 ;					
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
					int tmpx = rconfig.rx * 3 - i - 1;
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
				if ((table.ContainsKey((i+1 , j)) ? table[(i + 1, j)] >= 1: true  )&& 
                    (table.ContainsKey((i-1, j)) ? table[(i - 1, j)] >= 1 : true  )&& 
                    (table.ContainsKey((i, j+1)) ? table[(i, j + 1)] >= 1 : true )&& 
                    (table.ContainsKey((i, j-1)) ? table[(i, j - 1)] >= 1:  true )&&
                     table[(i, j)] == 0
                    )
                {
                    //Debug.Log((i, j));
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
                    //i take an element from my grid, if it is not seen before and in table there should be something in my table, 
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
                //tmp_string += $" { table[(i, j)]  } ";
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
		// adding rocks and stuff 
		for (int i = 0; i < rconfig.rx * 3; i++)
		{
			for (int j = 0; j < rconfig.ry * 3; j++)
			{
				if (grid[(i, j)])
				{
                    int num_neighbour = 0 ;
                    if (table.ContainsKey((i, j)))
                    {
                        (int, int) up = (i, j + 1), down = (i, j - 1), left = (i - 1, j), right = (i + 1, j);
                        //if it is a valid grid coordinate and there is anything there, num of neighbour is up.
                        if (grid.ContainsKey(up) && grid[up])
                        {
                            num_neighbour++;
                        }
                        if (grid.ContainsKey(left) && grid[left])
                        {
                            num_neighbour++;
                        }
                        if (grid.ContainsKey(down) && grid[down])
                        {
                            num_neighbour++;
                        }
                        if (grid.ContainsKey(right) && grid[right])
                        {
                            num_neighbour++;
                        }

                        if (num_neighbour == 4)
                        {
                            type_grid[(i, j)] = "rock";
                        }
                        else if (num_neighbour == 2)
                        {
                            type_grid[(i, j)] = "grass";
                        }
                        else
                        {
                            type_grid[(i, j)] = "dirt";
                        }
                    }
				}
			}
		}


	}

    [ClientRpc]
    public void draw_grid_ClientRpc(float tmp_x, float tmp_y, float grid_len_x, float grid_len_y, string type)
    {
        if (type == "dirt")
        {
            GameObject boulder = Instantiate(dirt_prefab, new Vector3(tmp_x, tmp_y, 0), Quaternion.identity);
            boulder.transform.localScale = new Vector3(grid_len_x / 3, grid_len_y / 3, 1);
        }else if (type == "rock")
        {
            GameObject boulder = Instantiate(rock_prefab, new Vector3(tmp_x, tmp_y, 0), Quaternion.identity);
            boulder.transform.localScale = new Vector3(grid_len_x / 3, grid_len_y / 3, 1);
        }
        else if (type == "grass")
        {
            GameObject boulder = Instantiate(grass_prefab, new Vector3(tmp_x, tmp_y, 0), Quaternion.identity);
            boulder.transform.localScale = new Vector3(grid_len_x / 3, grid_len_y / 3, 1);
        }
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
					draw_grid_ClientRpc(tmp_x, tmp_y, grid_len_x, grid_len_y, type_grid[(i, j)]);
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
			NW_prob -= reduction_amount;
			NE_prob -= reduction_amount;
		}
		
        if (!inside.down_connection.Contains(type))
        {
            chunk[(1, 0)] = true;
        }else
        {
			SW_prob -= reduction_amount;
			SE_prob -= reduction_amount;
		}
		if (!inside.left_connection.Contains(type))
        {
            chunk[(0, 1)] = true;
        }
	    else
        {
			NW_prob -= reduction_amount;
			SW_prob -= reduction_amount;
		}
        if (!inside.right_connection.Contains(type))
        {
            chunk[(2, 1)] = true;
        }
		else
		{
			NE_prob -= reduction_amount;
			SE_prob -= reduction_amount;
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




