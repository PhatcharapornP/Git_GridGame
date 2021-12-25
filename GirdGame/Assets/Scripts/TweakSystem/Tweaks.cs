using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Tweaks", menuName = "ScriptableObjects/Tweak", order = 1)]
public class Tweaks : ScriptableObject
{
   public int MaxBoardNum;
   public int MinBoardNum;
   
   public Color[] levelOne;

   public Color[] levelTwo;

   public Color[] levelThree;

}
