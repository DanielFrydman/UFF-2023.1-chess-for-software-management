using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class TurnEndState : State
{
    public override async void Enter(){
        bool gameFinished = CheckTeams();
        await Task.Delay(100);
        if(gameFinished)
            machine.ChangeTo<GameEndState>();
        else
            machine.ChangeTo<TurnBeginState>();
    }
    bool CheckTeams(){
        Piece goldPiece = Board.instance.goldPieces.Find((x) => x.gameObject.activeSelf == true);
        if(goldPiece == null){
            return true;
        }

        Piece greenPiece = Board.instance.greenPieces.Find((x) => x.gameObject.activeSelf == true);
        if(greenPiece == null){
            return true;
        }

        return false;
    }
}
