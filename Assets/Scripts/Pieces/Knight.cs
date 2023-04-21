using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : Piece
{
   void Awake(){
      movement = new KnightMovement();
    }
}
