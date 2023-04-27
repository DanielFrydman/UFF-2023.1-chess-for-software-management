using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class PieceMovementState : State
{
    public override async void Enter(){
        TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
        switch(Board.instance.selectedHighlight.tile.moveType){
            case MoveType.Normal:
                NormalMove(tcs);
                break;
            case MoveType.Castling:
                Castling(tcs);
                break;
        }


        await tcs.Task;
        machine.ChangeTo<TurnEndState>();
    }
    void NormalMove(TaskCompletionSource<bool> tcs){
        Piece piece = Board.instance.selectedPiece;
        piece.tile.content = null;
        piece.tile = Board.instance.selectedHighlight.tile;

        if(piece.tile.content != null){
            Piece deadPiece = piece.tile.content;
            deadPiece.gameObject.SetActive(false);
        }

        piece.tile.content = piece;
        piece.wasMoved = true;

        float timing = Vector3.Distance(piece.transform.position, Board.instance.selectedHighlight.transform.position)*0.5f;
        LeanTween.move(piece.gameObject, Board.instance.selectedHighlight.transform.position, timing).
            setOnComplete(()=> {
                tcs.SetResult(true);
            });
    }
    void Castling(TaskCompletionSource<bool> tcs){
        Piece king = Board.instance.selectedPiece;
        king.tile.content = null;
        Piece rook = Board.instance.selectedHighlight.tile.content;
        rook.tile.content = null;

        Vector2Int direction = rook.tile.pos - king.tile.pos;
        if(direction.x>0){
            king.tile = Board.instance.tiles[new Vector2Int(king.tile.pos.x+2, king.tile.pos.y)];
            rook.tile = Board.instance.tiles[new Vector2Int(king.tile.pos.x-1, king.tile.pos.y)];

        }else{
            king.tile = Board.instance.tiles[new Vector2Int(king.tile.pos.x-2, king.tile.pos.y)];
            rook.tile = Board.instance.tiles[new Vector2Int(king.tile.pos.x+1, king.tile.pos.y)];
        }
        king.tile.content = king;
        rook.tile.content = rook;
        king.wasMoved = true;
        rook.wasMoved = true;

        LeanTween.move(king.gameObject, new Vector3(king.tile.pos.x, king.tile.pos.y, 0), 1.5f).
            setOnComplete(()=> {
                tcs.SetResult(true);
            });
        LeanTween.move(rook.gameObject, new Vector3(rook.tile.pos.x, rook.tile.pos.y, 0), 1.4f);

    }
}
