using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class UI_Controller : MonoBehaviour
{
    public GameObject gear_health;
    public TextMeshProUGUI health_text;
    public TextMeshProUGUI coin_text;


    public static UI_Controller current_instance;

    private float fill;

    void Awake()
    {
        //not used to make the object a singleton but for communication purposes
        current_instance = this; 
    }
	

    void Start()
    {
        coin_text.text = "coin :" + Player_controller.instance.coin_num.Value;
        health_text.text = "Health :" + Player_controller.instance.health;

        fill = (float)Player_controller.instance.health;
        fill = fill / Player_controller.instance.max_health;
        gear_health.GetComponent<Image>().fillAmount = fill;
    }
    // Update is called once per frame
    void Update()
    {
                  
    }
    public void update_gear_health()
	{
        fill = (float)Player_controller.instance.health;
        fill = fill / Player_controller.instance.max_health;
        gear_health.GetComponent<Image>().fillAmount = fill;
    }
    public void update_health_text()
	{
        health_text.text = "Health: "+ Player_controller.instance.health.ToString() ;        
    }

    public void update_coin_text()
    {
        coin_text.text = "coin :" + Player_controller.instance.coin_num.Value.ToString();
    }


}
