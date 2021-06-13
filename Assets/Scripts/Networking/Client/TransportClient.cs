using UnityEngine;
using Unity.Networking.Transport;
using System;

namespace Oasez.Networking {

    public partial class TransportClient : MonoBehaviour {

        private const float MAX_DURATION_WITHOUT_PACKET = 10;

        private NetworkDriver driver;
        private NetworkConnection connection;

        private float timeAfterLastPacket = 0;

        public void Connect(string ip = null) {
            NetworkEndPoint endPoint = NetworkEndPoint.LoopbackIpv4;
            if (ip != null && ip != string.Empty && NetworkEndPoint.TryParse(ip, 9000, out NetworkEndPoint customEndPoint)) {
                endPoint = customEndPoint;
            }
            endPoint.Port = 9000;

            connection = driver.Connect(endPoint);
        }

        public void Disconnect() {
            connection = default;
        }

        private void Start() {
            driver = NetworkDriver.Create();
            connection = default;
        }

        private void OnDestroy() {
            if (driver.IsCreated) {
                driver.Dispose();
            }
        }

        private void OnApplicationQuit() {
            if (driver.IsCreated) {
                driver.Dispose();
            }
        }

        private void Update() {
            driver.ScheduleUpdate().Complete();

            if (!connection.IsCreated) { return; }
            HandleEvents();
            CheckForPing();
        }

        private void OnReceivePacket(DataStreamReader reader) {
            Debug.Log($"Client: Received a packet");
            byte key = reader.ReadByte();
            if (Enum.IsDefined(typeof(ServerNetworkEventKeys), key)) {
                ProcessServerEvent(reader, key);
            } else {
                ProcessClientEvent(reader, key);
            }
        }

        private void CheckForPing() {
            if (timeAfterLastPacket < MAX_DURATION_WITHOUT_PACKET) {
                timeAfterLastPacket += Time.deltaTime;
            } else {
                Ping();
                timeAfterLastPacket = 0;
            }
        }

        private void HandleEvents() {
            DataStreamReader reader;
            NetworkEvent.Type cmd;
            
            while ((cmd = connection.PopEvent(driver, out reader)) != NetworkEvent.Type.Empty) {
                switch (cmd) {
                    case NetworkEvent.Type.Data:
                        OnReceivePacket(reader);
                        break;
                    case NetworkEvent.Type.Connect:
                        Debug.Log("<color=green>Client: We are now connected to the server</color>");
                        break;
                    case NetworkEvent.Type.Disconnect:
                        Debug.Log("<color=red>Client: We got disconnected from server</color>");
                        connection = default;
                        CallbackTargets.ForEach(x => x.OnDisconnect());
                        break;
                }
            }
        }

    } 

}