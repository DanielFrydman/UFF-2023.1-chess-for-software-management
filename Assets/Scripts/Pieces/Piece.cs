using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Piece : MonoBehaviour
{
    public Tile tile;
    void OnMouseDown(){
        
    }
    void Start(){
        Board.instace.AddPiece(transform.parent.name, this);
    }
}
