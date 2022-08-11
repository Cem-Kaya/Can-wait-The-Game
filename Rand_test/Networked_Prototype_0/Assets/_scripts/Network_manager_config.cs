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

public class Network_manager_config : MonoBehaviour
{
    public string m_ConnectAddress;
	UnityTransport m_Transport;    
	string m_PortString = "26990";
    ushort port = 26990;
	
	public string GetLocalIPAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                string txt = ip.ToString();
                return txt;
            }
        }
        throw new System.Exception("No network adapters with an IPv4 address in the system!");
    }

    // Start is called before the first frame update
    void Start()
    {        
        m_Transport = (UnityTransport)NetworkManager.Singleton.NetworkConfig.NetworkTransport;
        m_ConnectAddress = GetLocalIPAddress();
        m_Transport.SetConnectionData(m_ConnectAddress, port);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
