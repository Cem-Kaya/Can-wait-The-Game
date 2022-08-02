using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;


public class Network_Gui : MonoBehaviour
{
    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(250, 10,900 , 600));
        if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
        {
            if (GUILayout.Button("Host")) NetworkManager.Singleton.StartHost();
            if (GUILayout.Button("Client")) NetworkManager.Singleton.StartClient();
        }

        GUILayout.EndArea();
    }

    // private void Awake() {
    //     GetComponent<UnityTransport>().SetDebugSimulatorParameters(
    //         packetDelay: 120,
    //         packetJitter: 5,
    //         dropRate: 3);
    // }
}
