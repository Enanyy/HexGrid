using UnityEngine;
using System.Collections;

public class HexTile 
{
    public TileIndex index { get; set; }
   
    public Vector3 localPosition
    {
        get { return grid.TilePosition(index); }
    }
   
    public Vector3 position
    {
        get { return grid.TilePosition(index,true); }
    }
    public HexGrid grid { get; set; }

    public GameObject gameObject { get; private set; }

    private static Mesh mesh;

    private MeshRenderer mRenderer;

    public void SetMaterial(Material material)
    {
        if(mRenderer != null)
        {
            
            mRenderer.material = material;
        }
    }

    public void SetColor(Color color)
    {
        if(mRenderer)
        {
            mRenderer.material.color = color;
        }
    }

    public void OnCreate()
    {
        gameObject = new GameObject(index.ToString());
        gameObject.transform.SetParent(grid.root);
        gameObject.transform.localPosition = localPosition;

        MeshFilter filter = gameObject.AddComponent<MeshFilter>();
        mRenderer = gameObject.AddComponent<MeshRenderer>();

        if(mesh == null)
        {
            mesh = HexGrid.GenerateHexMesh(grid.radius, grid.orientation);
        }
        filter.sharedMesh = mesh;
        

    }
    public void OnDestory()
    {
        Object.Destroy(gameObject);
        gameObject = null;
    }
}
