using Oasez.Extensions.Transport;
using Oasez.Networking;
using TMPro;
using UnityEngine;

public class LobbyPlayerItem : MonoBehaviour {

    [SerializeField] private TextMeshProUGUI nameTextField;
    [SerializeField] private TextMeshProUGUI scoreTextField;
    private Player player;

    public void Setup(Player player) {
        this.player = player;

        if (!player.CustomProperties.ContainsKey(PlayerPropertyKeys.Nickname) && player.IsLocal) {
            if (Account.IsLoggedIn) {
                player.SetNickname(Account.Username);
            } else {
                player.SetNickname($"Player ({player.InternalId})");
            }
        }

        if (!player.CustomProperties.ContainsKey(PlayerPropertyKeys.CurrentWins) && player.IsLocal) {
            player.SetWins(0);
        }

        gameObject.SetActive(true);
    }

    private void Update() {
        if (player.CustomProperties.ContainsKey(PlayerPropertyKeys.Nickname)) {
            nameTextField.text = player.GetNickname();
        }

        if (player.CustomProperties.ContainsKey(PlayerPropertyKeys.Nickname)) { 
            scoreTextField.text = player.GetWins().ToString();
        }
    }

}