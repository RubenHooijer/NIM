using Oasez.Networking;
using UnityEngine;

[CreateAssetMenu(menuName = "CustomSO/State/GameState")]
public class GameState : AbstractState {

    [SerializeField] private PlayerNetworkEventChannelSO lastPieceTakenNetworkEvent;
    [SerializeField] private VoidEventChannelSO disconnectedEvent;

    protected override void OnActivate() {
        BoardScreen.Instance.Show();

        lastPieceTakenNetworkEvent.OnEventRaised += OnLastPieceTaken;
        disconnectedEvent.OnEventRaised += OnDisconnect;
    }

    protected override void OnDeactivate() {
        BoardScreen.Instance.Hide();

        lastPieceTakenNetworkEvent.OnEventRaised -= OnLastPieceTaken;
        disconnectedEvent.OnEventRaised -= OnDisconnect;
    }

    private void OnLastPieceTaken(Player player) {
        fsm.GoToState<LobbyState>(new StateData(player));
    }

    private void OnDisconnect() {
        fsm.GoToState<MenuState>();
    }

}