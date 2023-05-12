using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class AIController : MonoBehaviour
{
    public static AIController instance;
    public Ply currentState;
    public HighlightClick AIhighlight;
    int calculationCount;
    public int objectivePlyDepth = 2;
    float lastInterval;
    void Awake(){
        instance = this;
    }
    [ContextMenu("Calculate Plays")]
    public async void CalculatePlays(){
        lastInterval = Time.realtimeSinceStartup;
        int minimaxDirection = 1;
        currentState = CreateSnapShot();
        calculationCount = 0;

        Ply currentPly = currentState;
        currentPly.originPly = null;
        int currentPlyDepth = 0;
        currentPly.changes = new List<AffectedPiece>();

        Task<Ply> calculation = CalculatePly(
            currentPly,
            GetTeam(currentPly, minimaxDirection),
            currentPlyDepth,
            minimaxDirection
        );
        await calculation;
        currentPly.bestFuture = calculation.Result;

        Debug.Log("calculations"+calculationCount);
        Debug.Log("time:"+(Time.realtimeSinceStartup-lastInterval));
        PrintBestPly(currentPly.bestFuture);
    }
    async Task<Ply> CalculatePly(Ply parentPly, List<PieceEvaluation> team, int currentPlyDepth, int minimaxDirection){
        parentPly.futurePlies = new List<Ply>();

        currentPlyDepth++;
        if(currentPlyDepth > objectivePlyDepth){
            EvaluateBoard(parentPly);
            // Task evaluationTask = Task.Run(() => EvaluateBoard(parentPly));
            // await evaluationTask;
            return parentPly;
        }
        Ply plyceHolder = new Ply();
        plyceHolder.score = -9999999 * minimaxDirection;
        parentPly.bestFuture = plyceHolder;

        foreach(PieceEvaluation eva in team){
            foreach(Tile t in eva.availableMoves){
                calculationCount++;
                Board.instance.selectedPiece = eva.piece;
                Board.instance.selectedHighlight = AIhighlight;
                AIhighlight.tile = t;
                AIhighlight.transform.position = new Vector3(t.pos.x, t.pos.y, 0);
                TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
                PieceMovementState.MovePiece(tcs, true);
                await tcs.Task;
                Ply newPly = CreateSnapShot(parentPly);
                newPly.changes = PieceMovementState.changes;
                Task<Ply> calculation = CalculatePly(
                    newPly,
                    GetTeam(newPly, minimaxDirection * -1),
                    currentPlyDepth,
                    minimaxDirection * -1
                );
            await calculation;
                parentPly.bestFuture = IsBest(parentPly.bestFuture, minimaxDirection, calculation.Result);
                newPly.moveType = t.moveType;
                newPly.originPly = parentPly;
                parentPly.futurePlies.Add(newPly);
                ResetBoardBackwards(newPly);
            }
        }
        return parentPly.bestFuture;
    }
    List<PieceEvaluation> GetTeam(Ply ply, int minimaxDirection){
        if(minimaxDirection == 1)
            return ply.golds;
        else
           return ply.greens;
    }
    Ply IsBest(Ply ply, int minimaxDirection, Ply potentialBest){
        if(minimaxDirection == 1){
            if(potentialBest.score > ply.score)
                return potentialBest;
            return ply;
        }else{
            if(potentialBest.score < ply.score)
                return potentialBest;
            return ply;
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
    Ply CreateSnapShot(Ply parentPly){
        Ply ply = new Ply();
        ply.golds = new List<PieceEvaluation>();
        ply.greens = new List<PieceEvaluation>();

        foreach(PieceEvaluation p in parentPly.golds){
            if(p.piece.gameObject.activeSelf){
                ply.golds.Add(CreateEvaluationPiece(p.piece, ply));
            }
        }
        foreach(PieceEvaluation p in parentPly.greens){
            if(p.piece.gameObject.activeSelf){
                ply.greens.Add(CreateEvaluationPiece(p.piece, ply));
            }
        }
        return ply;
    }
    PieceEvaluation CreateEvaluationPiece(Piece piece, Ply ply){
        PieceEvaluation eva = new PieceEvaluation();
        eva.piece = piece;
        Board.instance.selectedPiece = eva.piece;
        eva.availableMoves = eva.piece.movement.GetValidMoves();
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
        ply.score += eva.piece.movement.value*scoreDirection;
    }
    void ResetBoardBackwards(Ply ply){
        foreach(AffectedPiece p in ply.changes){
            p.piece.tile.content = null;
            p.piece.tile = p.from;
            p.from.content = p.piece;
            // p.piece.transform.position = new Vector3(p.from.pos.x, p.from.pos.y, 0);
            p.piece.gameObject.SetActive(true);
        }
    }
    void PrintBestPly(Ply finalPly){
        Ply currentPly = finalPly;
        Debug.Log("melhor jogada:");
        while(currentPly.originPly != null) {
            Debug.LogFormat("{0}~{1}->{2}",
                currentPly.changes[0].piece.transform.parent.name,
                currentPly.changes[0].piece.name,
                currentPly.changes[0].to.pos
            );
        }
    }
}
