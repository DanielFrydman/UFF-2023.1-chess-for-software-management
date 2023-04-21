using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PawnMovement : Movement
{
    public override List<Tile> GetValidMoves(){
        List<Vector2Int> temp = new List<Vector2Int>();
        Vector2Int direction = GetDirection();
        temp.Add(Board.instance.selectedPiece.tile.pos + direction);
        List<Tile> exists = ValidateExists(temp);
        List<Tile> movable = UntilBlockedPath(exists);
        movable.AddRange(GetPawnAttack(direction));
        return movable;
    }
    Vector2Int GetDirection(){
        if(StateMachineController.instance.currentlyPlaying.transform.name == "GreenPieces")
            return new Vector2Int(0, -1);
        return new Vector2Int(0, 1);
    }
    List<Tile> ValidateExists(List<Vector2Int> positions){
        List<Tile> rtv = new List<Tile>();
        foreach (Vector2Int pos in positions){
            Tile tile;
            if(Board.instance.tiles.TryGetValue(pos, out tile)){
                rtv.Add(tile);
            }
        }
        return rtv;
    }
    List<Tile> UntilBlockedPath(List<Tile> positions){
        List<Tile> valid = new List<Tile>();
        for(int i=0; i < positions.Count; i++){
            if(positions[i].content == null)
                valid.Add(positions[i]);
        }
        return valid;
    }
    bool IsEnemy(Vector2Int pos, out Tile temp){
        if(Board.instance.tiles.TryGetValue(pos, out temp)){
            if(temp != null && temp.content != null){
                if(temp.content.transform.parent != Board.instance.selectedPiece.transform.parent)
                    return true;
            }
        }
        return false;
    }
    List<Tile> GetPawnAttack(Vector2Int direction){
        List<Tile> pawnAttack = new List<Tile>();
        Tile temp;
        Piece piece = Board.instance.selectedPiece;
        Vector2Int leftPos = new Vector2Int(piece.tile.pos.x - 1, piece.tile.pos.y + direction.y);
        Vector2Int rightPos = new Vector2Int(piece.tile.pos.x + 1, piece.tile.pos.y + direction.y);
        if(IsEnemy(leftPos, out temp)){
            pawnAttack.Add(temp);
        }
        if(IsEnemy(rightPos, out temp)){
            pawnAttack.Add(temp);
        }
        return pawnAttack;
    }
}
