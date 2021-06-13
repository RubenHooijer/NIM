using System.Collections.Generic;

namespace Oasez.Networking {

    public class Player {

        public int InternalId { get; }
        public bool IsMasterClient => TransportNetwork.MasterClientId == InternalId;
        public bool IsLocal { get; }

        public Dictionary<byte, object> CustomProperties = new Dictionary<byte, object>();

        protected internal Player(int internalId, bool isLocal, Dictionary<byte, object> playerProperties) {
            IsLocal = isLocal;
            InternalId = internalId;
            CustomProperties = playerProperties;
        }

        protected internal Player(int internalId, bool isLocal) : this(internalId, isLocal, new Dictionary<byte, object>()) { }

        public void SetCustomProperties(Dictionary<byte, object> changedProperties) {
            foreach (KeyValuePair<byte, object> keyWithData in changedProperties) {
                CustomProperties[keyWithData.Key] = keyWithData.Value;
            }

            TransportNetwork.NetworkingClient.SendChangedProperties(this, changedProperties);
            TransportNetwork.NetworkingClient.OnLocalPropertiesChanged(this, changedProperties);
        }

    }

}