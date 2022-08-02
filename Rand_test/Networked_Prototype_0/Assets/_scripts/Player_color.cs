using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class Player_color : NetworkBehaviour
{
    private readonly NetworkVariable<Color> _netColor = new();
    private readonly Color[] _colors = { Color.red, Color.blue, Color.green, Color.yellow, Color.black, Color.white, Color.magenta, Color.gray };
    private int _index;

    [SerializeField] private SpriteRenderer _renderer;

    private void Awake()
    {
        // Subscribing to a change event. This is how the owner will change its color.
        // Could also be used for future color changes
        _netColor.OnValueChanged += OnValueChanged;
        _renderer = GetComponent<SpriteRenderer>();
    }


    [ServerRpc (RequireOwnership = false)]
    private void reapply_color_ServerRpc()
    {
        reapply_color_ClientRpc();
    }


    [ClientRpc]
    private void reapply_color_ClientRpc()
	{
        
    }


   public override void OnDestroy()
    {
        _netColor.OnValueChanged -= OnValueChanged;
    }

    private void OnValueChanged(Color prev, Color next)
    {
        _renderer.color = next;
    }

    public override void OnNetworkSpawn()
    {
        // Take note, RPCs are queued up to run.
        // If we tried to immediately set our color locally after calling this RPC it wouldn't have propagated
        if (IsOwner)
        {
            _index = (int)OwnerClientId;
            CommitNetworkColorServerRpc(GetNextColor());
        }
        else
        {
            _renderer.material.color = _netColor.Value;
        }
        reapply_color_ServerRpc();
    }

    [ServerRpc]
    private void CommitNetworkColorServerRpc(Color color)
    {
        _netColor.Value = color;
    }

    

    private Color GetNextColor()
    {
        return _colors[_index++ % _colors.Length];
    }
}
