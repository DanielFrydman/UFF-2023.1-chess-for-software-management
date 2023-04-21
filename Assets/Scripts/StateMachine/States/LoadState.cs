using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class LoadState : State
{
    public override async void Enter(){
        await Board.instance.Load();
        await LoadAllPiecesAsync();
        machine.currentlyPlaying = machine.player2;
        machine.ChangeTo<TurnBeginState>();
    }
    async Task LoadAllPiecesAsync(){
        LoadTeamPieces(Board.instance.greenPieces);
        LoadTeamPieces(Board.instance.goldPieces);
        await Task.Delay(100);
    }
    void LoadTeamPieces(List<Piece> pieces){
        foreach(Piece piece in pieces){
            Board.instance.AddPiece(piece.transform.parent.name, piece);
        }
    }
}
