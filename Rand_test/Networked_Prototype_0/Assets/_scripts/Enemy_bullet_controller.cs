using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_bullet_controller : MonoBehaviour
{
    public float lifetime = 7 ;
    public uint max_bounce = 5;
    public float damage = 1;
    public float terminal_velocity= 3.0f ;
    private Rigidbody2D rb;

	private void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
	}
	// Start is called before the first frame update
	void Start()
    {
        terminal_velocity = 3.0f;
        StartCoroutine(death_delay());
    }

    // Update is called once per frame
    void Update()
    {

    }
	private void FixedUpdate()
	{
        rb.velocity = Vector3.ClampMagnitude(rb.velocity,terminal_velocity);
       // Debug.Log("Velocity is:" + rb.velocity);
	}
	private IEnumerator death_delay()
	{
		yield return new WaitForSeconds(lifetime);
		Destroy(gameObject);
	}

	



    private void OnCollisionEnter2D(Collision2D other)
    {
        //Debug.Log("naem :" + collision.tag);
        if (other.gameObject.tag != "Enemy_bullet")
        {
            if (other.gameObject.tag == "Player")
			{
                Player_controller.take_damage(1);
                Destroy(gameObject);
            }
            if (--max_bounce == 0)
            {
                Destroy(gameObject);
            }
        }
    }

}
