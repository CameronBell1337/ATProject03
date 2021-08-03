using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{

    public static MapManager playerG;

    public TerrainManager terrain;

    public GameObject Player;

    private List<Vector2> inactiveTiles = new List<Vector2>();
    private List<Vector2> tempInactiveTiles = new List<Vector2>();
    private Vector2[] drawDistanceTiles;
    private Vector2 currentCoords;

    public mapTile currentTile;

    private List<mapTile> tiles = new List<mapTile>();
    void Start()
    {
        if (playerG == null)
        {
            playerG = this;
        }

        Player = GameObject.FindGameObjectWithTag("Player").gameObject;

        tiles = terrain.tiles;
        currentCoords = GetCoords(gameObject);

        drawDistanceTiles = new Vector2[9]; //the tiles around the player
        GetDrawTiles(currentCoords);
    }

    private void FixedUpdate()
    {
        Vector2 temp = GetCoords(Player);

        if(temp != currentCoords && temp != new Vector2(-1,-1))
        {
            currentCoords = temp;
            GetDrawTiles(currentCoords);
        }
    }


    private Vector2 GetCoords(GameObject obj)
    {
        foreach (mapTile t in tiles)
        {
            if (obj.transform.position.x >= t.wTransform.x
                && obj.transform.position.z >= t.wTransform.z
                && obj.transform.position.x < t.wTransform.x + terrain.tSize
                && obj.transform.position.z < t.wTransform.z + terrain.tSize)
            {
                if (obj = gameObject)
                {
                    currentTile = t;
                }
                return t.coords;
            }
        }

        return new Vector2(-1, -1);
    }

    private void GetDrawTiles(Vector2 currentCoord)
    {
        if (inactiveTiles.Count > 0)
        {
            inactiveTiles.Clear();
        }
        foreach (mapTile tile in tiles)
        {
            inactiveTiles.Add(tile.coords);
        }
        // 6 7 0
        // 5 P 1
        // 4 3 2
        drawDistanceTiles[0] = new Vector2(currentCoord.x + 1, currentCoord.y + 1);
        drawDistanceTiles[1] = new Vector2(currentCoord.x + 1, currentCoord.y);
        drawDistanceTiles[2] = new Vector2(currentCoord.x + 1, currentCoord.y - 1);
        drawDistanceTiles[3] = new Vector2(currentCoord.x, currentCoord.y - 1);
        drawDistanceTiles[4] = new Vector2(currentCoord.x - 1, currentCoord.y - 1);
        drawDistanceTiles[5] = new Vector2(currentCoord.x - 1, currentCoord.y);
        drawDistanceTiles[6] = new Vector2(currentCoord.x - 1, currentCoord.y + 1);
        drawDistanceTiles[7] = new Vector2(currentCoord.x, currentCoord.y + 1);
        //player tile
        drawDistanceTiles[8] = new Vector2(currentCoord.x, currentCoord.y);

        inactiveTiles.Remove(drawDistanceTiles[0]);
        inactiveTiles.Remove(drawDistanceTiles[1]);
        inactiveTiles.Remove(drawDistanceTiles[2]);
        inactiveTiles.Remove(drawDistanceTiles[3]);
        inactiveTiles.Remove(drawDistanceTiles[4]);
        inactiveTiles.Remove(drawDistanceTiles[5]);
        inactiveTiles.Remove(drawDistanceTiles[6]);
        inactiveTiles.Remove(drawDistanceTiles[7]);
        inactiveTiles.Remove(drawDistanceTiles[8]);

        StartCoroutine(ToggleObjs());


    }

    private IEnumerator ToggleObjs()
    {
        //Attempt to fix the modification error
        lock (drawDistanceTiles)
        {
            for (int j = 0; j < drawDistanceTiles.Length; j++)
            {

                mapTile tile = new mapTile();

                Vector2 tileVec = drawDistanceTiles[j];

                for (int i = 0; i < tiles.Count; i++)
                {
                    if (tiles[i].coords == tileVec)
                    {
                        tile = tiles[i];
                        break;
                    }
                }
                yield return null;
                Objects objContainer = ObjectsClass.Load("Assets/Resources/mapTile" + tile._id.ToString() + ".xml");
                StartCoroutine(terrain.SetObjectsActive(tile, true, objContainer));
            }
        }
        //Attempt to fix the modification error
        lock (inactiveTiles)
        {
            for (int j = 0; j < inactiveTiles.Count; j++)
            {
                mapTile tile = new mapTile();

                Vector2 temp = inactiveTiles[j];

                for (int i = 0; i < tiles.Count; i++)
                {
                    if (tiles[i].coords == temp)
                    {
                        tile = tiles[i];
                        break;
                    }
                }
                yield return null;
                Objects objContainer = ObjectsClass.Load("Assets/Resources/mapTile" + tile._id.ToString() + ".xml");
                StartCoroutine(terrain.SetObjectsActive(tile, false, objContainer));
            }
        }

    }
}
