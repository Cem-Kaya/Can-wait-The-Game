using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;



public static class GLOBAL
{
	public static int GRID_SIZE = 10;
	public static int GRID_SIZE_X = GRID_SIZE;
	public static int GRID_SIZE_Y = GRID_SIZE;
} 

public enum door_dir
{
	blank,
	u,
	r,
	d,
	l,
	ur,
	rd,
	dl,
	lu,
	rl,
	ud,
	urd,
	rdl,
	dlu,
	lur,
	urdl
}

public class Tile
{
	public Tile(int x, int y)
	{
		collapsed = false;
		possibles = new List<door_dir> { door_dir.blank,
													door_dir.u,
													door_dir.r,
													door_dir.d,
													door_dir.l,
													door_dir.ur,
													door_dir.rd,
													door_dir.dl,
													door_dir.lu,
													door_dir.rl,
													door_dir.ud,
													door_dir.urd,
													door_dir.rdl,
													door_dir.dlu,
													door_dir.lur,
													door_dir.urdl };
		value = door_dir.blank;
		x_cord = x;
		y_cord = y;
		fill_table();
	}

	private static Dictionary<door_dir, int> prob_weights = new Dictionary<door_dir, int>();
	private void fill_table()
	{
		if (!(prob_weights.Count > 0))
		{
			prob_weights.Add(door_dir.blank, 4);
			prob_weights.Add(door_dir.u, 2);
			prob_weights.Add(door_dir.r, 2);
			prob_weights.Add(door_dir.d, 2);
			prob_weights.Add(door_dir.l, 2);
			prob_weights.Add(door_dir.ur, 8);
			prob_weights.Add(door_dir.rd, 8);
			prob_weights.Add(door_dir.dl, 8);
			prob_weights.Add(door_dir.lu, 8);
			prob_weights.Add(door_dir.rl, 10);
			prob_weights.Add(door_dir.ud, 8);
			prob_weights.Add(door_dir.urd, 15);
			prob_weights.Add(door_dir.rdl, 15);
			prob_weights.Add(door_dir.dlu, 15);
			prob_weights.Add(door_dir.lur, 15);
			prob_weights.Add(door_dir.urdl, 20);
		}
	}

	public int x_cord, y_cord;
	public bool collapsed;
	public List<door_dir> possibles;
	public int possible_num { get { return possibles.Count; } }
	public door_dir value;
	public bool try_collapse()
	{
		if (possible_num > 0)
		{
			List<door_dir> select_from = new List<door_dir>();
			foreach(door_dir dd in possibles)
			{
				for (int i = 0; i < prob_weights[dd] ; i++)
				{
					select_from.Add(dd);
				}
			}
			
			value = select_from[UnityEngine.Random.Range(0, select_from.Count)];
			possibles.Clear();
			collapsed = true;
			return true;
		}
		
		return false;
	}

	public void propogate(List<door_dir> removables)
	{
		foreach (door_dir d in removables)
		{
			possibles.Remove(d);
		}
	}
}
public class Floor
{
	int max_x, max_y;

	List<door_dir> up_connection = new List<door_dir>();
    List<door_dir> down_connection = new List<door_dir>();
    List<door_dir> left_connection = new List<door_dir>();
    List<door_dir> right_connection = new List<door_dir>();

	public int max_tree_size;
    public Tile any_node_from_max_tree;

    public Dictionary<door_dir, (int, int)> tile_map_coord = new Dictionary<door_dir, (int, int)>();

	public Dictionary<(int, int), Tile> floor_data = new Dictionary<(int, int), Tile>();

