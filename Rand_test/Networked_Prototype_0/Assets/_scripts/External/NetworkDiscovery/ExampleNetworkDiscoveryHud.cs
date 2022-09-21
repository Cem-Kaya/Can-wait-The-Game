using System.Collections.Generic;
using System.Net;
using Unity.Netcode;
using Unity.Netcode.Transports.UNET;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using Object = UnityEngine.Object;
using System.Net.NetworkInformation;
using System.Net.Sockets;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Events;
#endif




[RequireComponent(typeof(ExampleNetworkDiscovery))]
[RequireComponent(typeof(NetworkManager))]
public class ExampleNetworkDiscoveryHud : MonoBehaviour
{
	[SerializeField, HideInInspector]
	ExampleNetworkDiscovery m_Discovery;
	
	NetworkManager m_NetworkManager;

	Dictionary<IPAddress, DiscoveryResponseData> discoveredServers = new Dictionary<IPAddress, DiscoveryResponseData>();

	private  Vector2 DrawOffset;

	void Awake()
	{
		m_Discovery = GetComponent<ExampleNetworkDiscovery>();
		m_NetworkManager = GetComponent<NetworkManager>();
		// get screan with in pixels 

		DrawOffset = new Vector2 ((Screen.width-500) /2 , Screen.height /3 );
	}

	private void Start()
	{

	}

   

#if UNITY_EDITOR
	void OnValidate()
	{
		if (m_Discovery == null) // This will only happen once because m_Discovery is a serialize field
		{
			m_Discovery = GetComponent<ExampleNetworkDiscovery>();
			UnityEventTools.AddPersistentListener(m_Discovery.OnServerFound, OnServerFound);
			Undo.RecordObjects(new Object[] { this, m_Discovery}, "Set NetworkDiscovery");
		}
	}
#endif

	void OnServerFound(IPEndPoint sender, DiscoveryResponseData response)
	{
		foreach (string ip_add in response.all_ips_respons.Split("/")) {
			DiscoveryResponseData tmp = new DiscoveryResponseData() ;
			tmp.Port = response.Port;
			tmp.ServerName = response.ServerName;
			//Debug.Log(ip_add);
			discoveredServers[ IPAddress.Parse( ip_add )] = tmp;
		}
	}

	void OnGUI()
	{
		GUILayout.BeginArea(new Rect(DrawOffset, new Vector2(500, 1000)));

		if (m_NetworkManager.IsServer || m_NetworkManager.IsClient)
		{
			if (m_NetworkManager.IsServer)
			{
				ServerControlsGUI();
			}
		}
		else
		{
			ClientSearchGUI();
		}

		GUILayout.EndArea();
	}

	void ClientSearchGUI()
	{
		if (m_Discovery.IsRunning)
		{
			if (GUILayout.Button("Stop Client Discovery"))
			{
				m_Discovery.StopDiscovery();
				discoveredServers.Clear();
			}
			
			if (GUILayout.Button("Refresh List"))
			{
				discoveredServers.Clear();
				m_Discovery.ClientBroadcast(new DiscoveryBroadcastData());
			}
			
			GUILayout.Space(20);
			
			foreach (var discoveredServer in discoveredServers)
			{
				string click_name = $"{discoveredServer.Value.ServerName}[{discoveredServer.Key.ToString()}]";
				
				if (click_name.Substring(8,3) == "172" || click_name.Substring(8, 3) == "127")
				{
					click_name += "(local computer same pc)";
				}else if (click_name.Substring(8, 3) == "192")
				{
					click_name += "(local network same wifi)";
				}
				else
				{
					click_name += "(unknown/other)";
				}


				if (GUILayout.Button(click_name))
				{
					UnityTransport transport = (UnityTransport)m_NetworkManager.NetworkConfig.NetworkTransport;
					
					transport.SetConnectionData(discoveredServer.Key.ToString(), discoveredServer.Value.Port);
					
					//Debug.Log(discoveredServer.Key.ToString());
					
					m_NetworkManager.StartClient();
				}
			}
		}
		else
		{
			if (GUILayout.Button("Discover Servers"))
			{
				m_Discovery.StartClient();
				m_Discovery.ClientBroadcast(new DiscoveryBroadcastData());
			}
		}
	}

	void ServerControlsGUI()
	{
		if (m_Discovery.IsRunning)
		{
			if (GUILayout.Button("Stop Server Discovery"))
			{
				m_Discovery.StopDiscovery();
			}
		}
		else
		{
			if (GUILayout.Button("Start Server Discovery"))
			{
				m_Discovery.StartServer();
			}
		}
	}
}
