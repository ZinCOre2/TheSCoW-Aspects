using System.Collections.Generic;
using UnityEngine;

public class Grid2d : MonoBehaviour
{
    [Header("Grid Properties")]
    [SerializeField] private float nodeSize;
    [Range(5, 100)]
    [SerializeField] private int xSize = 10, ySize = 10;

    public int XSize { get { return xSize; } private set { xSize = value; } }
    public int YSize { get { return ySize; } private set { ySize = value; } }
    public float NodeSize { get { return nodeSize; } private set { nodeSize = value; } }

    public Node[,] nodeList { get; private set; }
    public List<Unit> unitList { get; private set; }

    private void Awake()
    {
        unitList = new List<Unit>();
        nodeList = new Node[xSize, ySize];
    }

    public bool NodeExists(Vector2Int coords)
    {
        if (coords.x < 0 || coords.x >= xSize || coords.y < 0 || coords.y >= ySize || nodeList[coords.x, coords.y] == null)
            return false;
        return true;
    }
    public bool NodeExists(int x, int y)
    {
        if (x < 0 || x >= xSize || y < 0 || y >= ySize || nodeList[x, y] == null)
            return false;
        return true;
    }

    public bool NodeOccupied(Vector2Int coords)
    {
        if (coords.x < 0 || coords.x >= xSize || coords.y < 0 || coords.y >= ySize || nodeList[coords.x, coords.y] == null)
            return true;
        foreach (Unit unit in unitList)
        {
            if (unit.Coords == coords)
                return true;
        }
        return false;
    }
    public bool NodeOccupied(int x, int y)
    {
        if (x < 0 || x >= xSize || y < 0 || y >= ySize || nodeList[x, y] == null)
            return true;
        foreach (Unit unit in unitList)
        {
            if (unit.Coords.x == x && unit.Coords.y == y)
                return true;
        }
        return false;
    }

    public Unit GetUnitOnNode(Vector2Int coords)
    {
        if (coords.x < 0 || coords.x >= xSize || coords.y < 0 || coords.y >= ySize || nodeList[coords.x, coords.y] == null)
            return null;
        foreach (Unit unit in unitList)
        {
            if (unit.Coords == coords)
                return unit;
        }
        return null;
    }
    public Unit GetUnitOnNode(int x, int y)
    {
        if (x < 0 || x >= xSize || y < 0 || y >= ySize || nodeList[x, y] == null)
            return null;
        foreach (Unit unit in unitList)
        {
            if (unit.Coords.x == x && unit.Coords.y == y)
                return unit;
        }
        return null;
    }
}
