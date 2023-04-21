using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceSelectionState : State
{
    public override void Enter(){
        Board.instance.tileClicked += PiceClicked;
    }
    public override void Exit(){
        Board.instance.tileClicked -= PiceClicked;
    }
    void PiceClicked(object sender, object args){
        Piece piece = sender as Piece;
        Player player = args as Player;
        if(machine.currentlyPlaying == player){
            Board.instance.selectedPiece = piece;
            machine.ChangeTo<MoveSelectionState>();
        }
    }
}
