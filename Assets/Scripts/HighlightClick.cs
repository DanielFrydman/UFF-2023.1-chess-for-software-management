using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightClick : MonoBehaviour
{
    public Tile tile;
    void OnMouseDown(){
        InputController.instance.tileClicked(this, null);
    }
}