	Dictionary<(door_dir, string), List<door_dir>> rules = new Dictionary<(door_dir, string), List<door_dir>>();
	public void fill_tables()
	{

		rules.Add((door_dir.blank, "u"), new List<door_dir> { door_dir.d, door_dir.urdl, door_dir.rdl, door_dir.dl, door_dir.ud, door_dir.dlu, door_dir.rd, door_dir.urd });
		rules.Add((door_dir.blank, "r"), new List<door_dir> { door_dir.l, door_dir.urdl, door_dir.rdl, door_dir.dl, door_dir.lu, door_dir.dlu, door_dir.rl, door_dir.lur });
		rules.Add((door_dir.blank, "d"), new List<door_dir> { door_dir.u, door_dir.urdl, door_dir.lur, door_dir.lu, door_dir.ud, door_dir.dlu, door_dir.ur, door_dir.urd });
		rules.Add((door_dir.blank, "l"), new List<door_dir> { door_dir.r, door_dir.urdl, door_dir.rdl, door_dir.rl, door_dir.ur, door_dir.lur, door_dir.rd, door_dir.urd });

		//ones without a d in them
		rules.Add((door_dir.u, "u"), new List<door_dir> { door_dir.r, door_dir.u, door_dir.l, door_dir.ur, door_dir.rl, door_dir.blank, door_dir.lu, door_dir.lur });
		rules.Add((door_dir.u, "r"), new List<door_dir> { door_dir.l, door_dir.lu, door_dir.lur, door_dir.urdl, door_dir.dl, door_dir.dlu, door_dir.rdl, door_dir.rl });
		rules.Add((door_dir.u, "d"), new List<door_dir> { door_dir.u, door_dir.lu, door_dir.lur, door_dir.urdl, door_dir.ud, door_dir.dlu, door_dir.lur, door_dir.ur, door_dir.urd });
		rules.Add((door_dir.u, "l"), new List<door_dir> { door_dir.r, door_dir.ur, door_dir.lur, door_dir.urdl, door_dir.rd, door_dir.urd, door_dir.rdl, door_dir.rl });

		//ones with a d in them
		rules.Add((door_dir.d, "u"), new List<door_dir> { door_dir.d, door_dir.ud, door_dir.urd, door_dir.urdl, door_dir.dl, door_dir.dlu, door_dir.rdl, door_dir.rd });
		//ones with a l in them
		rules.Add((door_dir.d, "r"), new List<door_dir> { door_dir.l, door_dir.lu, door_dir.lur, door_dir.urdl, door_dir.dl, door_dir.dlu, door_dir.rdl, door_dir.rl });
		//ones without u in them
		rules.Add((door_dir.d, "d"), new List<door_dir> { door_dir.r, door_dir.d, door_dir.l, door_dir.rd, door_dir.rl, door_dir.blank, door_dir.dl, door_dir.rdl });
		//ones with a r in them
		rules.Add((door_dir.d, "l"), new List<door_dir> { door_dir.r, door_dir.ur, door_dir.lur, door_dir.urdl, door_dir.rd, door_dir.urd, door_dir.rdl, door_dir.rl });


		//ones with a d in them
		rules.Add((door_dir.r, "u"), new List<door_dir> { door_dir.d, door_dir.ud, door_dir.urd, door_dir.urdl, door_dir.dl, door_dir.dlu, door_dir.rdl, door_dir.rd });
		//ones without l in them
		rules.Add((door_dir.r, "r"), new List<door_dir> { door_dir.r, door_dir.d, door_dir.u, door_dir.rd, door_dir.ur, door_dir.blank, door_dir.ud, door_dir.urd });
		//ones with u in them
		rules.Add((door_dir.r, "d"), new List<door_dir> { door_dir.u, door_dir.lu, door_dir.lur, door_dir.urdl, door_dir.ud, door_dir.dlu, door_dir.lur, door_dir.ur, door_dir.urd });
		//ones with a r in them
		rules.Add((door_dir.r, "l"), new List<door_dir> { door_dir.r, door_dir.ur, door_dir.lur, door_dir.urdl, door_dir.rd, door_dir.urd, door_dir.rdl, door_dir.rl });

		//ones with a d in them
		rules.Add((door_dir.l, "u"), new List<door_dir> { door_dir.d, door_dir.ud, door_dir.urd, door_dir.urdl, door_dir.dl, door_dir.dlu, door_dir.rdl, door_dir.rd });
		//ones with a l in them
		rules.Add((door_dir.l, "r"), new List<door_dir> { door_dir.l, door_dir.lu, door_dir.lur, door_dir.urdl, door_dir.dl, door_dir.dlu, door_dir.rdl, door_dir.rl });
		//ones with u in them
		rules.Add((door_dir.l, "d"), new List<door_dir> { door_dir.u, door_dir.lu, door_dir.lur, door_dir.urdl, door_dir.ud, door_dir.dlu, door_dir.lur, door_dir.ur, door_dir.urd });
		//ones without r in them
		rules.Add((door_dir.l, "l"), new List<door_dir> { door_dir.l, door_dir.d, door_dir.u, door_dir.dl, door_dir.lu, door_dir.blank, door_dir.ud, door_dir.dlu });


		//ones without d in them
		rules.Add((door_dir.ur, "u"), new List<door_dir> { door_dir.r, door_dir.u, door_dir.l, door_dir.ur, door_dir.rl, door_dir.blank, door_dir.lu, door_dir.lur });
		//ones without l in them
		rules.Add((door_dir.ur, "r"), new List<door_dir> { door_dir.r, door_dir.d, door_dir.u, door_dir.rd, door_dir.ur, door_dir.blank, door_dir.ud, door_dir.urd });
		//ones with u in them
		rules.Add((door_dir.ur, "d"), new List<door_dir> { door_dir.u, door_dir.lu, door_dir.lur, door_dir.urdl, door_dir.ud, door_dir.dlu, door_dir.lur, door_dir.ur, door_dir.urd });
		//ones with r in them
		rules.Add((door_dir.ur, "l"), new List<door_dir> { door_dir.r, door_dir.ur, door_dir.lur, door_dir.urdl, door_dir.rd, door_dir.urd, door_dir.rdl, door_dir.rl });

		//ones without d in them
		rules.Add((door_dir.ud, "u"), new List<door_dir> { door_dir.r, door_dir.u, door_dir.l, door_dir.ur, door_dir.rl, door_dir.blank, door_dir.lu, door_dir.lur });
		//ones with l in them
		rules.Add((door_dir.ud, "r"), new List<door_dir> { door_dir.l, door_dir.lu, door_dir.lur, door_dir.urdl, door_dir.dl, door_dir.dlu, door_dir.rdl, door_dir.rl });
		//ones without u in them
		rules.Add((door_dir.ud, "d"), new List<door_dir> { door_dir.r, door_dir.d, door_dir.l, door_dir.rd, door_dir.rl, door_dir.blank, door_dir.dl, door_dir.rdl });
		//ones with r in them
		rules.Add((door_dir.ud, "l"), new List<door_dir> { door_dir.r, door_dir.ur, door_dir.lur, door_dir.urdl, door_dir.rd, door_dir.urd, door_dir.rdl, door_dir.rl });


		//ones with d in them
		rules.Add((door_dir.rd, "u"), new List<door_dir> { door_dir.d, door_dir.ud, door_dir.urd, door_dir.urdl, door_dir.dl, door_dir.dlu, door_dir.rdl, door_dir.rd });
        //ones without l in them
        rules.Add((door_dir.rd, "r"), new List<door_dir> { door_dir.r, door_dir.d, door_dir.u, door_dir.rd, door_dir.ur, door_dir.blank, door_dir.ud, door_dir.urd });
		//ones without u in them
		rules.Add((door_dir.rd, "d"), new List<door_dir> { door_dir.r, door_dir.d, door_dir.l, door_dir.rd, door_dir.rl, door_dir.blank, door_dir.dl, door_dir.rdl });
		//ones with r in them
		rules.Add((door_dir.rd, "l"), new List<door_dir> { door_dir.r, door_dir.ur, door_dir.lur, door_dir.urdl, door_dir.rd, door_dir.urd, door_dir.rdl, door_dir.rl });

		//ones with d in them
		rules.Add((door_dir.dl, "u"), new List<door_dir> { door_dir.d, door_dir.ud, door_dir.urd, door_dir.urdl, door_dir.dl, door_dir.dlu, door_dir.rdl, door_dir.rd });
		//ones with l in them
		rules.Add((door_dir.dl, "r"), new List<door_dir> { door_dir.l, door_dir.lu, door_dir.lur, door_dir.urdl, door_dir.dl, door_dir.dlu, door_dir.rdl, door_dir.rl });
		//ones without u in them
		rules.Add((door_dir.dl, "d"), new List<door_dir> { door_dir.r, door_dir.d, door_dir.l, door_dir.rd, door_dir.rl, door_dir.blank, door_dir.dl, door_dir.rdl });
		//ones without r in them
		rules.Add((door_dir.dl, "l"), new List<door_dir> { door_dir.l, door_dir.d, door_dir.u, door_dir.dl, door_dir.lu, door_dir.blank, door_dir.ud, door_dir.dlu });


		//ones without d in them
		rules.Add((door_dir.lu, "u"), new List<door_dir> { door_dir.r, door_dir.u, door_dir.l, door_dir.ur, door_dir.rl, door_dir.blank, door_dir.lu, door_dir.lur });
		//ones with l in them
		rules.Add((door_dir.lu, "r"), new List<door_dir> { door_dir.l, door_dir.lu, door_dir.lur, door_dir.urdl, door_dir.dl, door_dir.dlu, door_dir.rdl, door_dir.rl });
		//ones with u in them
		rules.Add((door_dir.lu, "d"), new List<door_dir> { door_dir.u, door_dir.lu, door_dir.lur, door_dir.urdl, door_dir.ud, door_dir.dlu, door_dir.lur, door_dir.ur, door_dir.urd });
		//ones without r in them
		rules.Add((door_dir.lu, "l"), new List<door_dir> { door_dir.l, door_dir.d, door_dir.u, door_dir.dl, door_dir.lu, door_dir.blank, door_dir.ud, door_dir.dlu });

		//ones with d in them
		rules.Add((door_dir.rl, "u"), new List<door_dir> { door_dir.d, door_dir.ud, door_dir.urd, door_dir.urdl, door_dir.dl, door_dir.dlu, door_dir.rdl, door_dir.rd });
		//ones without l in them
		rules.Add((door_dir.rl, "r"), new List<door_dir> { door_dir.r, door_dir.d, door_dir.u, door_dir.rd, door_dir.ur, door_dir.blank, door_dir.ud, door_dir.urd });
		//ones with u in them
		rules.Add((door_dir.rl, "d"), new List<door_dir> { door_dir.u, door_dir.lu, door_dir.urdl, door_dir.ud, door_dir.dlu, door_dir.lur, door_dir.ur, door_dir.urd });
		//ones without r in them
		rules.Add((door_dir.rl, "l"), new List<door_dir> { door_dir.l, door_dir.d, door_dir.u, door_dir.dl, door_dir.lu, door_dir.blank, door_dir.ud, door_dir.dlu });

		//ones without d in them
		rules.Add((door_dir.urd, "u"), new List<door_dir> { door_dir.r, door_dir.u, door_dir.l, door_dir.ur, door_dir.rl, door_dir.blank, door_dir.lu, door_dir.lur });
		//ones without l in them
		rules.Add((door_dir.urd, "r"), new List<door_dir> { door_dir.r, door_dir.d, door_dir.u, door_dir.rd, door_dir.ur, door_dir.blank, door_dir.ud, door_dir.urd });
		//ones without u in them
		rules.Add((door_dir.urd, "d"), new List<door_dir> { door_dir.r, door_dir.d, door_dir.l, door_dir.rd, door_dir.rl, door_dir.blank, door_dir.dl, door_dir.rdl });
		//ones with r in them
		rules.Add((door_dir.urd, "l"), new List<door_dir> { door_dir.r, door_dir.ur, door_dir.lur, door_dir.urdl, door_dir.rd, door_dir.urd, door_dir.rdl, door_dir.rl });

		//ones with d in them
		rules.Add((door_dir.rdl, "u"), new List<door_dir> { door_dir.d, door_dir.ud, door_dir.urd, door_dir.urdl, door_dir.dl, door_dir.dlu, door_dir.rdl, door_dir.rd });
		//ones without l in them
		rules.Add((door_dir.rdl, "r"), new List<door_dir> { door_dir.r, door_dir.d, door_dir.u, door_dir.rd, door_dir.ur, door_dir.blank, door_dir.ud, door_dir.urd });
		//ones without u in them
		rules.Add((door_dir.rdl, "d"), new List<door_dir> { door_dir.r, door_dir.d, door_dir.l, door_dir.rd, door_dir.rl, door_dir.blank, door_dir.dl, door_dir.rdl });
		//ones without r in them
		rules.Add((door_dir.rdl, "l"), new List<door_dir> { door_dir.l, door_dir.d, door_dir.u, door_dir.dl, door_dir.lu, door_dir.blank, door_dir.ud, door_dir.dlu });


		//ones without d in them
		rules.Add((door_dir.dlu, "u"), new List<door_dir> { door_dir.r, door_dir.u, door_dir.l, door_dir.ur, door_dir.rl, door_dir.blank, door_dir.lu, door_dir.lur });
		//ones with l in them
		rules.Add((door_dir.dlu, "r"), new List<door_dir> { door_dir.l, door_dir.lu, door_dir.lur, door_dir.urdl, door_dir.dl, door_dir.dlu, door_dir.rdl, door_dir.rl });
		//ones without u in them
		rules.Add((door_dir.dlu, "d"), new List<door_dir> { door_dir.r, door_dir.d, door_dir.l, door_dir.rd, door_dir.rl, door_dir.blank, door_dir.dl, door_dir.rdl });
		//ones without r in them
		rules.Add((door_dir.dlu, "l"), new List<door_dir> { door_dir.l, door_dir.d, door_dir.u, door_dir.dl, door_dir.lu, door_dir.blank, door_dir.ud, door_dir.dlu });

		//ones without d in them
		rules.Add((door_dir.lur, "u"), new List<door_dir> { door_dir.r, door_dir.u, door_dir.l, door_dir.ur, door_dir.rl, door_dir.blank, door_dir.lu, door_dir.lur });
		//ones without l in them
		rules.Add((door_dir.lur, "r"), new List<door_dir> { door_dir.r, door_dir.d, door_dir.u, door_dir.rd, door_dir.ur, door_dir.blank, door_dir.ud, door_dir.urd });
		//ones with u in them
		rules.Add((door_dir.lur, "d"), new List<door_dir> { door_dir.u, door_dir.lu, door_dir.lur, door_dir.urdl, door_dir.ud, door_dir.dlu, door_dir.lur, door_dir.ur, door_dir.urd });
		//ones without r in them
		rules.Add((door_dir.lur, "l"), new List<door_dir> { door_dir.l, door_dir.d, door_dir.u, door_dir.dl, door_dir.lu, door_dir.blank, door_dir.ud, door_dir.dlu });

		//ones without d in them
		rules.Add((door_dir.urdl, "u"), new List<door_dir> { door_dir.r, door_dir.u, door_dir.l, door_dir.ur, door_dir.rl, door_dir.blank, door_dir.lu, door_dir.lur });
		//ones without l in them
		rules.Add((door_dir.urdl, "r"), new List<door_dir> { door_dir.r, door_dir.d, door_dir.u, door_dir.rd, door_dir.ur, door_dir.blank, door_dir.ud, door_dir.urd });
		//ones without u in them
		rules.Add((door_dir.urdl, "d"), new List<door_dir> { door_dir.r, door_dir.d, door_dir.l, door_dir.rd, door_dir.rl, door_dir.blank, door_dir.dl, door_dir.rdl });
		//ones without r in them
		rules.Add((door_dir.urdl, "l"), new List<door_dir> { door_dir.l, door_dir.d, door_dir.u, door_dir.dl, door_dir.lu, door_dir.blank, door_dir.ud, door_dir.dlu });

		//initialize tile_map_coord
		tile_map_coord.Add(door_dir.blank, (3, 0));
		tile_map_coord.Add(door_dir.u, (3, 1));
		tile_map_coord.Add(door_dir.r, (1, 0));
		tile_map_coord.Add(door_dir.d, (2, 0));
		tile_map_coord.Add(door_dir.l, (0, 0));
		tile_map_coord.Add(door_dir.ur, (2, 2));
		tile_map_coord.Add(door_dir.rd, (3, 2));
		tile_map_coord.Add(door_dir.dl, (0, 1));
		tile_map_coord.Add(door_dir.lu, (1, 2));
		tile_map_coord.Add(door_dir.rl, (2, 1));
		tile_map_coord.Add(door_dir.ud, (1, 1));
		tile_map_coord.Add(door_dir.urd, (2, 3));
		tile_map_coord.Add(door_dir.rdl, (3, 3));
		tile_map_coord.Add(door_dir.dlu, (0, 2));
		tile_map_coord.Add(door_dir.lur, (1, 3));
		tile_map_coord.Add(door_dir.urdl, (0, 3));
		//////////////////////////////////////////////
		//up connection initialization
		up_connection = new List<door_dir> { door_dir.u, door_dir.lu, door_dir.urdl, door_dir.ud, door_dir.dlu, door_dir.lur, door_dir.ur, door_dir.urd };
		//down connection initialization
		down_connection = new List<door_dir> { door_dir.d, door_dir.ud, door_dir.urd, door_dir.urdl, door_dir.dl, door_dir.dlu, door_dir.rdl, door_dir.rd };
		right_connection = new List<door_dir> { door_dir.r, door_dir.ur, door_dir.lur, door_dir.urdl, door_dir.rd, door_dir.urd, door_dir.rdl, door_dir.rl };
		left_connection = new List<door_dir>  { door_dir.l, door_dir.lu, door_dir.lur, door_dir.urdl, door_dir.dl, door_dir.dlu, door_dir.rdl, door_dir.rl };
	}
	public Floor()
	{
		fill_tables();		
		max_x = GLOBAL.GRID_SIZE;
		max_y = GLOBAL.GRID_SIZE;
      
		for (int i = 0; i < max_x; i++)
		{
			for (int j = 0; j < max_y; j++)
			{
				floor_data[(i, j)] = new Tile(i, j);
			}
		}
        max_tree_size = -1;
    }
	//when the generated too small it is reset and generated again
	public void reset_floor()
	{
		
		max_x = GLOBAL.GRID_SIZE;
		max_y = GLOBAL.GRID_SIZE;
	
		for (int i = 0; i < max_x; i++)
		{
			for (int j = 0; j < max_y; j++)
			{
				floor_data[(i, j)] = new Tile(i, j);
			}
		}
		max_tree_size = -1;
		any_node_from_max_tree = null;
	}
	
