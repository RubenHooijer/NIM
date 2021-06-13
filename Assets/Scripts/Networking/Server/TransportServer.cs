using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;
using System;

namespace Oasez.Networking {

    public partial class TransportServer : MonoBehaviour {

        private NetworkDriver driver;
        private NativeList<NetworkConnection> connections;

        public void Host(string ip) {
            driver = NetworkDriver.Create();
            NetworkEndPoint endPoint = NetworkEndPoint.AnyIpv4;
            if (ip != null && ip != string.Empty && NetworkEndPoint.TryParse(ip, 9000, out NetworkEndPoint customEndPoint)) {
                endPoint = customEndPoint;
            }
            endPoint.Port = 9000;

            if (driver.Bind(endPoint) != 0) {
                Debug.Log($"<color=red>Server: Failed to bind to port <b>{endPoint}</b></color>");
            } else {
                driver.Listen();
            }

            connections = new NativeList<NetworkConnection>(2, Allocator.Persistent);
        }

        public void Disconnect() {
            for (int i = 0; i < connections.Length; i++) {
                connections[i].Disconnect(driver);
            }
        }

        private void OnDestroy() {
            for (int i = 0; i < connections.Length; i++) {
                connections[i].Disconnect(driver);
            }

            driver.Dispose();
            connections.Dispose();
        }

        private void Update() {
            driver.ScheduleUpdate().Complete();

            RemoveOldConnections();
            CheckForNewConnections();
            HandleEvents();
        }

        private void RemoveOldConnections() {
            for (int i = 0; i < connections.Length; i++) {
                if (!connections[i].IsCreated) {
                    connections.RemoveAtSwapBack(i);
                    --i;
                }
            }
        }

        private void CheckForNewConnections() {
            NetworkConnection connection;
            while ((connection = driver.Accept()) != new NetworkConnection()) {
                connections.Add(connection);
                OnNewConnection(connection);
            }
        }

        private void HandleEvents() {
            DataStreamReader reader;

            for (int i = 0; i < connections.Length; i++) {
                if (!connections[i].IsCreated) { continue; }
                NetworkEvent.Type cmd;

                while ((cmd = driver.PopEventForConnection(connections[i], out reader)) != NetworkEvent.Type.Empty) {
                    switch (cmd) {
                        case NetworkEvent.Type.Data:
                            OnReceivePacket(reader, connections[i]);
                            break;
                        case NetworkEvent.Type.Disconnect:
                            OnPlayerDisconnected(connections[i].InternalId);
                            connections[i] = default;
                            break;
                    }
                }
            }
        }

        private void OnNewConnection(NetworkConnection connection) {
            Debug.Log($"<color=green>Server: Accepted a connection {connection.InternalId}</color>");

            TransportClient client = TransportNetwork.NetworkingClient;
            Player player = new Player(connection.InternalId, connection.InternalId == 0);
            client.Players[player.InternalId] = player;
            if (player.IsLocal) { 
                client.MasterClientId = player.InternalId; 
            }

            SendServerData(connection);
            for (int i = 0; i < connections.Length; i++) {
                if (connections[i].InternalId == connection.InternalId) { continue; }
                SendJoinedPlayer(connections[i], player);
            }
        }

        private void OnPlayerDisconnected(int internalId) {
            Debug.Log($"<color=red>Server: A client with ID: ( {internalId} ) disconnected from server</color>");
            for (int i = 0; i < connections.Length; i++) {
                SendLeftPlayer(internalId, connections[i]);
            }
        }

        private void OnReceivePacket(DataStreamReader reader, NetworkConnection connection) {
            Debug.Log("Server: Got a packet");
            byte key = reader.ReadByte();

            if (Enum.IsDefined(typeof(ServerNetworkEventKeys), key)) {
                ProcessServerPacket(reader, connection, key);
            } else {
                ProcessClientPacket(reader, connection, key);
            }
        }

    } 

}