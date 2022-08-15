using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;



public class tmp_texture : MonoBehaviour    
{
	// Start is called before the first frame update
	void Start()
	{
		// Create a new 2x2 texture ARGB32 (32 bit with alpha) and no mipmaps
		Texture2D texture = new Texture2D(1080, 1080, TextureFormat.ARGB32, false);

		// set the pixel values
		for(int x = 0;x < texture.width; x++)
		{
			for (int y = 0; y < texture.height; y++)
			{
				if (x % 30 <3 )
				{
					texture.SetPixel(x, y, new Color(0.0f, 0.0f, 0.0f, 1f));
				}
				else if(y % 30 < 3)
				{
					texture.SetPixel(x, y, new Color(0.0f, 0.0f, 0.0f, 1f));
				}
				else
				{
					texture.SetPixel(x, y, new Color(1.0f, 1.0f, 1.0f, 1f));
				}
			}
		}
		
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