	public Tile get_min_enthropy()
	{
		Tile min = null;
		int current_min = int.MaxValue;
		foreach (var item in floor_data)
		{
			if ((!item.Value.collapsed) && (item.Value.possible_num > 0) && (item.Value.possible_num < current_min))
			{
				min = item.Value;
			}
		}
		if (min == null) { 	return null;}

		List<KeyValuePair<(int,int), Tile>> subList = floor_data.Where( item  => (item.Value.possible_num == min.possible_num)).ToList();

        var ret = subList[UnityEngine.Random.Range(0,subList.Count)].Value ;
		//Debug.Log( "the tile with min enthropy has " + ret.possible_num + "and is " + ret.x_cord + " , " + ret.y_cord);
		return ret;
	}

	public void adjust_corners()
	{
		//top row
		for (int i = 0; i < max_x; i++)
		{
			floor_data[(i, max_y - 1)].propogate(new List<door_dir> { door_dir.u, door_dir.lu, door_dir.lur, door_dir.urdl, door_dir.ud, door_dir.dlu, door_dir.lur, door_dir.ur, door_dir.urd });
		}
		//bottom row
		for (int i = 0; i < max_x; i++)
		{
			floor_data[(i, 0)].propogate(new List<door_dir> { door_dir.d, door_dir.ud, door_dir.urd, door_dir.urdl, door_dir.dl, door_dir.dlu, door_dir.rdl, door_dir.rd });
		}
		//right column
		for (int j = 0; j < max_y; j++)
		{
			floor_data[(max_x - 1, j)].propogate(new List<door_dir> { door_dir.r, door_dir.ur, door_dir.lur, door_dir.urdl, door_dir.rd, door_dir.urd, door_dir.rdl, door_dir.rl });
		}
		//left column                                                           
		for (int j = 0; j < max_y; j++)
		{
			floor_data[(0, j)].propogate(new List<door_dir> { door_dir.l, door_dir.lu, door_dir.lur, door_dir.urdl, door_dir.dl, door_dir.dlu, door_dir.rdl, door_dir.rl });
		}
	}

