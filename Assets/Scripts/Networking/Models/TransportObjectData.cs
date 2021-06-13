using ZeroFormatter;

namespace Oasez.Networking {

    [ZeroFormattable]
    public class TransportObjectData {

        [Index(0)]
        public virtual byte Typecode { get; set; }

        [Index(1)]
        public virtual byte[] Data { get; set; }

        public TransportObjectData() { }
        public TransportObjectData(byte typecode, byte[] data) {
            Typecode = typecode;
            Data = data;
        }

    } 

}