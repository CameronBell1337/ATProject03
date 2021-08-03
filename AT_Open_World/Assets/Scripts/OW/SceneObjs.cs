using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;

[System.Serializable]
public class SceneObjs
{
    public enum Type
    {
        ground = 8,
        obstacle = 9,
        tree = 10
    }

    public string name;
    public string path;
    public Vector3 position;
    public Vector3 rotation;
    public Vector3 scale;
    public Vector3 tilePosition;
    public Vector2 coordinate;
    public Type type;

    /// <summary>
    /// Sets values
    /// </summary>
    /// <param name="_n">string name</param>
    /// <param name="_p">path directory</param>
    /// <param name="_pos">position of transform</param>
    /// <param name="_rot">rotation of object</param>
    /// <param name="_s">scale of object</param>
    /// <param name="_tilePos">tile position</param>
    /// <param name="_coord">grid coords</param>
    /// <param name="_t">type of object</param>
    public void SetValues(string _n, string _p,
        Vector3 _pos, Vector3 _rot, Vector3 _s,
        Vector3 _tilePos, Vector2 _coord, Type _t)
    {
        name = _n;
        path = _p;
        position = _pos;
        rotation = _rot;
        scale = _s;
        tilePosition = _tilePos;
        coordinate = _coord;
        type = _t;
    }
}
