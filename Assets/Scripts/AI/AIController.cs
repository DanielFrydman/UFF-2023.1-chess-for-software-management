using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class AIController : MonoBehaviour
{
    public static AIController instance;
    public Ply currentState;
    public HighlightClick AIhighlight;
    void Awake(){
        instance = this;
    }
    [ContextMenu("Calculate Plays")]
    public async void CalculatePlace(){
        currentState = CreateSnapShot();
        currentState.name = "start";
        EvaluateBoard(currentState);

        Ply currentPly = currentState;
        currentPly.originPly = null;
        currentPly.futurePlies = new List<Ply>();
        foreach(PieceEvaluation eva in currentPly.golds){
            foreach(Tile t in eva.availableMoves){
                Board.instance.selectedPiece = eva.piece;
                Board.instance.selectedHighlight = AIhighlight;
                AIhighlight.tile = t;
                AIhighlight.transform.position = new Vector3(t.pos.x, t.pos.y, 0);
                TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
                PieceMovementState.MovePiece(tcs, true);
                await tcs.Task;
                Ply newPly = CreateSnapShot();
                newPly.name = string.Format("{0}, {1} to {2}", currentPly.name, eva.piece.name, t.pos);
                newPly.changes = PieceMovementState.changes;
                EvaluateBoard(newPly);
                newPly.moveType = t.moveType;
                currentPly.futurePlies.Add(newPly);
                ResetBoard(newPly);
            }
        }
        currentPly.futurePlies.Sort((x, y) => x.score.CompareTo(y.score));
    }
    Ply CreateSnapShot(){
        Ply ply = new Ply();
        ply.golds = new List<PieceEvaluation>();
        ply.greens = new List<PieceEvaluation>();

        foreach(Piece p in Board.instance.goldPieces){
            if(p.gameObject.activeSelf){
                ply.golds.Add(CreateEvaluationPiece(p, ply));
            }
        }
        foreach(Piece p in Board.instance.greenPieces){
            if(p.gameObject.activeSelf){
                ply.greens.Add(CreateEvaluationPiece(p, ply));
            }
        }
        return ply;
    }
    PieceEvaluation CreateEvaluationPiece(Piece piece, Ply ply){
        PieceEvaluation eva = new PieceEvaluation();
        eva.piece = piece;
        return eva;
    }
    void EvaluateBoard(Ply ply){        
        foreach(PieceEvaluation piece in ply.golds){
            EvaluatePiece(piece, ply, 1);
        }
        foreach(PieceEvaluation piece in ply.greens){
            EvaluatePiece(piece, ply, -1);
        }
    }
    void EvaluatePiece(PieceEvaluation eva, Ply ply, int scoreDirection){
        Board.instance.selectedPiece = eva.piece;
        eva.availableMoves = eva.piece.movement.GetValidMoves();

        eva.score = eva.piece.movement.value;
        ply.score += eva.score*scoreDirection;
    }
    void ResetBoard(Ply ply){
        foreach(AffectedPiece p in ply.changes){
            p.piece.tile.content = null;
            p.piece.tile = p.from;
            p.from.content = p.piece;
            p.piece.transform.position = new Vector3(p.from.pos.x, p.from.pos.y, 0);
            p.piece.gameObject.SetActive(true);
        }
    }
}
