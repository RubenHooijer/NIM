using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Oasez.Networking {

    public static class TransportNetwork {

        internal static TransportClient NetworkingClient => TransportNetworkHandler.Instance.Client;

        public static Player[] Players {
            get {
                if (NetworkingClient.Players.Count > 0) {
                    return NetworkingClient.Players.Values.OrderBy((x) => x.InternalId).ToArray();
                }
                return new Player[0];
            }
        }

        public static Player LocalPlayer {
            get {
                return NetworkingClient.LocalPlayer;
            }
        }

        public static int MasterClientId {
            get {
                return NetworkingClient.MasterClientId;
            }
        }

        public static void RaiseEvent(byte networkEventKey, RaiseEventOptions raiseEventOptions, params object[] customObjects) {
            NetworkingClient.SendPacket(networkEventKey, raiseEventOptions, customObjects);
        }

        public static void JoinServer(string ip) {
            NetworkingClient.Connect(ip);
        }

        public static void LeaveServer() {
            NetworkingClient.Disconnect();
            if (TransportNetworkHandler.Instance.Server != null) {
                TransportNetworkHandler.Instance.Server.Disconnect();
            }
        }

        public static void CreateServer(string ip) {
            TransportNetworkHandler.Instance.AddServer();
            TransportNetworkHandler.Instance.Server.Host(ip);
            NetworkingClient.Connect(ip);
        }

        public static void AddCallbackTarget(IConnectionCallbacks target) {
            NetworkingClient.AddCallbackTarget(target);
        }

        public static void RemoveCallbackTarget(IConnectionCallbacks target) {
            NetworkingClient.RemoveCallbackTarget(target);
        }

    }

    public interface IConnectionCallbacks {

        public void OnConnected();
        public void OnDisconnect();

        public void OnPlayerJoined(Player player);
        public void OnPlayerLeft(Player player);

        public void OnCustomPropertiesChanged(Player player, Dictionary<byte, object> changedProperties);

    } 

}