using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;  


public class Shop_information
{	
	public Dictionary<GameObject, bool> sale_status = new Dictionary<GameObject,bool>();
	public List<GameObject> current_floor_shop_items = new List<GameObject>();

	public Shop_information (List<GameObject> in_shop_pool, int num_items)
	{
		set_items_for_sale(in_shop_pool, num_items);
        Item_effect_manager.on_purchase += update_sale_status;
    }

	public void set_items_for_sale(List<GameObject> shop_pool, int num_items )
	{
		//Debug.Log("len of shoop pool " + shop_pool.Count);
		int num_item = shop_pool.Count < num_items ? shop_pool.Count : num_items;
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

    

    public void update_sale_status(GameObject item)
    {
        
        
		Debug.Log("It is turning true");
        sale_status[item] = true;
        foreach (KeyValuePair<GameObject, bool> pair in sale_status)
        {
            Debug.Log("Event: " + pair.Key.name + " " + pair.Value);
        }
    }

    ~Shop_information()
    {
        Item_effect_manager.on_purchase -= update_sale_status;
    }


}

public class Shop_layout_manager : NetworkBehaviour
{
	public List<GameObject> sellable_prefabs_pool;
	
	public List<GameObject> spawned;

	public static Shop_information shop_info;

	public int num_items;
	// Start is called before the first frame update
	float room_len_x ;
	float room_len_y ;
	private void Awake()
	{
		room_len_x = 23f;
		room_len_y = 12f;
		
	}
	void Start()
	{
		if (shop_info == null)
		{
			shop_info = new Shop_information(sellable_prefabs_pool, num_items);
		}
		int i = 0;

        foreach (KeyValuePair<GameObject, bool> pair in shop_info.sale_status)
        {
            Debug.Log(pair.Key.name + " " + pair.Value);
        }

        foreach (GameObject item in shop_info.current_floor_shop_items)
		{

           
            if (!shop_info.sale_status[item]){
				int sq = (int)Mathf.Ceil(Mathf.Sqrt(shop_info.current_floor_shop_items.Count));
				int grid_len_x = (int)Mathf.Floor(room_len_x / sq);
				int grid_len_y = (int)Mathf.Floor(room_len_y / sq);

				float tmp_x = ((int)(i / sq)) * grid_len_x - (room_len_x / 2) + (grid_len_x) / 2;
				float tmp_y = (sq - 1 - ((i) % sq)) * grid_len_y - (room_len_y / 2) + (grid_len_y) / 2; //  fix this number later !!! TODO

				GameObject tmp_item = Instantiate(item, new Vector3(tmp_x, tmp_y, 0), Quaternion.identity);
				tmp_item.GetComponent<NetworkObject>().Spawn();
				spawned.Add(tmp_item);
            }

            i++;

        }
    }

	// Update is called once per frame
	void Update()
	{
		
	}

    public override void OnNetworkDespawn()
    {
 

        foreach (GameObject spawned_object in spawned)
        {
            //Debug.Log("Got in OnNetworkDespawn");

            if (IsServer && (spawned_object != null) && spawned_object.GetComponent<NetworkObject>() != null && spawned_object.GetComponent<NetworkObject>().IsSpawned)
            {
                //Debug.Log("Got in OnNetworkDespawn's if clause");
                spawned_object.GetComponent<NetworkObject>().Despawn();
            }
            base.OnNetworkDespawn();
        }
    }
}
