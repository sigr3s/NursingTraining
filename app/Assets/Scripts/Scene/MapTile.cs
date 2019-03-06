using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTile : MonoBehaviour
{
    [SerializeField] private int _id;

    public bool blocked = false;


    public int id{
        get{
            return _id;
        }

        set{
            _id = value;
            name = "Tile: "+ id;
        }
    }
}
