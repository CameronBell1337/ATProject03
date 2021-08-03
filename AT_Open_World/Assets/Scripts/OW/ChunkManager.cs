using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{
    [Header("Terrain Object"), Tooltip("What will be rendered")]
    public terrain_type terrainType;
    public GameObject meshObj;

    public Material floor;

    public GameObject tree;

    public Vector2[] test;

    public enum terrain_type
    {
        cube = 0,
        plane,
    };

    [Header("Terrain Dimentions"), Tooltip("The max size the playable area can be")]
    [SerializeField]
    [Range(1, 10000)]
    private int playArea = 50;

    

    [Header("Max Chunk Size"), Tooltip("The size each chunk will be")]
    [SerializeField]
    [Range(1, 586)]
    private int chunkSize = 241;

    [Header("Max View Distance"), Tooltip("For far away can you see terrain")]
    [SerializeField]
    [Range(0, 2000)]
    public float viewDist = 750;


    public Camera mainCam;

///####################- Non-Inspector Variables -###################///


    private int chunksVisible;
    
    public GameObject player;
    private PlayerController controller;

    public static Vector3 playerPos;
    public static float viewDistance;

    public Dictionary<Vector3, Terrain> terrainChunkDict = new Dictionary<Vector3, Terrain>();
    static List<Terrain> terrainNotInRange = new List<Terrain>();
    //static List<Terrain> terrainNotVisible = new List<Terrain>();

    



    public GameObject[] treesM;

    public float radius = 1;
    public Vector2 regionSize = Vector2.one;
    public int rejectionSamples = 5;
    public float displayRadius = 1;

    List<Vector2> points = new List<Vector2>();


    

    void Start()
    {
        //objectT = GameObject.FindGameObjectWithTag("parentObj").transform;
        points = TreeManager.Create(radius, regionSize, rejectionSamples);
        viewDistance = viewDist;
        switch (terrainType)
        {
            case terrain_type.cube:
                {
                    meshObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    meshObj.GetComponent<MeshRenderer>().material = floor;
                    break;
                }
            case terrain_type.plane:
                {

                    meshObj = GameObject.CreatePrimitive(PrimitiveType.Plane);
                    meshObj.GetComponent<MeshRenderer>().material = floor;
                    break;
                }
            default:
                {
                    break;
                }
        }

        //player = GameObject.FindGameObjectWithTag("Player").transform;
        controller = FindObjectOfType<PlayerController>();
        chunksVisible = Mathf.RoundToInt(viewDistance / chunkSize);

        player = GameObject.FindGameObjectWithTag("Player").gameObject;

        //SaveObjects();
    }
    void Update()
    {
        playerPos = new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z);
        if (controller.GetVelocity() != 0)
        {
            CheckChunksVisible();
        }
        
    }


    void CheckChunksVisible()
    {
        for (int i = 0; i < terrainNotInRange.Count; i++)
        {
            terrainNotInRange[i].SetVis(false);
        }
        terrainNotInRange.Clear();

        int currentChunkPosX = Mathf.RoundToInt(playerPos.x / chunkSize);
        int currentChunkPosY = Mathf.RoundToInt(playerPos.y / chunkSize);
        int currentChunkPosZ = Mathf.RoundToInt(playerPos.z / chunkSize);


        if(currentChunkPosX >= playArea)
        {
            currentChunkPosX = playArea;
        }
        else if( currentChunkPosX <= -playArea)
        {
            currentChunkPosX = -playArea;
        }

        if (currentChunkPosY >= playArea)
        {
            currentChunkPosY = playArea;
        }
        else if (currentChunkPosY <= -playArea)
        {
            currentChunkPosY = -playArea;
        }

        if (currentChunkPosZ >= playArea)
        {
            currentChunkPosZ = playArea;
        }
        else if (currentChunkPosZ <= -playArea)
        {
            currentChunkPosZ = -playArea;
        }

        for (int xC = -chunksVisible; xC <= chunksVisible; xC++)
        {
            for (int yC = -chunksVisible; yC <= chunksVisible; yC++)
            {
                Vector3 checkChunkCOORD = 
                    new Vector3(currentChunkPosX + xC, 0, currentChunkPosZ + yC);
                if (terrainChunkDict.ContainsKey(checkChunkCOORD))
                {
                    terrainChunkDict[checkChunkCOORD].UpdateChunk();
                    terrainChunkDict[checkChunkCOORD].Tree();
                    if (terrainChunkDict[checkChunkCOORD].IsVis())
                    {
                        terrainNotInRange.Add(terrainChunkDict[checkChunkCOORD]);
                    }
                }
                else
                {
                    terrainChunkDict.Add(checkChunkCOORD, 
                        new Terrain(checkChunkCOORD, 
                        chunkSize, transform, meshObj, treesM, points));
                }
            }
        }
    }

    public void CheckPlayArea()
    {

    }

    
    public class Terrain
    {
        public GameObject mesh;
        Vector3 pos;
        Bounds bounds;
        Renderer renderer;
        GameObject[] treeM;
        List<Vector2> points;
        int count;

        bool test;



        public Terrain(Vector3 _coord, int _size, Transform _parent, GameObject _obj, GameObject[] tree, List<Vector2> _points)
        {
            pos = _coord * _size;
            bounds = new Bounds(pos, Vector3.one * _size);
            Vector3 position = new Vector3(pos.x, pos.y, pos.z);

            mesh = Instantiate(_obj);
            renderer = mesh.GetComponent<Renderer>();
            count = 0;
            treeM = tree;
            points = _points;

            mesh.transform.position = position;
            mesh.transform.localScale = new Vector3(1 * _size / 10f, 1, 1 * _size / 10f);
            mesh.transform.parent = _parent;
            Tree();

            test = false;
            SetVis(false);



            bounds = new Bounds(pos, Vector3.one * _size);
        }
        public void UpdateChunk()
        {
            float playerDistFromEdge = Mathf.Sqrt(bounds.SqrDistance(playerPos));
            bool visible = playerDistFromEdge <= viewDistance;
            SetVis(visible);


        }

        public void SetVis(bool visible)
        {
            mesh.SetActive(visible);
            
        }

        public bool IsVis()
        {
            return mesh.activeSelf;
        }

        public bool end()
        {
            return test;
        }

        public void Tree()
        {
            if (!test)
            {
                foreach (Vector2 p in points)
                {
                    int index = Random.Range(0, treeM.Length);
                    GameObject trees = Instantiate(treeM[index], new Vector3(p.x, 0, p.y) + mesh.transform.position, Quaternion.identity) as GameObject;
                    trees.transform.parent = mesh.transform;
                    count++;
                    if (count == 30)
                    {
                        test = true;
                        break;


                    }



                }
            }
            else
            {
                return;
            }



        }


      
    }
}

