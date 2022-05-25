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

    private void Awake()
    {
        nodeList = new Node[xSize, ySize];

        var nodes = GetComponentsInChildren<Node>();

        foreach (var node in nodes)
        {
            nodeList[node.Coords.x, node.Coords.y] = node;
        }
    }

    public bool NodeExists(Vector2Int coords)
    {
        return coords.x >= 0 && coords.x < xSize && coords.y >= 0 && coords.y < ySize && nodeList[coords.x, coords.y] != null;
    }
    public bool NodeExists(int x, int y)
    {
        return x >= 0 && x < xSize && y >= 0 && y < ySize && nodeList[x, y] != null;
    }

    public bool NodeOccupied(Vector2Int coords)
    {
        if (coords.x < 0 || coords.x >= xSize || coords.y < 0 || coords.y >= ySize || nodeList[coords.x, coords.y] == null)
            return true;
        foreach (var entity in GameController.Instance.EntityManager.PhysicalEntities)
        {
            if (entity.Coords == coords)
                return true;
        }
        return false;
    }
    public bool NodeOccupied(int x, int y)
    {
        if (x < 0 || x >= xSize || y < 0 || y >= ySize || nodeList[x, y] == null)
            return true;
        foreach (var entity in GameController.Instance.EntityManager.PhysicalEntities)
        {
            if (entity.Coords.x == x && entity.Coords.y == y)
                return true;
        }
        return false;
    }

    public Unit GetUnitOnNode(Vector2Int coords)
    {
        if (coords.x < 0 || coords.x >= xSize || coords.y < 0 || coords.y >= ySize || nodeList[coords.x, coords.y] == null)
            return null;
        foreach (var unit in GameController.Instance.EntityManager.Units)
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
        foreach (var unit in GameController.Instance.EntityManager.Units)
        {
            if (unit.Coords.x == x && unit.Coords.y == y)
                return unit;
        }
        return null;
    }
}
