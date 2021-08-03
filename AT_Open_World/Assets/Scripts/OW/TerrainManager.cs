using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Controls the grid of the map
public class TerrainManager : MonoBehaviour
{
    [HideInInspector]
    public Transform objectT;

    private List<Objects> objContainer = new List<Objects>();


    public GameObject tileP;
    public int tSize = 1;
    public Vector2 gridDims;

    public bool show = false;


    public List<mapTile> tiles = new List<mapTile>();
    private List<GameObject> objsInEntireScene = new List<GameObject>();
    private List<SceneObjs> sceneObj = new List<SceneObjs>();

    public void Start()
    {
        //assigns transform of parent obj
        objectT = GameObject.FindGameObjectWithTag("parentObj").transform;
        onCreate();
        
        //shows debugging outline of grid for testing
        if (!show)
        {
            foreach (mapTile t in tiles)
            {
                t.tObj.GetComponent<MeshRenderer>().enabled = true;
            }
        }
        foreach(GameObject obj in objsInEntireScene)
        {
            Destroy(obj);
        }
    }

    public void onCreate()
    {
        Transform[] allObjsInParent = objectT.GetComponentsInChildren<Transform>();
        foreach(Transform child in allObjsInParent)
        {
            if(child != objectT)
            {
                //gets all the children game objects 
                objsInEntireScene.Add(child.gameObject);
            }
        }

        var i = 0;
        for (var x = 0; x < gridDims.x; x++)
        {
            for(var y = 0; y < gridDims.y; y++)
            {
                /*Gets local transform posistion and multiplies by tile dimentions to get a whole number for grid checking
                 * Use X & Z here as vector2 uses (x,z). maybe worth switching to Vector3 to use height??
                 */
                Vector3 _p = new Vector3(transform.position.x + (x * tSize), 0.0f, transform.position.z + (y * tSize));

                

                GameObject _t = Instantiate(tileP, _p, Quaternion.identity, transform);
                _t.transform.localScale = new Vector3(tSize / 10.0f, 1, tSize / 10.0f);
                _t.name = _t.transform.position.ToString();

                //fills in the parameters for seralisation
                mapTile tile = new mapTile();
                tile.tObj = _t;
                tile.coords = new Vector2(x, y);
                tile.wTransform = _p;
                tile._id = i;
                i++;

                //Checks and loops for all the objects within the bounds of each individual tile
                GetObjsWithinMap(tile, _t);
                tiles.Add(tile);
            }
        }
        //Save the tile.xml files with the objects data
        SaveObjects();
    }

    public void SaveObjects()
    {
        for (var i = 0; i < objContainer.Count; i++)
        {
            //saves the file as name of obj and as an xml file
            ObjectsClass.Save(objContainer[i], "Assets/Resources/mapTile" + i.ToString() + ".xml");
        }
    }

    public void GetObjsWithinMap(mapTile _t, GameObject _g)
    {
        List<GameObject> temp = objsInEntireScene;
        objContainer.Add(new Objects());

        for (var i = 0; i < temp.Count; i++)
        {
            //https://docs.unity3d.com/ScriptReference/GameObject-layer.html use along with occlusion culling and finding objects
            if (temp[i].layer == 8 || 
                temp[i].layer == 9 || 
                temp[i].layer == 10)
            {
                //Bound checks
                if (temp[i].transform.position.x >= _t.wTransform.x && temp[i].transform.position.z >= _t.wTransform.z &&
                    temp[i].transform.position.x < _t.wTransform.x + tSize && temp[i].transform.position.z < _t.wTransform.z + tSize)
                {
                    string directory = GetDirectory(temp[i]);

                    SceneObjs newObj = new SceneObjs();

                    var name = temp[i].name;
                    var pos = temp[i].transform.position;
                    var rot = temp[i].transform.eulerAngles;
                    var scale = temp[i].transform.localScale;
                    var wPos = _t.wTransform;
                    var coord = _t.coords;
                    var type = (SceneObjs.Type)temp[i].layer;

                    newObj.SetValues(name, directory, pos, rot, scale, wPos, coord, type);
                    objContainer[_t._id].mapObjs.Add(newObj);

                    if(temp[i].layer == 8)
                    {
                        _t.ground.Add(newObj);
                    }else if(temp[i].layer == 9)
                    {
                        _t.obstacles.Add(newObj);
                    }
                    else if(temp[i].layer == 10)
                    {
                        _t.trees.Add(newObj);
                    }
                }

            }
            else
            {
                //Removes anything unnessasary gameobjects not included within the search
                objsInEntireScene.Remove(temp[i]);
            }
        }
    }

    public string GetDirectory(GameObject _o)
    {
        Object pObj = UnityEditor.PrefabUtility.GetCorrespondingObjectFromSource(_o);

        string _dir = UnityEditor.AssetDatabase.GetAssetPath(pObj);

        string path = "Assets/Resources/";
        if(_dir.Contains(path))
        {
            _dir = _dir.Replace(path, "");
        }

        string[] split = _dir.Split('.'); //O.o 
        _dir = split[0];

        return _dir;
    }

    //use a couroutine rather than have it in called in update allowing for it to be slightly more efficient
    public IEnumerator SetObjectsActive(mapTile t, bool active, Objects objCon)
    {
        if(active)
        {
            if(!t.isTileLoaded)
            {
                t.isTileLoaded = true;
                var i = 0;
                var subStep = 5; //sets the amount of loops per frame

                foreach (SceneObjs o in objCon.mapObjs)
                {

                   
                        GameObject _2load = Resources.Load<GameObject>(o.path);


                        //if there is an object to instantiate
                        if (_2load != null)
                        {
                            GameObject obj = Instantiate(_2load, objectT);
                            obj.transform.position = o.position;
                            obj.transform.eulerAngles = o.rotation;
                            obj.transform.localScale = o.scale;
                            obj.layer = (int)o.type;

                            t.objs.Add(obj);
                        }

                        i++;
                        if (i > subStep)
                        {
                            yield return null;
                            i = 0;
                        }
                    
                }
            }
        }
        else if(!active)
        {
            if(t.isTileLoaded)
            {
                List<GameObject> temp = new List<GameObject>();
                temp.InsertRange(0, t.objs);

                var i = 0;
                var subStep = 5;
                foreach(GameObject o in t.objs)
                {
                    //destory obj and remove from array
                    Destroy(o);
                    temp.Remove(o);

                    i++;
                    if(i > subStep)
                    {
                        yield return null;
                        i = 0;
                    }
                    t.objs = temp;
                }
                
            }
            t.isTileLoaded = false;
        }
        yield return null;
    }
}
