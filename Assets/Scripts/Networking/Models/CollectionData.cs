using ZeroFormatter;

namespace Oasez.Networking {

    [ZeroFormattable]
    public class CollectionData {

        [Index(0)]
        public virtual byte Typecode { get; set; }

        [Index(1)]
        public virtual byte[][] DataArray { get; set; }

    }

}