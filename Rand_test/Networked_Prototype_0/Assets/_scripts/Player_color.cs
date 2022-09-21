using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class Player_color : NetworkBehaviour
{
	private readonly NetworkVariable<Color> _netColor = new();
	private readonly Color[] _colors = { Color.HSVToRGB(0.25f, 0.85f, 1f),
		Color.HSVToRGB(0.125f, 0.6f, 1f),
		Color.HSVToRGB(0.4f, 0.7f, 1f),
		Color.HSVToRGB(0.500f, 0.9f, 1f),
		Color.HSVToRGB(0.625f, 0.7f, 1f),
		Color.HSVToRGB(0.750f, 1f, 1f),
		Color.HSVToRGB(0.875f, 0.35f, 1f),
		Color.HSVToRGB(1f, 0.2f, 1f) };
	private int _index; 

	[SerializeField] private SpriteRenderer _renderer;

	private void Awake()
	{
		// Subscribing to a change event. This is how the owner will change its color.
		// Could also be used for future color changes
		_netColor.OnValueChanged += OnValueChanged;
		_renderer = GetComponent<SpriteRenderer>();
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
			//_renderer.material.color = _netColor.Value;
			_renderer.color = _netColor.Value;
		}
		
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
