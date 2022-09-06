using System;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;


[RequireComponent(typeof(NetworkManager))]
public class ExampleNetworkDiscovery : NetworkDiscovery<DiscoveryBroadcastData, DiscoveryResponseData>
{


    public string GetLocalIPAddress_string()
    {
        string ips ="";
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                ips +=  ip.ToString() + "/";
            }
        }
        ips = ips.Remove(ips.Length - 1);
        if (ips.Length == 0)
        {
            throw new System.Exception("No network adapters with an IPv4 address in the system!");
        }
        else
        {
            return ips;
        }
    }


    [Serializable]
    public class ServerFoundEvent : UnityEvent<IPEndPoint, DiscoveryResponseData>
    {
    };

    NetworkManager m_NetworkManager;
    
    [SerializeField]
    [Tooltip("If true NetworkDiscovery will make the server visible and answer to client broadcasts as soon as netcode starts running as server.")]
    bool m_StartWithServer = true;

    public string ServerName = "EnterName";

    public ServerFoundEvent OnServerFound;
    
    private bool m_HasStartedWithServer = false;

    public void Awake()
    {
        m_NetworkManager = GetComponent<NetworkManager>();
    }

    public void Update()
    {
        if (m_StartWithServer && m_HasStartedWithServer == false && IsRunning == false)
        {
            if (m_NetworkManager.IsServer)
            {
                StartServer();
                m_HasStartedWithServer = true;
            }
        }
    }

    protected override bool ProcessBroadcast(IPEndPoint sender, DiscoveryBroadcastData broadCast, out DiscoveryResponseData response)
    {
        //Debug.Log("server got the messege ");
        response = new DiscoveryResponseData()
        {
            ServerName = ServerName,
            Port = ((UnityTransport)m_NetworkManager.NetworkConfig.NetworkTransport).ConnectionData.Port,
			all_ips_respons = GetLocalIPAddress_string(),
		};
        return true;
    }

    protected override void ResponseReceived(IPEndPoint sender, DiscoveryResponseData response)
    {
        OnServerFound.Invoke(sender, response);
    }
}