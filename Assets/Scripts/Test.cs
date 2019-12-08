using UnityEngine;
using System.Collections.Generic;

public class Test : MonoBehaviour
{
    public HexOrientation orientation = HexOrientation.Flat;
    public float radius = 10;
    public Material material;
    public int range = 10;

    public int width = 20;
    public int height = 20;
    HexGrid grid;
    HexGridShapeHex shapeHex;
    HexGridShapeRect shapeRect;
    HexGridTerritory territory;
    Dictionary<TileIndex, HexTile> tiles = new Dictionary<TileIndex, HexTile>();

    // Use this for initialization
    void Start()
    {
        CameraManager.Instance.Init();
        CameraManager.Instance.onMove -= OnMove;
        CameraManager.Instance.onMove += OnMove;
        CameraManager.Instance.onZoom -= OnZoom;
        CameraManager.Instance.onZoom += OnZoom;

        grid = new HexGrid(radius, orientation);
        grid.SetRoot(transform);

        //shapeHex = new HexGridShapeHex(grid);
        //shapeHex.UpdateTiles(CameraManager.Instance.center, range,OnCreateHexTile, OnRemoveHexTile);

        shapeRect = new HexGridShapeRect(grid);
        shapeRect.UpdateTiles(CameraManager.Instance.center, width, height, OnCreateHexTile, OnRemoveHexTile);

        territory = new HexGridTerritory(grid);
    }


    void OnMove()
    {
        //shapeHex.UpdateTiles(CameraManager.Instance.center, range,OnCreateHexTile, OnRemoveHexTile);
        shapeRect.UpdateTiles(CameraManager.Instance.center, width, height,OnCreateHexTile, OnRemoveHexTile);

    }

    void OnZoom(float distance)
    {

    }

    void OnCreateHexTile(TileIndex index)
    {
        HexTile tile = new HexTile();
        tile.index = index;
      
        tile.grid = grid;
        tile.OnCreate();

        tile.SetMaterial(material);

        tiles.Add(index, tile);
    }

    void OnRemoveHexTile(TileIndex index)
    {
        HexTile tile;
        if(tiles.TryGetValue(index, out tile))
        {
            tile.OnDestory();
            tiles.Remove(index);
        }
    }

    // Update is called once per frame
    bool isSetFrom = false;
    TileIndex from = TileIndex.zero;
    List<TileIndex> paths = new List<TileIndex>();
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 position = CameraManager.Instance.GetWorldMousePosition();
            TileIndex index = grid.IndexOf(position);
            territory.AddTile(index);
            if (isSetFrom == false)
            {
                from = index;
                isSetFrom = true;
            }
            else
            {
                isSetFrom = false;
                grid.FindPath(ref paths, from, index, (t) =>
                {
                    return true;
                });

                if (paths != null)
                {
                    for (int i = 0; i < paths.Count; ++i)
                    {
                        if (tiles.ContainsKey(paths[i]))
                        {
                            tiles[paths[i]].SetColor(Color.yellow);
                        }
                    }
                }
            }

            if (tiles.ContainsKey(index))
            {
                tiles[index].SetColor(Color.yellow);
            }
        }

        if(Input.GetKeyDown(KeyCode.T))
        {
            var verts = territory.GetTerritoryVertexs(true);
            for(int i = 0; i < verts.Count; i++)
            {
                GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                go.transform.position = verts[i];
                go.name = "vert-" + i;
            }
        }
    }
}
