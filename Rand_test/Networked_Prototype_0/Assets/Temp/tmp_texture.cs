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
		Texture2D texture = new Texture2D(50, 50, TextureFormat.ARGB32, false);		

        // set the pixel values
        texture.SetPixel(0, 0, new Color(1.0f, 1.0f, 1.0f, 0.5f));
		texture.SetPixel(50, 0, Color.clear);
		texture.SetPixel(0, 50, Color.white);
		texture.SetPixel(50, 50, Color.black);

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
