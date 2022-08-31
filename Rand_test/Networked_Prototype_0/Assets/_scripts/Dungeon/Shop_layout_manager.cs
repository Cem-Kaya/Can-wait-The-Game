using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;  


public class Shop_information
{	
	public Dictionary<GameObject, bool> sale_status;
	public List<GameObject> current_floor_shop_items;

	public Shop_information (List<GameObject> in_shop_pool)
	{
		set_items_for_sale(in_shop_pool);
	}

	public void set_items_for_sale(List<GameObject> shop_pool )
	{
		int num_item = shop_pool.Count < 4 ? shop_pool.Count : 4;
		for (int i = 0; i < num_item; i++)
		{
			while (true)
			{
				var tmp_item = shop_pool[Random.Range(0,shop_pool.Count)];
				if ( !current_floor_shop_items.Contains(tmp_item))				
				{
					current_floor_shop_items.Add(tmp_item);
					sale_status.Add(tmp_item, false);
					break; 
				}				
			}
		}
	}

}

public class Shop_layout_manager : NetworkBehaviour
{
	public List<GameObject> sellable_prefabs_pool;
	
	public List<GameObject> spawned;

	public static Shop_information shop_info;
	// Start is called before the first frame update

	private void Awake()
	{
		if(shop_info == null)
		{
			shop_info = new Shop_information(sellable_prefabs_pool);
		}
	}
	void Start()
	{
		int i = 0;
		foreach (GameObject item in shop_info.current_floor_shop_items)
		{
			GameObject tmp_item = Instantiate(item, new Vector3(0,0,0), Quaternion.identity);

			i++;
		}
	}

	// Update is called once per frame
	void Update()
	{
		
	}
}
