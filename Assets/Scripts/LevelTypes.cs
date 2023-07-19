using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum levelTypes
{
    Tutorial,
    Torus,
    Pencil,
    Book,
    Field,
    Tree,
    Train,
    Car
};

public class LevelTypes : MonoBehaviour
{
   
    [SerializeField] private levelTypes levelType;

    public levelTypes GetSelectedLevelType()
    {
        return levelType;
    }
 
}
