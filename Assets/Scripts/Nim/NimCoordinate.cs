using UnityEngine;

public struct NimCoordinate {

    public int Row;
    public int Index;

    public NimCoordinate(int row, int index) {
        Row = row;
        Index = index;
    }

    public static implicit operator Vector2Int(NimCoordinate nimCoordinate) => new Vector2Int(nimCoordinate.Index, nimCoordinate.Row);

}