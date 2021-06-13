namespace Oasez.Networking {

    public class EventData {

        public byte Key { get; private set; }
        public object[] CustomData { get; private set; }
        public int SenderId { get; private set; }

        public EventData(byte key, object[] customData, int senderId) {
            Key = key;
            CustomData = customData;
            SenderId = senderId;
        }

    } 

}