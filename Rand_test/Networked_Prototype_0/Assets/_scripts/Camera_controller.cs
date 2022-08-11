using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Camera_controller : MonoBehaviour
{
	
    public CinemachineVirtualCamera vcam ;


    public static Camera_controller instance;

    private CinemachineConfiner2D confiner;

    [SerializeField] private PolygonCollider2D current_bounding_shape;

    public static void load_new_boundry (PolygonCollider2D new_bounding_shape)
    {
        instance.confiner.m_BoundingShape2D = new_bounding_shape;
        instance.current_bounding_shape = new_bounding_shape;
    }
	
    private void Awake()
    {
		
        if (instance == null)
        {
            instance = this;		
        }
        confiner = instance.GetComponent<CinemachineConfiner2D>();
        
        

        //confiner = instance.GetCinemachineComponent<>().m_BoundingShape2D;

    }

    // Start is called before the first frame update
    void Start()
    {
        
        float screen_w = Screen.width;
        float screen_h = Screen.height;
        float as_ratio = screen_w / screen_h;
        if (as_ratio > 16.0f/9.0f )
        {
            float new_ortho = Mathf.Floor((screen_h * 20f ) / screen_w);
            vcam.m_Lens.OrthographicSize = new_ortho;
        }
        
        //float new_ortho = Mathf.Floor((screen_h * 16) / screen_w);
        //vcam.m_Lens.OrthographicSize = new_ortho;

    }

    // Update is called once per frame
    void Update()
    {

    }
}
