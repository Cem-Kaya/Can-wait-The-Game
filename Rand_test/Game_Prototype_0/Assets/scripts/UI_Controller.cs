using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UI_Controller : MonoBehaviour
{
    public GameObject gear_helth_0;

    private float fill;
    void Start()
    {


    }
    // Update is called once per frame
    void Update()
    {
            fill = (float)Player_controller.Health;
            fill = fill / Player_controller.Max_health;
            gear_helth_0.GetComponent<Image>().fillAmount = fill;
        
    }
}
