using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PieceRow : MonoBehaviour {

    public readonly UnityEvent<PieceRow> OnTakePiece = new UnityEvent<PieceRow>();

    public bool HasPiecesLeft => pieces.Count > 0;
    public bool Interactable {
        get => takeButton.interactable;
        set => takeButton.interactable = value;
    }

    [SerializeField] private Button takeButton;

    private Stack<Piece> pieces = new Stack<Piece>();

    public NimCoordinate TakePiece() {
        Piece lastPiece = pieces.Pop();
        lastPiece.gameObject.SetActive(false);

        if (pieces.Count <= 0) {
            takeButton.interactable = false;
        }

        return lastPiece.Coordinate;
    }

    public void SpawnPieces(Piece piecePrefab, int row, int amount) {
        for (int i = 0; i < amount; i++) {
            Piece instantiatedPiece = Instantiate(piecePrefab, transform, false);
            instantiatedPiece.Coordinate = new NimCoordinate(row, i);

            pieces.Push(instantiatedPiece);
        }
    }

    private void OnEnable() {
        takeButton.onClick.AddListener(OnTakeButtonClicked);
    }

    private void OnDisable() {
        takeButton.onClick.RemoveListener(OnTakeButtonClicked);
    }

    private void OnTakeButtonClicked() {
        OnTakePiece.Invoke(this);
    }

}