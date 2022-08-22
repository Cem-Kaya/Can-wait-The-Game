using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine.Rendering;

public static class GLOBAL
{
    public const int GRID_SIZE = 10;
    public const int GRID_SIZE_X = 5;
    public const int GRID_SIZE_Y = 3;
}


//singleton
public class Dungeon_controller : NetworkBehaviour
{
    public static Dungeon_controller instance;
    public Floor current_floor;
    public bool created;


    Color light_grey = new Color(0.7f, 0.7f, 0.7f, 1f);
    Color dark = new Color(0.05f, 0.05f, 0.05f, 1f);

    [SerializeField] Texture2D texture_atlas;
    float grid_size_x;
    float grid_size_y;
    int block_len_x;
    int block_len_y;

    int line_thickness;
    public Texture2D texture;




    // Start is called before the first frame update
    private void Awake()
    {
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
        texture.Apply();
        created = true;
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
}
