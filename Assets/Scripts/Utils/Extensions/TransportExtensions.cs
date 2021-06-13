using Oasez.Extensions.List;
using Oasez.Networking;
using System.Collections.Generic;

namespace Oasez.Extensions.Transport {

    public static class TransportExtensions {

        public static void SetNickname(this Player player, string nickname) {
            Dictionary<byte, object> changedProperties = new Dictionary<byte, object>() {
                { PlayerPropertyKeys.Nickname, nickname },
            };

            player.SetCustomProperties(changedProperties);
        }

        public static string GetNickname(this Player player) {
            return (string)player.CustomProperties[PlayerPropertyKeys.Nickname];
        }

        public static void SetWins(this Player player, int wins) {
            Dictionary<byte, object> changedProperties = new Dictionary<byte, object>() {
                { PlayerPropertyKeys.CurrentWins, wins },
            };

            player.SetCustomProperties(changedProperties);
        }

        public static int GetWins(this Player player) {
            if (player.CustomProperties.TryGetCastedValue(PlayerPropertyKeys.CurrentWins, out int wins)) {
                return wins;
            }
            return -1;
        }

        public static void SetTurn(this Player player, bool HasTurn) {
            Dictionary<byte, object> changedProperties = new Dictionary<byte, object>() {
                { PlayerPropertyKeys.IsAtTurn, HasTurn },
            };

            player.SetCustomProperties(changedProperties);
        }


        public static bool IsAtTurn(this Player player) {
            return (bool)player.CustomProperties[PlayerPropertyKeys.IsAtTurn];
        }

        public static void EndTurn(this Player player) {
            player.SetTurn(false);

            List<Player> players = new List<Player>(TransportNetwork.Players);
            Player nextPlayer = players.GetNext(player);
            nextPlayer.SetTurn(true);
        }

    }

}