using System.Collections.Generic;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;

namespace Oasez.Networking {

    public partial class TransportServer : MonoBehaviour {

        private void ProcessServerPacket(DataStreamReader reader, NetworkConnection connection, byte key) {
            switch ((ServerNetworkEventKeys)key) {
                case ServerNetworkEventKeys.ChangeProperties:
                    SendChangedProperties(reader, connection);
                    break;
                case ServerNetworkEventKeys.Ping:
                    Ping(connection);
                    break;
            }
        }

        private void Ping(NetworkConnection connection) {
            driver.BeginSend(connection, out DataStreamWriter writer);
            writer.WriteByte((byte)ServerNetworkEventKeys.Ping);
            driver.EndSend(writer);
        }

        private void SendChangedProperties(DataStreamReader reader, NetworkConnection connection) {
            int playerId = reader.ReadInt();

            byte amountOfProperties = reader.ReadByte();
            Dictionary<byte, TransportObjectData> changedProperties = new Dictionary<byte, TransportObjectData>(amountOfProperties);
            for (int i = 0; i < amountOfProperties; i++) {
                byte key = reader.ReadByte();

                byte typecode = reader.ReadByte();
                ushort dataLength = reader.ReadUShort();
                byte[] data = new byte[dataLength];
                NativeArray<byte> nativeData = new NativeArray<byte>(data, Allocator.Temp);
                reader.ReadBytes(nativeData);

                changedProperties[key] = new TransportObjectData(typecode, nativeData.ToArray());
            }

            for (int i = 0; i < connections.Length; i++) {
                if (connections[i].InternalId == connection.InternalId) { continue; }
                driver.BeginSend(connections[i], out DataStreamWriter writer);
                writer.WriteByte((byte)ServerNetworkEventKeys.ChangeProperties);
                writer.WriteInt(playerId);

                writer.WriteByte((byte)changedProperties.Count);
                foreach (KeyValuePair<byte, TransportObjectData> item in changedProperties) {
                    writer.WriteByte(item.Key);

                    TransportObjectData objectData = item.Value;
                    writer.WriteByte(objectData.Typecode);
                    NativeArray<byte> nativeData = new NativeArray<byte>(objectData.Data, Allocator.Temp);
                    writer.WriteUShort((ushort)nativeData.Length);
                    writer.WriteBytes(nativeData);
                }

                driver.EndSend(writer);
            }

        }

        private void SendServerData(NetworkConnection connection) {
            TransportClient client = TransportNetwork.NetworkingClient;
            Dictionary<int, Player> players = client.Players;

            driver.BeginSend(connection, out DataStreamWriter writer);
            writer.WriteByte((byte)ServerNetworkEventKeys.ServerData);
            writer.WriteInt(client.MasterClientId);
            writer.WriteInt(connection.InternalId);

            writer.WriteByte((byte)players.Count);
            foreach (KeyValuePair<int, Player> internalIdPlayerPair in players) {
                Player player = internalIdPlayerPair.Value;

                writer.WriteInt(player.InternalId);
                writer.WriteByte((byte)player.CustomProperties.Count);

                foreach (KeyValuePair<byte, object> keyValuePair in player.CustomProperties) {
                    TransportObjectData serializedValue = TransportSerializer.Serialize(keyValuePair.Value);
                    NativeArray<byte> nativeData = new NativeArray<byte>(serializedValue.Data, Allocator.Temp);

                    writer.WriteByte(keyValuePair.Key);
                    writer.WriteByte(serializedValue.Typecode);
                    writer.WriteUShort((ushort)nativeData.Length);
                    writer.WriteBytes(nativeData);
                }
            }
            driver.EndSend(writer);
        }

        private void SendJoinedPlayer(NetworkConnection connection, Player player) {
            driver.BeginSend(connection, out DataStreamWriter writer);
            writer.WriteByte((byte)ServerNetworkEventKeys.PlayerJoined);

            writer.WriteInt(player.InternalId);
            writer.WriteByte((byte)player.CustomProperties.Count);

            foreach (KeyValuePair<byte, object> keyValuePair in player.CustomProperties) {
                TransportObjectData serializedValue = TransportSerializer.Serialize(keyValuePair.Value);
                NativeArray<byte> nativeData = new NativeArray<byte>(serializedValue.Data, Allocator.Temp);

                writer.WriteByte(keyValuePair.Key);
                writer.WriteByte(serializedValue.Typecode);
                writer.WriteUShort((ushort)nativeData.Length);
                writer.WriteBytes(nativeData);
            }
            driver.EndSend(writer);
        }

        private void SendLeftPlayer(int leftInternalId, NetworkConnection toConnection) {
            driver.BeginSend(toConnection, out DataStreamWriter writer);
            writer.WriteByte((byte)ServerNetworkEventKeys.PlayerLeft);
            writer.WriteInt(leftInternalId);
            driver.EndSend(writer);
        }

    }

}