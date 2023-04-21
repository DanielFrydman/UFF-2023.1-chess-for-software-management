using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Piece : MonoBehaviour
{
    public Tile tile;
    void OnMouseDown(){ 
        Board.instance.tileClicked(this, transform.parent.GetComponent<Player>());
    }
}
