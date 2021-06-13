using UnityEngine;
using ZeroFormatter;
using ZeroFormatter.Formatters;

public class Vector2IntFormatter<TTypeResolver> : Formatter<TTypeResolver, Vector2Int>
    where TTypeResolver : ITypeResolver, new() {

    public override int? GetLength() {
        return null;
    }

    public override int Serialize(ref byte[] bytes, int offset, Vector2Int value) {
        int startOffset = offset;
        offset += Formatter<TTypeResolver, byte>.Default.Serialize(ref bytes, offset, (byte)value.x);
        offset += Formatter<TTypeResolver, byte>.Default.Serialize(ref bytes, offset, (byte)value.y);

        return offset - startOffset;
    }

    public override Vector2Int Deserialize(ref byte[] bytes, int offset, DirtyTracker tracker, out int byteSize) {
        byte x = Formatter<TTypeResolver, byte>.Default.Deserialize(ref bytes, offset, tracker, out byteSize);
        offset += sizeof(byte);
        byte y = Formatter<TTypeResolver, byte>.Default.Deserialize(ref bytes, offset, tracker, out byteSize);

        return new Vector2Int(x, y);
    }

}