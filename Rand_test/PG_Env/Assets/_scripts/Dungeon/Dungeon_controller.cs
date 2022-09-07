using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine.Rendering;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine.SocialPlatforms;

public static class GLOBAL
{
	public const int GRID_SIZE = 10;
	public const int GRID_SIZE_X = 6;
	public const int GRID_SIZE_Y = 5;
}


//singleton
public class Dungeon_controller : NetworkBehaviour
{
	public static Dungeon_controller instance;
	public Floor current_floor;
	public bool created;
	public int init_vector;

	Color light_grey = new Color(0.7f, 0.7f, 0.7f, 1f);
	Color dark = new Color(0.05f, 0.05f, 0.05f, 1f);

	[SerializeField] Texture2D texture_atlas;
	float grid_size_x;
	float grid_size_y;
	int block_len_x;
	int block_len_y;
	(int, int) boss_room;
	(int, int) item_room;

	int line_thickness;
	public Texture2D texture;
	public Dictionary<(int,int),string> special = new Dictionary<(int, int), string>();
	public Dictionary<(int, int), bool> cleaned = new Dictionary<(int, int), bool>();

	private System.Random rng_sorter = new System.Random();
	private System.Random rng_special_rooms = new System.Random();


	// Start is called before the first frame update
	private void Awake()
	{
		init_vector = Random.Range(int.MinValue,int.MaxValue);
		created = false;
	}

	[ServerRpc]
	public void gen_map_ServerRpc()
	{
		//Debug.Log("server sent rpc");
		gen_map_ClientRpc(Random.Range(int.MinValue, int.MaxValue));
	}

	[ClientRpc]
	public void gen_map_ClientRpc(int my_seed)
	{
		//Debug.Log("clietn got rpc");
		rng_special_rooms = new System.Random(my_seed);
		current_floor = new Floor(my_seed);
		StartCoroutine(gen_map());
	}
	void Start()
	{
		if (instance == null)
		{
			instance = this;
			DontDestroyOnLoad(this.gameObject);
		}		
		rng_sorter = new System.Random(init_vector);

		texture = new Texture2D(972, 972, TextureFormat.ARGB32, false);
		line_thickness = 3;
		if (IsServer)
		{
			gen_map_ServerRpc();
		}
		grid_size_x = GLOBAL.GRID_SIZE_X;
		grid_size_y = GLOBAL.GRID_SIZE_Y;

		//block_len corresponds to one pixel in 12x12 texture atlas
		block_len_x = (int)((texture.width / grid_size_x) - line_thickness) / 3;
		block_len_y = (int)((texture.width / grid_size_y) - line_thickness) / 3;

		draw_grid();

	}
	IEnumerator gen_map()
	{
		//Debug.Log("clietn got  gen_map rpc");
		while (true)
		{
			current_floor.start_collapse();
			while (current_floor.next_collapse())
			{
				yield return new WaitForSeconds(0.0001f);
			}
			if (current_floor.validate())
			{
				break;
			}
			current_floor.reset_floor();
		}
		draw_current_floor();
		//
		find_boss_room();
		find_item_room();
		texture.Apply();
		created = true;
	}

	private float add_small_rand()
	{
		return rng_special_rooms.Next(-1000000000, 1000000000) / 100000000000.0f;
	}
	
	public (int,int) find_boss_room()
	{
		(int, int) ret;
		SortedList<float, (int, int)> sosted = new SortedList<float, (int, int)>() ;
		foreach ((int,int) rm in current_floor.reachable_map_tree)
		{
			float dist = Mathf.Abs(current_floor.any_node_from_max_tree.x_cord - rm.Item1) + Mathf.Abs(current_floor.any_node_from_max_tree.y_cord - rm.Item2)+ add_small_rand(); // menheten distance 
			sosted[dist] = rm;
			//sosted.Add(dist, rm);
		}		
		ret = sosted.Values[sosted.Count-1];
		draw_boss_room(ret.Item1, ret.Item2);

		//Debug.Log(ret);
		boss_room = ret;
		special.Add(ret , "bossroom");
		return ret;
	}
	public (int, int) find_item_room()
	{
		(int, int) ret;
		SortedList<float, (int, int)> sosted = new SortedList<float, (int, int)>();
		foreach ((int, int) rm in current_floor.reachable_map_tree)
		{
			float dist = Mathf.Abs(current_floor.any_node_from_max_tree.x_cord - rm.Item1) +
				Mathf.Abs(current_floor.any_node_from_max_tree.y_cord - rm.Item2) +
				Mathf.Abs(boss_room.Item1 - rm.Item1) + Mathf.Abs(boss_room.Item2 - rm.Item2) + add_small_rand() ; // menheten distance 
			if (rm != boss_room)
			{
				sosted[dist] = rm;
			}
		}
		
		ret = sosted.Values[sosted.Count - 1];
		draw_boss_room(ret.Item1, ret.Item2);

		//Debug.Log("item_room" + ret);
		item_room = ret;
		special.Add(ret, "itemroom");
		return ret;
	}

	// Update is called once per frame
	void Update()
	{

	}