	public void print_enthropy()
	{
		for (int j = max_x - 1; j >= 0; j--)
		{
			string tmp = $"r {j}:  ";
			for (int i = 0; i < max_y; i++)
			{
				tmp += $" {floor_data[(i, j)].possible_num}";
			}
			Debug.Log(tmp);
		}

	}
	public void start_collapse()
	{
		adjust_corners();
		Tile one = get_min_enthropy();
		if (one.try_collapse())
		{
			//checks the tile on the right
			if (floor_data.ContainsKey((one.x_cord + 1, one.y_cord)))
			{
				List<door_dir> curr_rule_list = rules[(one.value, "r")];
				floor_data[(one.x_cord + 1, one.y_cord)].propogate(curr_rule_list);
			}
			//checks the tile on the left
			if (floor_data.ContainsKey((one.x_cord - 1, one.y_cord)))
			{
				List<door_dir> curr_rule_list = rules[(one.value, "l")];
				floor_data[(one.x_cord - 1, one.y_cord)].propogate(curr_rule_list);
			}
			//checks the tile on the up
			if (floor_data.ContainsKey((one.x_cord, one.y_cord + 1)))
			{
				List<door_dir> curr_rule_list = rules[(one.value, "u")];
				floor_data[(one.x_cord, one.y_cord + 1)].propogate(curr_rule_list);
			}
			//checks the tile on the down
			if (floor_data.ContainsKey((one.x_cord, one.y_cord - 1)))
			{
				List<door_dir> curr_rule_list = rules[(one.value, "d")];
				floor_data[(one.x_cord, one.y_cord - 1)].propogate(curr_rule_list);
			}
			
			//print_enthropy();
		}
		else
		{
			Debug.Log("can not find a solution ");
		}
	}

