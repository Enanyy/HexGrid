using UnityEngine;
using System.Collections.Generic;

public class HexGridTerritory 
{
    public HexGrid grid { get; private set; }

    public List<TileIndex> tiles { get; private set; }

    private List<Vector3> mVertexs;
    
    public HexGridTerritory(HexGrid grid)
    {
        this.grid = grid;
        tiles = new List<TileIndex>();
        mVertexs = new List<Vector3>();
    }

    public bool AddTile(TileIndex index)
    {
        if(tiles.Contains(index)==false)
        {
            if (tiles.Count == 0)
            {
                tiles.Add(index);
                UpdateVertexs();
                return true;
            }
            else
            {
                var neighbours = grid.Neighbours(index);
                for(int i =0; i < neighbours.Count;   ++i)
                {
                    if(tiles.Contains(neighbours[i]))
                    {
                        tiles.Add(index);
                        UpdateVertexs();
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public void RemoveTile(TileIndex index)
    {
        tiles.Remove(index);
        UpdateVertexs();
    }

    private void UpdateVertexs()
    {
        Dictionary<Vector3, List<TileIndex>> vertexToTileDic = new Dictionary<Vector3, List<TileIndex>>(); 
        for(int i = 0; i < tiles.Count; ++i)
        {
            var tile = tiles[i];
            var verts = grid.TileVertexs(tile);
            for(int j = 0; j <verts.Count; ++j)
            {
                Vector3 vert = verts[j];
                if (vertexToTileDic.ContainsKey(vert)==false)
                {
                    vertexToTileDic.Add(vert, new List<TileIndex>());
                }
                if(vertexToTileDic[vert].Contains(tile) ==false)
                {
                    vertexToTileDic[vert].Add(tile);
                }
            }
        }
        mVertexs.Clear();
        var it = vertexToTileDic.GetEnumerator();
        while(it.MoveNext())
        {
            if(it.Current.Value.Count <= 2)
            {
                mVertexs.Add(it.Current.Key);
            }
        }
    }

    public List<Vector3> GetTerritoryVertexs(bool worldSpace  =false)
    {
        if(worldSpace == false)
        {
            return mVertexs;
        }
        else
        {
            List<Vector3> verts = new List<Vector3>();
            for(int i = 0; i < mVertexs.Count; ++i)
            {
                verts.Add(grid.root.TransformPoint(mVertexs[i]));
            }
            return verts;
        }
    }
}