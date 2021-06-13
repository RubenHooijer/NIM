using UnityEngine;
using Oasez.Networking;
using System.Collections.Generic;
using Oasez.Extensions.List;
using Oasez.Extensions.Transport;
using System;

[CreateAssetMenu(menuName = "CustomSO/State/LobbyState")]
public class LobbyState : AbstractState {

    [SerializeField] private VoidNetworkEventChannelSO startGameNetworkEvent;
    [SerializeField] private PlayerEventChannelSO playerLeftEvent;
    [SerializeField] private VoidEventChannelSO disconnectedEvent;

    protected override void OnActivate() {
        LobbyScreen.Instance.Show();

        LobbyScreen.Instance.StartButtonClickedEvent.AddListener(OnStartButtonClicked);
        LobbyScreen.Instance.ExitButtonClickedEvent.AddListener(ExitLobby);

        startGameNetworkEvent.OnEventRaised += OnStartGame;
        playerLeftEvent.OnEventRaised += OnPlayerLeft;
        disconnectedEvent.OnEventRaised += OnDisconnect;

        if (data != null && data.HasParameterAtIndex(0)) {
            ShowWinner(data.Get<Player>(0));
        }
    }

    protected override void OnDeactivate() {
        LobbyScreen.Instance.Hide();

        LobbyScreen.Instance.StartButtonClickedEvent.RemoveListener(OnStartButtonClicked);
        LobbyScreen.Instance.ExitButtonClickedEvent.RemoveListener(ExitLobby);

        startGameNetworkEvent.OnEventRaised -= OnStartGame;
        playerLeftEvent.OnEventRaised -= OnPlayerLeft;
        disconnectedEvent.OnEventRaised -= OnDisconnect;
    }

    private void OnStartButtonClicked() {
        SetupCustomProperties();
        startGameNetworkEvent.NetworkRaise(ReceiverGroup.All);
    }

    private void ExitLobby() {
        TransportNetwork.LeaveServer();
        fsm.GoToState<MenuState>();
    }

    private void OnDisconnect() {
        ExitLobby();
    }

    private void OnPlayerLeft(Player player) {
        Debug.Log("Someone left");
        if (player.IsMasterClient) {
            ExitLobby();
        }
    }

    private void OnStartGame() {
        Debug.Log($"<color=cyan>Host started the game</color>");
        fsm.GoToState<GameState>();
        //UnityEngine.SceneManagement.SceneManager.LoadScene(mapScene, UnityEngine.SceneManagement.LoadSceneMode.Additive);
        //fsm.GoToState<TurnState>();
    }

    private void ShowWinner(Player lastPiecePlayer) {
        if (TransportNetwork.LocalPlayer.IsMasterClient) {
            TransportNetwork.Players.GetNext(lastPiecePlayer).SetWins(lastPiecePlayer.GetWins() + 1);
        }

        Account.AddGame(!lastPiecePlayer.IsLocal);
    }

    private void SetupCustomProperties() {
        Player[] players = TransportNetwork.Players;
        Dictionary<byte, object> changedProperties = new Dictionary<byte, object>() {
            { PlayerPropertyKeys.IsAtTurn, false }
        };

        for (int i = 0; i < players.Length; i++) {
            players[i].SetCustomProperties(changedProperties);
        }

        players.GetRandom().SetTurn(true);
    }

}