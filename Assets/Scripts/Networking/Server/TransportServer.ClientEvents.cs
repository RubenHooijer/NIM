using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;
using ZeroFormatter;

namespace Oasez.Networking {

    public partial class TransportServer : MonoBehaviour {

        public void SendPacket(RaiseEventOptions raiseEventOptions, NetworkConnection fromConnection, byte key, TransportObjectData[] customObjectDatas) {
            for (int connectionIndex = 0; connectionIndex < connections.Length; connectionIndex++) {
                int connectionId = connections[connectionIndex].InternalId;
                if (!ShouldSendTo(connectionId, raiseEventOptions, fromConnection.InternalId)) { continue; }

                driver.BeginSend(connections[connectionIndex], out DataStreamWriter writer);

                writer.WriteByte(key);
                writer.WriteInt(fromConnection.InternalId);
                writer.WriteByte((byte)customObjectDatas.Length);

                for (int dataIndex = 0; dataIndex < customObjectDatas.Length; dataIndex++) {
                    NativeArray<byte> nativeData = new NativeArray<byte>(customObjectDatas[dataIndex].Data, Allocator.Temp);

                    writer.WriteByte(customObjectDatas[dataIndex].Typecode);
                    writer.WriteUShort((ushort)nativeData.Length);
                    writer.WriteBytes(nativeData);
                }

                driver.EndSend(writer);
            }
            Debug.Log("Server: Resend the packet");
        }

        private void ProcessClientPacket(DataStreamReader reader, NetworkConnection connection, byte key) {
            ushort raiseDataLength = reader.ReadUShort();
            byte[] raiseData = new byte[raiseDataLength];
            NativeArray<byte> nativeRaiseData = new NativeArray<byte>(raiseData, Allocator.Temp);

            reader.ReadBytes(nativeRaiseData);
            RaiseEventOptions raiseEventOptions = ZeroFormatterSerializer.Deserialize<RaiseEventOptions>(nativeRaiseData.ToArray());

            byte amountOfObjects = reader.ReadByte();
            TransportObjectData[] customObjectDatas = new TransportObjectData[amountOfObjects];

            for (int i = 0; i < customObjectDatas.Length; i++) {
                byte typecode = reader.ReadByte();
                ushort dataLength = reader.ReadUShort();
                byte[] data = new byte[dataLength];
                NativeArray<byte> nativeData = new NativeArray<byte>(data, Allocator.Temp);
                reader.ReadBytes(nativeData);

                TransportObjectData objectData = new TransportObjectData(typecode, nativeData.ToArray());
                customObjectDatas[i] = objectData;
            }

            SendPacket(raiseEventOptions, connection, key, customObjectDatas);
        }

        private bool ShouldSendTo(int toConnectionId, RaiseEventOptions raiseEventOptions , int senderId) {
            if (raiseEventOptions.HasInternalIds) {
                if (!raiseEventOptions.InternalIdList.Contains(toConnectionId)) { return false; }
            } else if (
                (raiseEventOptions.Receivers == ReceiverGroup.Master && toConnectionId != TransportNetwork.LocalPlayer.InternalId) ||
                (raiseEventOptions.Receivers == ReceiverGroup.Others && toConnectionId == senderId)) {
                return false;
            }
            return true;
        }

    }

}