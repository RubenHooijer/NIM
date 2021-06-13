using UnityEngine;
using Unity.Networking.Transport;
using Unity.Collections;
using System.Collections.Generic;

namespace Oasez.Networking {

    public partial class TransportClient : MonoBehaviour {

        public void SendChangedProperties(Player player, Dictionary<byte, object> changedProperties) {
            driver.BeginSend(connection, out DataStreamWriter writer);
            writer.WriteByte((byte)ServerNetworkEventKeys.ChangeProperties);
            writer.WriteInt(player.InternalId);

            writer.WriteByte((byte)changedProperties.Count);
            foreach (KeyValuePair<byte, object> item in changedProperties) {
                writer.WriteByte(item.Key);

                TransportObjectData objectData = TransportSerializer.Serialize(item.Value);
                writer.WriteByte(objectData.Typecode);
                NativeArray<byte> nativeData = new NativeArray<byte>(objectData.Data, Allocator.Temp);
                writer.WriteUShort((ushort)nativeData.Length);
                writer.WriteBytes(nativeData);
            }
            driver.EndSend(writer);
            Debug.Log($"Client: Send changed properties");
        }

        private void Ping() {
            driver.BeginSend(connection, out DataStreamWriter writer);
            writer.WriteByte((byte)ServerNetworkEventKeys.Ping);
            driver.EndSend(writer);
        }

        private void ProcessServerEvent(DataStreamReader reader, byte key) {
            switch ((ServerNetworkEventKeys)key) {
                case ServerNetworkEventKeys.ServerData:
                    ProcessServerDataEvent(reader);
                    break;
                case ServerNetworkEventKeys.PlayerJoined:
                    ProcessPlayerJoinedEvent(reader);
                    break;
                case ServerNetworkEventKeys.PlayerLeft:
                    ProcessPlayerLeftEvent(reader);
                    break;
                case ServerNetworkEventKeys.ChangeProperties:
                    ProcessChangedPropertiesEvent(reader);
                    break;
                case ServerNetworkEventKeys.Ping:
                    break;
                default:
                    Debug.Log($"No process event set for {key}");
                    break;
            }
        }

        private void ProcessChangedPropertiesEvent(DataStreamReader reader) {
            int playerId = reader.ReadInt();

            byte amountOfProperties = reader.ReadByte();
            Dictionary<byte, object> changedProperties = new Dictionary<byte, object>(amountOfProperties);
            for (int i = 0; i < amountOfProperties; i++) {
                byte key = reader.ReadByte();

                byte typecode = reader.ReadByte();
                ushort dataLength = reader.ReadUShort();
                byte[] data = new byte[dataLength];
                NativeArray<byte> nativeData = new NativeArray<byte>(data, Allocator.Temp);
                reader.ReadBytes(nativeData);

                Debug.Log($"Received tc: {typecode}");
                changedProperties[key] = TransportSerializer.Deserialize(typecode, nativeData.ToArray());
            }

            Player player = Players[playerId];
            foreach (KeyValuePair<byte, object> keyValuePair in changedProperties) {
                player.CustomProperties[keyValuePair.Key] = keyValuePair.Value;
            }
            Debug.Log($"Client: Process changed properties for {playerId}");

            CallbackTargets.ForEach(x => x.OnCustomPropertiesChanged(player, changedProperties));
        }

        private void ProcessServerDataEvent(DataStreamReader reader) {
            Debug.Log("Client: Received server data event");
            MasterClientId = reader.ReadInt();
            int myInternalId = reader.ReadInt();
            byte playerCount = reader.ReadByte();

            for (int i = 0; i < playerCount; i++) {
                Player player = ReadPlayer(ref reader, myInternalId);
                Players[player.InternalId] = player;
                if (player.IsLocal) {
                    LocalPlayer = player;
                } 
            }

            CallbackTargets.ForEach(x => x.OnConnected());
        }

        private void ProcessPlayerJoinedEvent(DataStreamReader reader) {
            Debug.Log("Client: Received player joined event");
            Player player = ReadPlayer(ref reader);
            Players[player.InternalId] = player;
            CallbackTargets.ForEach(x => x.OnPlayerJoined(player));
        }

        private void ProcessPlayerLeftEvent(DataStreamReader reader) {
            Debug.Log("Client: Received player left event");
            int leftPlayerInternalId = reader.ReadInt();
            Player player = Players[leftPlayerInternalId];
            Players.Remove(leftPlayerInternalId);
            CallbackTargets.ForEach(x => x.OnPlayerLeft(player));
        }

        private Player ReadPlayer(ref DataStreamReader reader, int myInternalId = -1) {
            int internalId = reader.ReadInt();
            byte amountOfCustomProperties = reader.ReadByte();

            Dictionary<byte, object> customProperties = new Dictionary<byte, object>();
            for (int p = 0; p < amountOfCustomProperties; p++) {
                byte propertyKey = reader.ReadByte();
                byte typecode = reader.ReadByte();
                ushort dataLength = reader.ReadUShort();
                byte[] data = new byte[dataLength];
                NativeArray<byte> nativeData = new NativeArray<byte>(data, Allocator.Temp);
                reader.ReadBytes(nativeData);

                customProperties[propertyKey] = TransportSerializer.Deserialize(typecode, nativeData.ToArray());
            }

            return new Player(internalId, internalId == myInternalId, customProperties);
        }


    }

}