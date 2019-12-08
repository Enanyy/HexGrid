using UnityEngine;
using System.Collections.Generic;
using System;

public enum HexDirection
{
    NE, E, SE, SW, W, NW
}


[System.Serializable]
public enum HexOrientation
{
    Pointy,
    Flat
}


[System.Serializable]
public struct TileIndex :IEquatable<TileIndex>
{
    public int x;
    public int y;
    public int z;

    public TileIndex(int x, int y, int z)
    {
        this.x = x; this.y = y; this.z = z;
    }

    public static bool operator ==(TileIndex a, TileIndex b)
    {
        return a.Equals(b);
    }
    public static bool operator !=(TileIndex a, TileIndex b)
    {
        return !a.Equals(b);
    }
    public static TileIndex operator +(TileIndex a, TileIndex b)
    {
        return new TileIndex(a.x + b.x, a.y + b.y, a.z + b.z);
    }
    public static TileIndex operator -(TileIndex a, TileIndex b)
    {
        return new TileIndex(a.x - b.x, a.y - b.y, a.z - b.z);
    }
    public static TileIndex operator *(TileIndex a, int b)
    {
        return new TileIndex(a.x *b, a.y * b, a.z * b);
    }

    public override bool Equals(object obj)
    {
        if (obj == null)
            return false;
        TileIndex o = (TileIndex)obj;
        if ((object)o == null)
            return false;
        return Equals(o);
    }

    public bool Equals(TileIndex o)
    {
        return ((x == o.x) && (y == o.y) && (z == o.z));
    }

    public override int GetHashCode()
    {
        return (x.GetHashCode() ^ (y.GetHashCode() + (int)(Mathf.Pow(2, 32) / (1 + Mathf.Sqrt(5)) / 2) + (x.GetHashCode() << 6) + (x.GetHashCode() >> 2)));
    }

    public override string ToString()
    {
        return string.Format("[{0},{1},{2}]", x, y, z);
    }

    public static TileIndex zero = new TileIndex(0, 0, 0);
}


public class HexGrid 
{
    public static readonly float SQRT3 = Mathf.Sqrt(3f);
    //Hex Settings
    public HexOrientation orientation { get; private set; }
    public float radius { get; private set; }

    public Transform root { get; protected set; }

    private static readonly TileIndex[] directions =
        new TileIndex[]
        {
            new TileIndex(1, -1, 0),
            new TileIndex(1, 0, -1),
            new TileIndex(0, 1, -1),
            new TileIndex(-1, 1, 0),
            new TileIndex(-1, 0, 1),
            new TileIndex(0, -1, 1)
        };

    #region Public Methods

    public HexGrid(float radius,HexOrientation orientation)
    {
        this.radius = radius;
        this.orientation = orientation;
        //Generating a new grid, clear any remants and initialise values
    }
    public void SetRoot(Transform root)
    {
        this.root = root;
    }

    public TileIndex IndexOf(int x, int y, int z)
    {
        return new TileIndex(x,y,z);
    }

    public TileIndex IndexOf(int x, int z)
    {
        return new TileIndex(x, -x - z, z);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public TileIndex IndexOf(Vector3 worldPosition)
    {
        Vector3 localPosition = root.InverseTransformPoint(worldPosition);
        float x = localPosition.x;
        float z = localPosition.z;

        if (orientation == HexOrientation.Flat)
        {
            int q = (int)Math.Round(x / radius / 1.5f, MidpointRounding.AwayFromZero);
            int r = (int)Math.Round(z / radius / SQRT3 - q * 0.5f, MidpointRounding.AwayFromZero);

            return new TileIndex(q, r, -q - r);
        }
        else
        {
            int r = (int)Math.Round(z / radius / 1.5f, MidpointRounding.AwayFromZero);
            int q = (int)Math.Round(x / radius / SQRT3 - r * 0.5f, MidpointRounding.AwayFromZero);

            return new TileIndex(q, r, -q - r);
        }
    }
  
    public TileIndex DirectionTo(Vector3 worldPosition, HexDirection direction, int distance)
    {
        return DirectionTo(IndexOf(worldPosition), direction, distance);
    }
 
  
    public TileIndex DirectionTo(TileIndex index, HexDirection direction, int distance)
    {
        int i = (int) direction;
        TileIndex o = index + directions[i] * distance;
        return o;
    }

    public List<TileIndex> Neighbours(TileIndex index)
    {
        List<TileIndex> ret = new List<TileIndex>();
        TileIndex o;

        for (int i = 0; i < 6; i++)
        {
            o = index + directions[i];
            ret.Add(o);
        }

        return ret;
    }

    public List<TileIndex> Neighbours(int x, int y, int z)
    {
        return Neighbours(IndexOf(x, y, z));
    }

    public List<TileIndex> Neighbours(int x, int z)
    {
        return Neighbours(IndexOf(x, z));
    }

 

    public List<TileIndex> Neighbours(Vector3 worldPosition)
    {
        return Neighbours(IndexOf(worldPosition));
    }

    public List<TileIndex> TilesInRange(TileIndex index, int range)
    {
        //Return tiles rnage steps from center, http://www.redblobgames.com/grids/hexagons/#range
        List<TileIndex> ret = new List<TileIndex>();
        TileIndex o;

        for (int dx = -range; dx <= range; dx++)
        {
            int i = Mathf.Max(-range, -dx - range);
            int j = Mathf.Min(range, -dx + range);
            for (int dy = i; dy <= j; dy++)
            {
                o = new TileIndex(dx, dy, -dx - dy) + index;

                ret.Add(o);
            }
        }

        return ret;
    }
    public List<TileIndex> TilesInRange(Vector3 worldPosition,int range)
    {
        return TilesInRange(IndexOf(worldPosition), range);
    }
    public List<TileIndex> TilesInRange(int x, int y, int z, int range)
    {
        return TilesInRange(IndexOf(x, y, z), range);
    }

    public List<TileIndex> TilesInRange(int x, int z, int range)
    {
        return TilesInRange(IndexOf(x, z), range);
    }

    public List<TileIndex> TilesInDistance(TileIndex index, int range)
    {
        List<TileIndex> ret = new List<TileIndex>();
        TileIndex o;

        for (int dx = -range; dx <= range; dx++)
        {
            int i = Mathf.Max(-range, -dx - range);
            int j = Mathf.Min(range, -dx + range);
            for (int dy = i; dy <= j;)
            {
                o = new TileIndex(dx, dy, -dx - dy) + index;
              
                ret.Add(o);

                dy = (dx == -range || dx == range) ? dy + 1 : dy + (j - i);
            }
        }

        return ret;
    }



    public List<TileIndex> TilesInDistance(int x, int y, int z, int range)
    {
        return TilesInDistance(IndexOf(x, y, z), range);
    }

    public List<TileIndex> TilesInDistance(int x, int z, int range)
    {
        return TilesInDistance(IndexOf(x, z), range);
    }

    public int Distance(TileIndex a, TileIndex b)
    {
        return (Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y) + Mathf.Abs(a.z - b.z)) / 2;
    }