	public bool next_collapse()
	{
		Tile one = get_min_enthropy();
		if (one == null) { return false; }
		if (one.try_collapse())
		{
			//checks the tile on the right
			if (floor_data.ContainsKey((one.x_cord + 1, one.y_cord)))
			{
				List<door_dir> curr_rule_list = rules[(one.value, "r")];
				floor_data[(one.x_cord + 1, one.y_cord)].propogate(curr_rule_list);
			}
			//checks the tile on the left
			if (floor_data.ContainsKey((one.x_cord - 1, one.y_cord)))
			{
				List<door_dir> curr_rule_list = rules[(one.value, "l")];
				floor_data[(one.x_cord - 1, one.y_cord)].propogate(curr_rule_list);
			}
			//checks the tile on the up
			if (floor_data.ContainsKey((one.x_cord, one.y_cord + 1)))
			{
				List<door_dir> curr_rule_list = rules[(one.value, "u")];
				floor_data[(one.x_cord, one.y_cord + 1)].propogate(curr_rule_list);
			}
			//checks the tile on the down
			if (floor_data.ContainsKey((one.x_cord, one.y_cord - 1)))
			{
				List<door_dir> curr_rule_list = rules[(one.value, "d")];
				floor_data[(one.x_cord, one.y_cord - 1)].propogate(curr_rule_list);
			}
			
			//print_enthropy();
			return true;
		}
		else
		{
			Debug.Log("can not find a solution ");
			return false;
		}

	}

