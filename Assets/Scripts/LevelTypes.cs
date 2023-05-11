using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum levelTypes
{
    Torus,
    Pencil,
    Book,
    Fruit
};

public class LevelTypes : MonoBehaviour
{
   
    [SerializeField] private levelTypes levelType;

    public levelTypes GetSelectedLevelType()
    {
        return levelType;
    }
 
}