    public int Distance(Vector3 from, Vector3 to)
    {
        return Distance(IndexOf(from), IndexOf(to));
    }

    public Vector3 TilePosition(TileIndex index, bool worldSpace = false)
    {
        Vector3 pos = Vector3.zero;

        int q = index.x;
        int r = index.y;
        if (orientation == HexOrientation.Flat)
        {
            pos.x = radius * 3.0f / 2.0f * q;
            pos.z = radius * SQRT3 * (r + q / 2.0f);
        }
        else
        {
            pos.x = radius * SQRT3 * (q + r / 2.0f);
            pos.z = radius * 3.0f / 2.0f * r;
        }
        if (worldSpace) pos = root.TransformPoint(pos);

        return pos;
    }
    public List<Vector3> TileVertexs(TileIndex index, bool worldSpace = true)
    {
        List<Vector3> vertexs = new List<Vector3>();
        Vector3 center = TilePosition(index);
        for (int i = 0; i < 6; i++)
        {
            Vector3 pos = Corner(center, radius, i, orientation);
            if (worldSpace) pos = root.TransformPoint(pos);
            vertexs.Add(pos);
        }
        return vertexs;
    }
    #endregion

    #region Private Methods



    #endregion

    #region  Static Methods

    public static Vector3 Corner(Vector3 origin, float radius, int corner, HexOrientation orientation)
    {
        float angle = 60 * corner;
        if (orientation == HexOrientation.Pointy)
            angle += 30;
        angle *= Mathf.PI / 180;
        return new Vector3(origin.x + radius * Mathf.Cos(angle), 0.0f, origin.z + radius * Mathf.Sin(angle));
    }

    public static Mesh GenerateHexMesh(float radius, HexOrientation orientation)
    {
        Mesh mesh = new Mesh();

        List<Vector3> verts = new List<Vector3>();
        List<int> tris = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        for (int i = 0; i < 6; i++)
            verts.Add(Corner(Vector3.zero, radius, i, orientation));

        tris.Add(0);
        tris.Add(2);
        tris.Add(1);

        tris.Add(0);
        tris.Add(5);
        tris.Add(2);

        tris.Add(2);
        tris.Add(5);
        tris.Add(3);

        tris.Add(3);
        tris.Add(5);
        tris.Add(4);

        //UVs are wrong, I need to find an equation for calucalting them
        uvs.Add(new Vector2(0.5f, 1f));
        uvs.Add(new Vector2(1, 0.75f));
        uvs.Add(new Vector2(1, 0.25f));
        uvs.Add(new Vector2(0.5f, 0));
        uvs.Add(new Vector2(0, 0.25f));
        uvs.Add(new Vector2(0, 0.75f));

        mesh.vertices = verts.ToArray();
        mesh.triangles = tris.ToArray();
        mesh.uv = uvs.ToArray();

        mesh.name = "Hexagonal Plane";

        mesh.RecalculateNormals();

        return mesh;
    }

    public static int GetCostValue(TileIndex a, TileIndex b)
    {
        int x = Mathf.Abs(a.x - b.x);
        int y = Mathf.Abs(a.y - b.y);
        int z = Mathf.Abs(a.z - b.z);

        int min = Mathf.Min(Mathf.Min(x, y), z);
        int max = Mathf.Max(Mathf.Max(x, y), z);

        // 判断到底是那个轴相差的距离更远
        return 14 * min + 10 * (max - min);
       
    }
    #endregion

    private PathFinder<TileIndex> mPathFinder = new PathFinder<TileIndex>();
    public bool FindPath(ref List<TileIndex> result, TileIndex from, TileIndex to, Func<TileIndex, bool> isValid)
    {
        if(from == to || isValid == null)
        {
            Debug.LogError("参数错误");
            return false;
        }

        return mPathFinder.FindPath(ref result, from, to, isValid,(tile)=>
        {
            var neighbours = Neighbours(tile); return neighbours.GetEnumerator();
        }, GetCostValue);
    }
}

