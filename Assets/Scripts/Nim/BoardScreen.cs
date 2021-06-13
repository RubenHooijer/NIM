using Oasez.Extensions.Transport;
using Oasez.Networking;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardScreen : AbstractScreen<BoardScreen> {

    [SerializeField] private IntNetworkEventChannelSO takePieceNetworkEvent;
    [SerializeField] private PlayerNetworkEventChannelSO lastPieceTakenNetworkEvent;
    [SerializeField] private PlayerEventChannelSO newPlayerTurnEvent;
    [SerializeField] private List<PieceRow> rows = new List<PieceRow>();
    [SerializeField] private Piece piecePrefab;
    [SerializeField] private Button endTurnButton;

    protected override void OnShow() {
        gameObject.SetActive(true);

        takePieceNetworkEvent.OnEventRaised += OnTakePiece;
        newPlayerTurnEvent.OnEventRaised += OnNewPlayerTurn;
        endTurnButton.onClick.AddListener(EndTurn);

        SpawnPieces();
        for (int i = 0; i < rows.Count; i++) {
            rows[i].OnTakePiece.AddListener(OnTryTakePiece);
        }

        bool isMyTurn = TransportNetwork.LocalPlayer.IsAtTurn();
        rows.ForEach(x => x.Interactable = isMyTurn);
    }

    protected override void OnHide() {
        for (int i = 0; i < rows.Count; i++) {
            rows[i].OnTakePiece.RemoveListener(OnTryTakePiece);
        }

        takePieceNetworkEvent.OnEventRaised -= OnTakePiece;
        newPlayerTurnEvent.OnEventRaised -= OnNewPlayerTurn;
        endTurnButton.onClick.RemoveListener(EndTurn);

        gameObject.SetActive(false);
    }

    private void OnNewPlayerTurn(Player player) {
        rows.ForEach(x => x.Interactable = player.IsLocal);
        endTurnButton.interactable = false;
        
    }

    private void OnTakePiece(int rowIndex) {
        rows[rowIndex].TakePiece();

        if (!TransportNetwork.LocalPlayer.IsAtTurn()) { return; }

        endTurnButton.interactable = true;
        if (AreAllPiecesTaken()) {
            lastPieceTakenNetworkEvent.NetworkRaise(TransportNetwork.LocalPlayer, ReceiverGroup.All);
        } else if (!rows[rowIndex].HasPiecesLeft) {
            TransportNetwork.LocalPlayer.EndTurn();
        }
    }

    private void OnTryTakePiece(PieceRow pieceRow) {
        if (!TransportNetwork.LocalPlayer.IsAtTurn()) { return; }
        rows.ForEach(x => {
            if (x != pieceRow) {
                x.Interactable = false;
            }
        });
        takePieceNetworkEvent.NetworkRaise(rows.IndexOf(pieceRow), ReceiverGroup.All);
    }

    private void EndTurn() {
        if (TransportNetwork.LocalPlayer.IsAtTurn()) {
            TransportNetwork.LocalPlayer.EndTurn();
        }
    }

    private void SpawnPieces() {
        int piecesPerRow = 1;
        for (int i = 0; i < rows.Count; i++) {
            rows[i].SpawnPieces(piecePrefab, i, piecesPerRow);
            piecesPerRow += 2;
        }
    }

    private bool AreAllPiecesTaken() {
        return rows.TrueForAll(x => !x.HasPiecesLeft);
    }

}