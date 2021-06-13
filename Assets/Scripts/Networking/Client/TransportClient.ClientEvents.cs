using UnityEngine;
using Unity.Networking.Transport;
using ZeroFormatter;
using Unity.Collections;
using System;
using System.Collections.Generic;

namespace Oasez.Networking {

    public partial class TransportClient : MonoBehaviour {

        public Dictionary<int, Player> Players = new Dictionary<int, Player>();
        public Player LocalPlayer { get; internal set; } = new Player(-1, true);
        public int MasterClientId = -1;

        public Action<EventData> OnEventReceived;
        
        private readonly List<IConnectionCallbacks> CallbackTargets = new List<IConnectionCallbacks>();

        public void SendPacket(byte networkEventKey, RaiseEventOptions raiseEventOptions, params object[] customObjects) {
            driver.BeginSend(connection, out DataStreamWriter writer);

            writer.WriteByte(networkEventKey);

            byte[] raiseData = ZeroFormatterSerializer.Serialize(raiseEventOptions);
            NativeArray<byte> nativeRaiseData = new NativeArray<byte>(raiseData, Allocator.Temp);
            writer.WriteUShort((ushort)nativeRaiseData.Length);
            writer.WriteBytes(nativeRaiseData);

            if (customObjects == null) {
                writer.WriteByte(0);
                driver.EndSend(writer);
                return;
            }

            writer.WriteByte((byte)customObjects.Length);
            for (int i = 0; i < customObjects.Length; i++) {
                TransportObjectData objectData = TransportSerializer.Serialize(customObjects[i]);
                NativeArray<byte> nativeData = new NativeArray<byte>(objectData.Data, Allocator.Temp);

                writer.WriteByte(objectData.Typecode);
                writer.WriteUShort((ushort)nativeData.Length);
                writer.WriteBytes(nativeData);
            }

            driver.EndSend(writer);
        }

        public void AddCallbackTarget(IConnectionCallbacks target) {
            CallbackTargets.Add(target);
        }

        public void RemoveCallbackTarget(IConnectionCallbacks target) {
            CallbackTargets.Remove(target);
        }

        internal void OnLocalPropertiesChanged(Player player, Dictionary<byte, object> changedProperties) {
            CallbackTargets.ForEach(x => x.OnCustomPropertiesChanged(player, changedProperties));
        }

        private void ProcessClientEvent(DataStreamReader reader, byte key) {
            int senderId = reader.ReadInt();
            byte amountOfObjects = reader.ReadByte();
            object[] customObjects = new object[amountOfObjects];

            for (int i = 0; i < customObjects.Length; i++) {
                byte typecode = reader.ReadByte();
                ushort dataLength = reader.ReadUShort();
                byte[] data = new byte[dataLength];
                NativeArray<byte> nativeData = new NativeArray<byte>(data, Allocator.Temp);
                reader.ReadBytes(nativeData);
                data = nativeData.ToArray();

                customObjects[i] = TransportSerializer.Deserialize(typecode, data);
            }

            EventData eventData = new EventData(key, customObjects, senderId);
            OnEventReceived?.Invoke(eventData);
        }

    }

}