	void draw_grid()
	{
		for (int x = 0; x < texture.width; x++)
		{
			for (int y = 0; y < texture.height; y++)
			{
				if (x % (texture.width / grid_size_x) < 1 * line_thickness)
				{
					texture.SetPixel(x, y, light_grey);
				}
				else if (y % (texture.height / grid_size_y) < 1 * line_thickness)
				{
					texture.SetPixel(x, y, light_grey);
				}
				else if (x > texture.width - line_thickness)
				{
					texture.SetPixel(x, y, light_grey);
				}
				else if (y > texture.height - line_thickness)
				{
					texture.SetPixel(x, y, light_grey);
				}
				else
				{
					texture.SetPixel(x, y, dark);
				}
			}
		}
	}

	//drawing minigrid room no is for chunk coordinates and room is for global length
	void draw_room(int room_x, int room_y, int room_no_x, int room_no_y)
	{
		for (int x = 0; x < texture_atlas.width / 4; x++)
		{
			for (int y = 0; y < texture_atlas.height / 4; y++)
			{
				Color[] src_colors = new Color[block_len_x * block_len_y];
				for (int i = 0; i < block_len_x * block_len_y; i++)
				{
					src_colors[i] = texture_atlas.GetPixel(room_x * 3 + x, room_y * 3 + y);
				}
				int strt_x = x * block_len_x + line_thickness + (int)(room_no_x * (texture.width / grid_size_x));
				int strt_y = y * block_len_y + line_thickness + (int)(room_no_y * (texture.height / grid_size_y));

				texture.SetPixels(strt_x, strt_y, block_len_x, block_len_y, src_colors);
			}
		}

		texture.Apply();

		// connect texture to material of GameObject this script is attached to

	}

	public void draw_current_floor()
	{
		foreach (var t in current_floor.floor_data)
		{
			if (t.Value.collapsed)
			{
				draw_room(current_floor.tile_map_coord[(t.Value.value)].Item1, current_floor.tile_map_coord[(t.Value.value)].Item2, t.Value.x_cord, t.Value.y_cord);
			}
		}
	}

	//this is used for minimap
	public Texture2D draw_player_copy_texture(int room_x, int room_y)
	{        		
		Texture2D my_texture = new Texture2D(972, 972, TextureFormat.ARGB32, false);
		Graphics.CopyTexture(texture, my_texture);
		//ind for indicator
		int ind_x = (int)(my_texture.width / grid_size_x);
		int ind_y = (int)(my_texture.width / grid_size_y);
		//room_x will be the current room's x in map coordinates
		//times threee because three pixels correspond to one edge 
		for (int x = ind_x * room_x; x < ind_x * room_x + ind_x; x++)
		{
			for (int y = ind_y * room_y; y < ind_y * room_y + ind_y; y++)
			{
				if ((x < ind_x * room_x + line_thickness * 5) || (x > ind_x * room_x + ind_x - line_thickness * 5))
				{
					my_texture.SetPixel(x, y, Color.red);
				}
				if ((y < ind_y * room_y + line_thickness * 5) || (y > ind_y * room_y + ind_y - line_thickness * 5))
				{
					my_texture.SetPixel(x, y, Color.red);

				}
			}
		}
		my_texture.Apply();
		return my_texture;
	}

	//chunk coordinates are room_x, room_y it was room_no in the previous one.
	public void draw_boss_room (int room_x, int room_y)
	{
		//ind for indicator
		int ind_x = (int)(texture.width / grid_size_x);
		int ind_y = (int)(texture.width / grid_size_y);
		//room_x will be the current room's x in map coordinates
		//times threee because three pixels correspond to one edge 
		//for loop below it will paint from  the start of the corresponding chunk till the end of the chunk
		for (int x = ind_x * room_x; x < ind_x * room_x + ind_x; x++)
		{
			for (int y = ind_y * room_y; y < ind_y * room_y + ind_y; y++)
			{
				//so that it won't draw on grid's thickness lines (the black contours around the chunks)

				// offset necessary because otherwise since these are drawn considering local coordinates
				//the logic of the minimap made via local coordinates assumes map isn't scaled and when it
				//is scaled it is distorted
				float local_x = x - ind_x * room_x - ind_x/2;
				float local_y = y - ind_y * room_y - ind_y/2;
				local_x /= rconfig.rx;
				local_y /= rconfig.ry;
				//local_y /= 2;
				//if x is inside line thickness
				//intersection of lines, there are images, code is self explanatory
				//transforming chunk room coordinates to its local coordinates for intersection checking (x+y<C) stuff

				if (!(x < ind_x * room_x + line_thickness * 10 ) &&  !(x > ind_x * room_x + ind_x - line_thickness * 10))
				{
					if ((int)Mathf.Abs(local_x - local_y) < line_thickness || (int)Mathf.Abs(local_x + local_y) < line_thickness)
					{
						texture.SetPixel(x, y, new Color(Random.Range(0,255)/255.0f, Random.Range(0, 255) / 255.0f, Random.Range(0,255)/255.0f) );
						//Debug.Log("local x : " + local_x + " , local y " + local_y);

					}
				}



				
				if (! (y < ind_y * room_y + line_thickness * 5) || ! (y > ind_y * room_y + ind_y - line_thickness * 5))
				{
					
				}
			}


		}
		
	}
}
