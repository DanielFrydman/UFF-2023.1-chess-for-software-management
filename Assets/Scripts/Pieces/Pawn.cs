using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : Piece
{
    protected override void Start(){
        base.Start();
        movement = new PawnMovement(GetDirection());
    }
    Vector2Int GetDirection(){
        if(Board.instance.selectedPiece.transform.parent.name == "GreenPieces")
            return new Vector2Int(0, -1);
        return new Vector2Int(0, 1);
    }
}
