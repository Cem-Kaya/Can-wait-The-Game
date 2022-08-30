using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
	
public class Item_effect_manager : NetworkBehaviour
{
	public uint  delay_dec ;
	public int speed_up ;

    public int bullet_bounce;
    public float bullet_lf;
    public float bullet_dmg;
    public float bullet_scale;
    public float bullet_speed;

	// Start is called before the first frame update
	void Start()
	{
		
	}

	// Update is called once per frame
	void Update()
	{
		
	}
	
	void OnTriggerEnter2D(Collider2D hitObject)
	{		
		if (hitObject.gameObject.layer == 3)
		{
			box_mover player = hitObject.gameObject.GetComponent<box_mover>();
			player.dec_fire_rate_delay(delay_dec );
			player.inc_speed(speed_up);
			player.inc_bullet_att(bullet_lf , bullet_bounce,  bullet_dmg, bullet_scale, bullet_speed);

			gameObject.GetComponent<NetworkObject>().Despawn();
		}
		
		
	}


}
