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
		texture = new Texture2D(1000, 1000, TextureFormat.ARGB32, false);
		line_thickness = 6;
	}
	// Start is called before the first frame update
	void Start()
	{
		grid_size= 10;

		for (int x = 0;x < texture.width; x++)
		{
			for (int y = 0; y < texture.height; y++)
			{
				if (x % (texture.width / grid_size)  < 1 * line_thickness)
				{
					texture.SetPixel(x, y, light_grey);
				}		
				else if (y % (texture.height / grid_size)  < 1 * line_thickness)
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
					texture.SetPixel(x, y, dark );
				}
			}
		}
		texture_atlas.Reinitialize((int)Mathf.Floor( texture.width / grid_size)*4 , (int)Mathf.Floor(texture.height / grid_size) * 4);
		
		//Graphics.CopyTexture(texture_atlas,  0,  0,(int)(texture.width / grid_size) * 2 , (int) ( (texture.height / grid_size) * 2) , (int)texture_atlas.width, (int)texture_atlas.height , texture,  0,  0, 50 , 50); ;
		 Graphics.CopyTexture(texture_atlas, 0, 0, 50, 50, 50,50, texture, 0, 0, 50, 50); 



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
