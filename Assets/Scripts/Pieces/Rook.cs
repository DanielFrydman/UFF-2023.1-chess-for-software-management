using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rook : Piece
{
   void Awake(){
      movement = new RookMovement();
    }
}
