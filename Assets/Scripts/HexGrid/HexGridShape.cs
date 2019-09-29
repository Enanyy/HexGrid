using UnityEngine;
using System;
using System.Collections.Generic;

public abstract class HexGridShape
{
    public HexGrid grid { get; private set; }
    public TileIndex center { get; protected set; }

    public List<TileIndex> tiles { get; protected set; } = new List<TileIndex>();
    protected List<TileIndex> mTildes = new List<TileIndex>();

    public HexGridShape (HexGrid grid)
    {
        this.grid = grid;
    }

}
public class HexGridShapeHex : HexGridShape
{
    public int range { get; private set; }
    public HexGridShapeHex(HexGrid grid):base(grid)
    {

    }

    public void UpdateTiles(Vector3 center, int range, Action<TileIndex> onCreate = null, Action<TileIndex> onRemove = null)
    {
        TileIndex index = grid.IndexOf(center);
        if (this.center != index || this.range != range)
        {
            this.center = index;
            this.range = range;
            mTildes.Clear();

            List<TileIndex> neighbours = grid.TilesInRange(center, range);

            for (int i = 0; i < neighbours.Count; ++i)
            {
                index = neighbours[i];

                if (tiles.Contains(index))
                {
                    tiles.Remove(index);
                }
                else
                {
                    if (onCreate != null)
                    {
                        onCreate(index);
                    }
                }
                
                mTildes.Add(index);
            }

            if (onRemove != null)
            {
                var it = tiles.GetEnumerator();
                while (it.MoveNext())
                {
                    onRemove(it.Current);
                }
            }

            var tmp = tiles;
            tiles = mTildes;
            mTildes = tmp;
        }
    }
}

public class HexGridShapeRect : HexGridShape
{
    public int width { get; private set; }
    public int height { get; private set; }

    public HexGridShapeRect(HexGrid grid) : base(grid)
    {

    }

    public void UpdateTiles(Vector3 center, int width, int height, Action<TileIndex> onCreate = null, Action<TileIndex> onRemove = null)
    {
        TileIndex index = grid.IndexOf(center);
        if (this.center != index || this.width != width || this.height != height)
        {
            this.center = index;
            this.width = width;
            this.height = height;

            mTildes.Clear();


            switch (grid.orientation)
            {
                case HexOrientation.Flat:
                    {
                        int x = width / 2;
                        int y = height / 2;
                        for (int q = -x; q < x; q++)
                        {
                            int qOff = q >> 1;
                            for (int r = -qOff - y; r < y - qOff; r++)
                            {
                                index = new TileIndex(q, r, -q - r) + this.center;

                                if (tiles.Contains(index))
                                {
                                    tiles.Remove(index);
                                }
                                else
                                {
                                    if (onCreate != null)
                                    {
                                        onCreate(index);
                                    }
                                }
                                mTildes.Add(index);
                            }
                        }

                    }
                    break;
                case HexOrientation.Pointy:
                    {
                        int x = width / 2;
                        int y = height / 2;
                        for (int r = -y; r < y; r++)
                        {
                            int rOff = r >> 1;
                            for (int q = -rOff - x; q < x - rOff; q++)
                            {
                                index = new TileIndex(q, r, -q - r) + this.center;

                                if (tiles.Contains(index))
                                {
                                    tiles.Remove(index);
                                }
                                else
                                {
                                    if (onCreate != null)
                                    {
                                        onCreate(index);
                                    }
                                }
                                mTildes.Add(index);
                            }
                        }
                    }
                    break;
            }
            if (onRemove != null)
            {
                var it = tiles.GetEnumerator();
                while (it.MoveNext())
                {
                    onRemove(it.Current);
                }
            }
            var tmp = tiles;
            tiles = mTildes;
            mTildes = tmp;
        }
    }
}
