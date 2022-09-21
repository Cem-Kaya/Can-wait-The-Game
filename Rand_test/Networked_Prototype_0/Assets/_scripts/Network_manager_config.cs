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
	public Vector2 DrawOffset;
	
	
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
		DrawOffset = new Vector2((Screen.width - 500) / 2, Screen.height / 3);

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
		GUILayout.BeginArea(new Rect(DrawOffset, new Vector2(500, 1000)));

		selectIPGUI();

		GUILayout.EndArea();
	}

	void selectIPGUI()
	{
		List<string> my_ips = GetLocalIPAddress();
		for (int i = 0; i < my_ips.Count; i++)
		{
			string click_name = my_ips[i];
			
			if (click_name.Substring(0, 3) == "172" || click_name.Substring(0, 3) == "127")
			{
				click_name += "(local computer same pc)";
			}
			else if (click_name.Substring(0, 3) == "192")
			{
				click_name += "(local network same wifi)";
			}
			else
			{
				click_name += "(unknown/other)";
			}



			if (GUILayout.Button(click_name))
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
