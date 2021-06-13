using System.Collections.Generic;
using ZeroFormatter;

namespace Oasez.Networking {

    [ZeroFormattable]
    public class RaiseEventOptions {

        [Index(0)]
        public virtual ReceiverGroup Receivers { get; set; } = ReceiverGroup.Others;
        [Index(1)]
        public virtual int[] InternalIds { get; set; }

        [IgnoreFormat]
        public bool HasInternalIds => InternalIds != null && InternalIds.Length > 0;

        [IgnoreFormat]
        public List<int> InternalIdList {
            get {
                if (internalIdList == null) {
                    internalIdList = new List<int>(InternalIds);
                }
                return internalIdList;
            }
        }
        [IgnoreFormat]
        private List<int> internalIdList;

        public RaiseEventOptions() { }

        public RaiseEventOptions(ReceiverGroup receivers = ReceiverGroup.Others, params int[] internalIds) {
            Receivers = receivers;
            InternalIds = internalIds;
        }

        public RaiseEventOptions(params int[] internalIds) : this(ReceiverGroup.Others, internalIds) { }

        public override string ToString() {
            return $"Receivers: {Receivers}";
        }

    } 

}