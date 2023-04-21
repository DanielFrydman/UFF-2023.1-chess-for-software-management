using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class PieceMovementState : State
{
    public override async void Enter(){
        Piece piece = Board.instance.selectedPiece;
        piece.transform.position = Board.instance.selectedHighlight.transform.position;
        piece.tile.content = null;
        piece.tile = Board.instance.selectedHighlight.tile;

        if(piece.tile.content != null){
            Piece deadPiece = piece.tile.content;
            Debug.LogFormat("peça {0} foi morta", deadPiece.transform);
            deadPiece.gameObject.SetActive(false);
        }

        piece.tile.content = piece;

        await Task.Delay(100);
        machine.ChangeTo<TurnEndState>();
    }
}
