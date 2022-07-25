using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

// Singleton
public class Player_controller : MonoBehaviour
{
    public static Player_controller instance;
    public TextMeshProUGUI health_text;

    private static int health = 10 ;
    public static int Health
    {
        get { return health; }
        set { health = value; }
    }
	
    private static int max_health = 10 ; // max health of the player
    public static int Max_health
    {
        get { return max_health; }
        set { max_health = value; }
    }
    private static int fire_delay = 2 ;
    private static int Fire_delay
    {
        get { return fire_delay ; }
        set { fire_delay = value; }
    }

    private static float move_speed;
    public static float Move_speed
    {
        get { return move_speed; }
        set { move_speed = value; }
    }


    
    private static bool alive;
    private static bool Alive
    {
        get { return alive; }
        set { alive = value; }
    }
	private static bool i_frame ;
    private static float i_frame_sec;
	
    void Awake()
    {
        i_frame = false;
		i_frame_sec = 0.5f;
		if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        health_text.text = "Health : " + health;
    }

    // Update is called once per frame
    void Update()
    {
        health_text.text = "Health : " + health;
    }
    private  IEnumerator i_frame_delay()
    {

        yield return new WaitForSeconds(i_frame_sec);
		i_frame = false;
	}
    public static void take_damage(int damage ) {
        if (!i_frame)
        {
            i_frame = true;
            health -= damage;
            instance.StartCoroutine(instance.i_frame_delay());


            if (health < 0)
            {
                alive = false;
                death();
            }
        }
		
		
    }
	
    public static bool take_health_up(int health_up)
    {
        if (health >= max_health )
        {
            return false;
		}
		else
		{
			health= Mathf.Min(max_health,health_up + health ) ;//clamp
          
            return true;
        }
	}
    public static void death()
    {
        //Scene$$anonymous$$anager.LoadScene(GetActiveScene().name);
        health = 10;
        SceneManager.LoadScene("SampleScene");
    }
}

