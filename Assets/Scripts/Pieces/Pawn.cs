using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : Piece
{
    public Movement savedMovement;
    public Movement queenMovement = new QueenMovement();
    public Movement knightMovement = new KnightMovement();
    protected override void Start(){
        base.Start();
        movement = savedMovement = new PawnMovement(GetDirection());
    }
    public override AffectedPiece CreateAffected(){
        AffectedPawn aff = new AffectedPawn();
        aff.wasMoved = wasMoved;
        return aff;
    }
    Vector2Int GetDirection(){
        if(maxTeam)
            return new Vector2Int(0, 1);
        return new Vector2Int(0, -1);
    }
}
