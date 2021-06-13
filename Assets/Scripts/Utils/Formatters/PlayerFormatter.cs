using Oasez.Networking;
using UnityEngine;
using ZeroFormatter;
using ZeroFormatter.Formatters;

public class PlayerFormatter<TTypeResolver> : Formatter<TTypeResolver, Player>
    where TTypeResolver : ITypeResolver, new() {

    public override int? GetLength() {
        return null;
    }

    public override int Serialize(ref byte[] bytes, int offset, Player value) {
        return Formatter<TTypeResolver, int>.Default.Serialize(ref bytes, offset, value.InternalId);
    }

    public override Player Deserialize(ref byte[] bytes, int offset, DirtyTracker tracker, out int byteSize) {
        int internalId = Formatter<TTypeResolver, int>.Default.Deserialize(ref bytes, offset, tracker, out byteSize);
        return TransportNetwork.NetworkingClient.Players[internalId];
    }

}