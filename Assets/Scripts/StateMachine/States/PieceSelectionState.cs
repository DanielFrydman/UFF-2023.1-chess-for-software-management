using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceSelectionState : State
{
    public override void Enter(){
        InputController.instance.tileClicked += PiceClicked;
        SetColliders(true);
    }
    public override void Exit(){
        InputController.instance.tileClicked -= PiceClicked;
        SetColliders(false);
    }
    void PiceClicked(object sender, object args){
        Piece piece = sender as Piece;
        Player player = args as Player;
        if(machine.currentlyPlaying == player){
            Board.instance.selectedPiece = piece;
            machine.ChangeTo<MoveSelectionState>();
        }
    }

    /*void SetColliders(bool State){
        foreach(BoxCollider2D b in machine.currentlyPlaying.GetComponentInChildren<BoxCollider2D>()){
            b.enabled = state;
        }
    }   <-------------- forma que deveria estar e não funcionou*/

    void SetColliders(bool state){                                                                     //<--------------------Forma alterado funcionou
    BoxCollider2D[] colliders = machine.currentlyPlaying.GetComponentsInChildren<BoxCollider2D>();
    foreach(BoxCollider2D b in colliders){
        b.enabled = state;
        } 
    }
}
