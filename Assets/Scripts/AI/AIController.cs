using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class AIController : MonoBehaviour
{
    public static AIController instance;
    public Ply currentState;
    public HighlightClick AIhighlight;
    public Ply minPly;
    public Ply maxPly;
    public int objectivePlyDepth = 2;
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

        maxPly = new Ply();
        maxPly.score = float.MinValue;
        minPly = new Ply();
        minPly.score = float.MaxValue;

        int currentPlyDepth = 1;
        CalculatePly(currentPly, currentPly.golds, currentPlyDepth);
        currentPlyDepth++;
        foreach(Ply plyToTest in currentPly.futurePlies){
            CalculatePly(plyToTest, plyToTest.greens, currentPlyDepth);
        }

        currentPly.futurePlies.Sort((x, y) => x.score.CompareTo(y.score));
        Debug.Log("escolher "+maxPly.name);
    }
    async void CalculatePly(Ply parentPly, List<PieceEvaluation> team, int currentPlyDepth){
        parentPly.futurePlies = new List<Ply>();
        foreach(PieceEvaluation eva in team){
            foreach(Tile t in eva.availableMoves){
                Board.instance.selectedPiece = eva.piece;
                Board.instance.selectedHighlight = AIhighlight;
                AIhighlight.tile = t;
                AIhighlight.transform.position = new Vector3(t.pos.x, t.pos.y, 0);
                TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
                PieceMovementState.MovePiece(tcs, true);
                await tcs.Task;
                Ply newPly = CreateSnapShot();
                newPly.name = string.Format("{0}, {1} to {2}", parentPly.name, eva.piece.transform.parent.name+eva.piece.name, t.pos);
                newPly.changes = PieceMovementState.changes;
                EvaluateBoard(newPly);
                newPly.originPly = parentPly;
                newPly.moveType = t.moveType;
                parentPly.futurePlies.Add(newPly);
                ResetBoard(newPly);
            }
        }
        if(currentPlyDepth == objectivePlyDepth)
            SetMinMax(parentPly.futurePlies);
    }
    void SetMinMax(List<Ply> plies){
        foreach(Ply p in plies){
            if(p.score > maxPly.score)
                maxPly = p;
            else if(p.score < minPly.score)
                minPly = p;
        }
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