	//this function is utilized to get the biggest map out of all generated maps
	public bool validate()
	{
		Dictionary<(int, int), bool> visited  = new Dictionary<(int, int), bool>();
		for (int j = max_y - 1; j >= 0; j--)
		{
			for (int i = 0; i < max_x; i++)
			{
				visited[(i, j)] = false;
			}
		}

		//		
		foreach(var tile in floor_data)
		{
			Tile node = tile.Value;
			Stack<Tile> stack = new Stack<Tile>();
			if (!visited[(node.x_cord, node.y_cord)])
			{
				stack.Push(node);
			}		
			
			int this_tree_size=0;
			while (stack.Count > 0)
			{				
				Tile this_node = stack.Pop();
				
				visited[(this_node.x_cord, this_node.y_cord)] = true;

				this_tree_size++;
				if (this_tree_size > max_tree_size)
				{
					max_tree_size = this_tree_size;
					any_node_from_max_tree = this_node;
				}

				if (up_connection.Contains(this_node.value))
				{
					Tile next_node = floor_data[(this_node.x_cord, this_node.y_cord + 1)];
					if (!stack.Contains(next_node)) {
						if (!visited[(next_node.x_cord, next_node.y_cord)])
						{
							stack.Push(next_node);
						}
					}
					
				}
                if (right_connection.Contains(this_node.value))
                {
						Tile next_node = floor_data[(this_node.x_cord + 1, this_node.y_cord)];
                    if (!stack.Contains(next_node)) {

                        if (!visited[(next_node.x_cord, next_node.y_cord)])
						{
							stack.Push(next_node);
						}
					}

                }
                if (left_connection.Contains(this_node.value))
                {
                    Tile next_node = floor_data[(this_node.x_cord - 1, this_node.y_cord)];
					if (!stack.Contains(next_node)) {

						if (!visited[(next_node.x_cord, next_node.y_cord)])
						{
							stack.Push(next_node);
						}
					}

                }
                if (down_connection.Contains(this_node.value))
                {
                    Tile next_node = floor_data[(this_node.x_cord, this_node.y_cord-1)];
					if (!stack.Contains(next_node)) {

						if (!visited[(next_node.x_cord, next_node.y_cord)])
						{
							stack.Push(next_node);
						}
					}

                }
            }
		}
		//Debug.Log("max tree size:" + max_tree_size);
		//Debug.Log("any node from max tree: " + any_node_from_max_tree.x_cord + " " + any_node_from_max_tree.y_cord);
		if (max_tree_size > GLOBAL.GRID_SIZE_X* GLOBAL.GRID_SIZE_Y /2)
		{		
			return true;
		} 
		return false;
	}

}

