using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_controller : MonoBehaviour
{
    public float lifetime= 3 ;
    public uint max_bounce = 5 ;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(death_delay() );   
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private IEnumerator death_delay()
    {
        yield return new WaitForSeconds(lifetime);
        Destroy(gameObject);        
	}
      
    
    private void OnCollisionEnter2D(Collision2D other)
    {
        //Debug.Log("naem :" + collision.tag);
        if (other.gameObject.tag != "Bullet")
        {
            if (--max_bounce == 0)
            {
                Destroy(gameObject);
            }
        }
    }

}
