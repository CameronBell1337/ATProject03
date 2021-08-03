using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mapTile
{
    public bool isTileLoaded = false;

    //Tile GameObject
    public GameObject tObj;

    //Map grid coord
    public Vector2 coords;

    //Position of the tile in world space
    public Vector3 wTransform;

    public int _id;

    //Lists to the game objects within that tile
    public List<SceneObjs> obstacles = new List<SceneObjs>();
    public List<SceneObjs> trees = new List<SceneObjs>();
    public List<SceneObjs> ground = new List<SceneObjs>();
    public List<GameObject> objs = new List<GameObject>();


}