public class tmp_texture : MonoBehaviour
{


	Color light_grey = new Color(0.7f, 0.7f, 0.7f, 1f);
	Color dark = new Color(0.05f, 0.05f, 0.05f, 1f);


	[SerializeField] Texture2D texture_atlas;
	float grid_size;
	int line_thickness;
	Texture2D texture;

	public Floor floor;

	private void Awake()
	{
		texture = new Texture2D(972, 972, TextureFormat.ARGB32, false);
		line_thickness = 3;
		floor = new Floor();
	}



	// Start is called before the first frame update
	void Start()
	{
		grid_size = GLOBAL.GRID_SIZE ;
		draw_grid();

		//block_len corresponds to one pixel in 12x12 texture atlas
		int block_len = (int)((texture.width / grid_size) - line_thickness) / 3;

		// Apply all SetPixel calls
		texture.Apply();

		// connect texture to material of GameObject this script is attached to
		GetComponent<RawImage>().material.mainTexture = texture;


		//floor.start_collapse();
		//draw_current_floor();
		StartCoroutine(gen_map());
		StartCoroutine(update_texture());
	}
	void draw_grid ()
	{
		for (int x = 0; x < texture.width; x++)
		{
			for (int y = 0; y < texture.height; y++)
			{
				if (x % (texture.width / grid_size) < 1 * line_thickness)
				{
					texture.SetPixel(x, y, light_grey);
				}
				else if (y % (texture.height / grid_size) < 1 * line_thickness)
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
	void draw_room(int room_x, int room_y, int room_no_x, int room_no_y)
	{
		int block_len = (int)((texture.width / grid_size) - line_thickness) / 3;

		for (int x = 0; x < texture_atlas.width / 4; x++)
		{
			for (int y = 0; y < texture_atlas.height / 4; y++)
			{
				Color[] src_colors = new Color[block_len * block_len];
				for (int i = 0; i < block_len * block_len; i++)
				{
					src_colors[i] = texture_atlas.GetPixel(room_x * 3 + x, room_y * 3 + y);
				}
				int strt_x = x * block_len + line_thickness + (int)(room_no_x * (texture.width / grid_size));
				int strt_y = y * block_len + line_thickness + (int)(room_no_y * (texture.height / grid_size));
				
				texture.SetPixels(strt_x, strt_y, block_len, block_len, src_colors);
			}
		}

		texture.Apply();

		// connect texture to material of GameObject this script is attached to

	}

	public void draw_current_floor()
	{
		foreach (var t in floor.floor_data)
		{
			if (t.Value.collapsed)
			{
				draw_room(floor.tile_map_coord[(t.Value.value)].Item1, floor.tile_map_coord[(t.Value.value)].Item2, t.Value.x_cord, t.Value.y_cord);
			}
		}
	}

    IEnumerator gen_map()
    {
		while (true)
		{
			floor.start_collapse();
			//draw_current_floor();
			yield return new WaitForSeconds(2);
			while (floor.next_collapse())
			{
				//draw_current_floor();
				yield return new WaitForSeconds(0.001f);
			}
			draw_current_floor();
			floor.validate();
			floor.reset_floor();
			draw_grid();
		}
    }

	IEnumerator update_texture()
	{
		while (true)
		{			
			yield return new WaitForSeconds(10);			
			draw_current_floor();						
		}
	}



	// Update is called once per frame
	void Update()
	{

	}
}
