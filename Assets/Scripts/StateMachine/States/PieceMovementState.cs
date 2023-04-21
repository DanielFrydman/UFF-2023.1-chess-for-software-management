using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class PieceMovementState : State
{
    public override async void Enter(){
        Piece piece = Board.instance.selectedPiece;
        piece.tile.content = null;
        piece.tile = Board.instance.selectedHighlight.tile;

        if(piece.tile.content != null){
            Piece deadPiece = piece.tile.content;
            deadPiece.gameObject.SetActive(false);
        }

        piece.tile.content = piece;

        TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
        float timing = Vector3.Distance(piece.transform.position, Board.instance.selectedHighlight.transform.position)*0.5f;
        LeanTween.move(piece.gameObject, Board.instance.selectedHighlight.transform.position, timing).
            setOnComplete(()=> {
                tcs.SetResult(true);
            });
        await tcs.Task;
        machine.ChangeTo<TurnEndState>();
    }
}
