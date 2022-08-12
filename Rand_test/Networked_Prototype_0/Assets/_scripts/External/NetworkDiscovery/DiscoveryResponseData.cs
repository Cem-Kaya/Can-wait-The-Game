using Unity.Netcode;
using UnityEngine;

public struct DiscoveryResponseData: INetworkSerializable
{
    public ushort Port;

    public string ServerName;

	public string all_ips_respons ;
	public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref Port);
        serializer.SerializeValue(ref ServerName);
		serializer.SerializeValue(ref all_ips_respons);
	}
}
