using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;


public class tmp_texture : MonoBehaviour    
{
	Color light_grey = new Color(0.7f, 0.7f, 0.7f, 1f);
	Color dark = new Color( 0.05f, 0.05f, 0.05f, 1f);


	[SerializeField] Texture2D texture_atlas ;
	float grid_size;
	int line_thickness;
	Texture2D texture;

	private void Awake()
	{
		texture = new Texture2D(972, 972, TextureFormat.ARGB32, false);
		line_thickness = 6;
	}
	// Start is called before the first frame update
	void Start()
	{	
		grid_size = 10;
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

		//block_len corresponds to one pixel in 12x12 texture atlas
		int block_len = (int)((texture.width / grid_size)-line_thickness )/3 ;
		Debug.Log(texture_atlas.width + " " + texture_atlas.height);
        Debug.Log(texture.width + " " + texture.height);


		int room_x = 1; 
		int room_y = 1;
        for (int x = 0; x < texture_atlas.width/4; x++)
		{
			for (int y = 0 ; y < texture_atlas.height/4; y++)
			{
				Color[] src_colors = new Color[block_len* block_len];
				for(int i = 0; i < block_len*block_len; i++)
				{					
                    src_colors[i] = texture_atlas.GetPixel(room_x*3 + x, room_y*3+ y);
                }
                //Debug.Log(src_colors.Length);
                var strt_x = x * block_len;
                var strt_y = y * block_len;
				//Debug.Log(strt_x + " " + strt_y);
				//Debug.Log((strt_x + block_len) + " " + (strt_y + block_len));

				texture.SetPixels(strt_x, strt_y, block_len, block_len, src_colors);
			}
		}
		//texture_atlas.Reinitialize((int)Mathf.Floor( texture.width / grid_size)*4 , (int)Mathf.Floor(texture.height / grid_size) * 4);
		
		//Graphics.CopyTexture(texture_atlas,  0,  0,(int)(texture.width / grid_size) * 2 , (int) ( (texture.height / grid_size) * 2) , (int)texture_atlas.width, (int)texture_atlas.height , texture,  0,  0, 50 , 50); ;
		 //Graphics.CopyTexture(texture_atlas, 0, 0, 50, 50, 50,50, texture, 0, 0, 50, 50); 



		// Apply all SetPixel calls
		texture.Apply();

		// connect texture to material of GameObject this script is attached to
		GetComponent<RawImage>().material.mainTexture = texture;
	}

	// Update is called once per frame
	void Update()
    {
        
    }
}
