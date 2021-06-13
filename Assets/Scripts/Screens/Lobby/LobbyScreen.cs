using Oasez.Networking;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LobbyScreen : AbstractScreen<LobbyScreen> {

    public readonly UnityEvent StartButtonClickedEvent = new UnityEvent();
    public readonly UnityEvent ExitButtonClickedEvent = new UnityEvent();

    [SerializeField] private LobbyPlayerItem[] lobbyPlayers;
    [SerializeField] private Button startButton;
    [SerializeField] private Button exitButton;

    [Header("Events")]
    [SerializeField] private VoidEventChannelSO joinedRoomEvent;
    [SerializeField] private PlayerEventChannelSO playerJoinedEvent;
    [SerializeField] private PlayerEventChannelSO playerLeftEvent;

    public void Refresh() {
        Player[] players = TransportNetwork.Players;
        for (int i = 0; i < players.Length; i++) {
            if (lobbyPlayers.Length <= i) { break; }
            lobbyPlayers[i].Setup(players[i]);
        }

        startButton.gameObject.SetActive(TransportNetwork.LocalPlayer.IsMasterClient);
    }

    protected override void Awake() {
        base.Awake();
        gameObject.SetActive(false);
    }

    protected override void OnShow() {
        gameObject.SetActive(true);
        Refresh();

        joinedRoomEvent.OnEventRaised += Refresh;
        playerJoinedEvent.OnEventRaised += Refresh;
        playerLeftEvent.OnEventRaised += Refresh;

        startButton.onClick.AddListener(StartButtonClickedEvent.Invoke);
        exitButton.onClick.AddListener(ExitButtonClickedEvent.Invoke);
    }

    protected override void OnHide() {
        gameObject.SetActive(false);

        joinedRoomEvent.OnEventRaised -= Refresh;
        playerJoinedEvent.OnEventRaised -= Refresh;
        playerLeftEvent.OnEventRaised -= Refresh;

        startButton.onClick.RemoveListener(StartButtonClickedEvent.Invoke);
        exitButton.onClick.RemoveListener(ExitButtonClickedEvent.Invoke);
    }

    private void Refresh(Player player) => Refresh();

}