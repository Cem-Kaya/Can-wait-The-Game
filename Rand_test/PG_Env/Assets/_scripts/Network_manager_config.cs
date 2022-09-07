using System;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Net;
using System.Runtime.CompilerServices;
using Unity.Netcode;
using Unity.Netcode.Transports.UNET;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;


public class Network_manager_config : MonoBehaviour
{
    public string m_ConnectAddress;
	UnityTransport m_Transport;    
	string m_PortString = "26990";
    ushort port = 26990;
    NetworkManager m_NetworkManager;
    public Vector2 DrawOffset = new Vector2(10, 210);
    
    
    public List<string> GetLocalIPAddress()
    {
        List<string> ips = new List<string>();
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                ips.Add(ip.ToString());
            }
        }
        if (ips.Count == 0)
        {
            throw new System.Exception("No network adapters with an IPv4 address in the system!");
        }
        else
        {
            return ips;
        }
    }

    private void Awake()
    {
        m_NetworkManager = GetComponent<NetworkManager>();
    }

    // Start is called before the first frame update
    void Start()
    {        
        m_Transport = (UnityTransport)NetworkManager.Singleton.NetworkConfig.NetworkTransport;
        //m_ConnectAddress = GetLocalIPAddress();
        //m_Transport.SetConnectionData(m_ConnectAddress, port);
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(DrawOffset, new Vector2(200, 600)));

        selectIPGUI();

        GUILayout.EndArea();
    }

    void selectIPGUI()
    {
        List<string> my_ips = GetLocalIPAddress();
        for (int i = 0; i < my_ips.Count; i++)
        {
            if (GUILayout.Button(my_ips[i]))
            {
                m_ConnectAddress = my_ips[i];
                m_Transport.SetConnectionData(m_ConnectAddress, port);
                
            }
        }
        
        
        //if (m_Discovery.IsRunning)
        //{
        //    if (GUILayout.Button("Stop Server Discovery"))
        //    {
        //        m_Discovery.StopDiscovery();
        //    }
        //}
        //else
        //{
        //    if (GUILayout.Button("Start Server Discovery"))
        //    {
        //        m_Discovery.StartServer();
        //    }
        //}
    }

    public void StartGameClient()
    {
        m_NetworkManager.StartClient();
    }
}
