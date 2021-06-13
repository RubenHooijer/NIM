using Oasez.Extensions.List;
using Oasez.Extensions.Transport;
using Oasez.Networking;
using System.Collections.Generic;
using UnityEngine;

public class NetworkEventReceiver : MonoBehaviour, IConnectionCallbacks {

    [SerializeField] private VoidEventChannelSO onConnectedEvent;
    [SerializeField] private VoidEventChannelSO onDisconnectedEvent;
    [SerializeField] private PlayerEventChannelSO onPlayerJoinedEvent;
    [SerializeField] private PlayerEventChannelSO onPlayerLeftEvent;

    [Header("Properties")]
    [SerializeField] private PlayerEventChannelSO newPlayerTurnEvent;

    private Dictionary<byte, BaseNetworkEventSO> networkEventsDictionary;

    public void OnCustomPropertiesChanged(Player player, Dictionary<byte, object> changedProperties) {
        Debug.Log($"{player.InternalId} properties changed");
        if (changedProperties.TryGetCastedValue(PlayerPropertyKeys.Nickname, out string nickname)) {
            Debug.Log($"Player ({player.InternalId}) changed nickname to: {nickname}");
        }

        if (changedProperties.TryGetCastedValue(PlayerPropertyKeys.IsAtTurn, out bool hasTurn)) {
            Debug.Log($"Player ended turnstate is set to {hasTurn}");
            if (hasTurn) {
                newPlayerTurnEvent.Raise(player);
            }
        }
    }

    public void OnConnected() {
        onConnectedEvent.Raise();
    }

    public void OnDisconnect() {
        onDisconnectedEvent.Raise();
    }

    public void OnPlayerJoined(Player player) {
        onPlayerJoinedEvent.Raise(player);
    }

    public void OnPlayerLeft(Player player) {
        onPlayerLeftEvent.Raise(player);
    }

    private void Awake() {
        BaseNetworkEventSO[] networkEvents = Resources.LoadAll<BaseNetworkEventSO>("NetworkEvents/");

        networkEventsDictionary = new Dictionary<byte, BaseNetworkEventSO>();
        foreach (BaseNetworkEventSO networkEvent in networkEvents) {
            networkEventsDictionary.Add((byte)networkEvent.NetworkEventKey, networkEvent);
        }
    }

    private void Start() {
        TransportNetwork.AddCallbackTarget(this);
        TransportNetwork.NetworkingClient.OnEventReceived += OnEventReceived;
    }
    private void OnDestroy() {
        TransportNetwork.NetworkingClient.OnEventReceived -= OnEventReceived;
    }

    private void OnEventReceived(EventData eventData) {
        byte eventKey = eventData.Key;
        if (!System.Enum.IsDefined(typeof(NetworkEventKeys), eventKey)) { return; }

        Debug.Log($"EventSO received with code {eventKey} with data:{eventData.CustomData}");
        if (networkEventsDictionary.ContainsKey(eventKey)) {
            networkEventsDictionary[eventKey].Raise(eventData.CustomData);
        }
    }

}