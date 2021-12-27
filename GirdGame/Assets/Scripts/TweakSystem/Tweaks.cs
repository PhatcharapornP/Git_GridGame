using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Tweaks", menuName = "ScriptableObjects/Tweak", order = 1)]
public class Tweaks : ScriptableObject
{
   public int MaxBoardNum;
   public int MinBoardNum;

   public int startColumnSize;
   public int startRowSize;

   public float startTimer = 50;
   public float discoPieceTimeBonus = .10f;
   public float bombPieceTimeBonus = .5f;
   public float discoPieceBonus = .5f;
   public float bombPieceBonus = .2f;
   
   public Color[] levelOne;

   public Color[] levelTwo;

   public Color[] levelThree;

}
