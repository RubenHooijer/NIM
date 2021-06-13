using Oasez.Extensions.Transport;
using Oasez.Networking;
using UnityEngine;

[CreateAssetMenu(menuName = "CustomSO/State/MenuState")]
public class MenuState : AbstractState {

    [SerializeField] private VoidEventChannelSO onConnectedEvent;

    protected override void OnActivate() {
        MenuScreen.Instance.Show();

        onConnectedEvent.OnEventRaised += OnConnectedToServer;
    }

    protected override void OnDeactivate() {
        MenuScreen.Instance.Hide();

        onConnectedEvent.OnEventRaised -= OnConnectedToServer;
    }

    private void OnConnectedToServer() {
        if (Account.IsLoggedIn) {
            TransportNetwork.LocalPlayer.SetNickname(Account.Username);
        }
        fsm.GoToState<LobbyState>();
    }

}