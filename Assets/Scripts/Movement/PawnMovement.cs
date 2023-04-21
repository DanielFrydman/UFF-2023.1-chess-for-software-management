using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PawnMovement : Movement
{
    public override List<Tile> GetValidMoves(){
        List<Vector2Int> temp = new List<Vector2Int>();
        Vector2Int direction = GetDirection();
        temp.Add(Board.instance.selectedPiece.tile.pos + direction);
        return ValidateExists(temp);
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
}
