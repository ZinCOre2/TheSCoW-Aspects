using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Pathfinding
{
    public static List<PathNode> GetNodesInPathfindingRange(Node start, int minRange, int maxRange)
    {
        Vector2Int maskCenter = new Vector2Int(maxRange, maxRange);
        PathNode centerNode;

        List<PathNode> area = new List<PathNode>();
        Vector2Int offset = start.Coords - new Vector2Int(maxRange, maxRange);
        PathNode[,] mask = new PathNode[2 * maxRange + 1, 2 * maxRange + 1];
        
        for (int i = 0; i < 2 * maxRange + 1; i++)
        {
            for (int j = 0; j < 2 * maxRange + 1; j++)
            {
                if (offset.x + i >= 0 && offset.x + i < GameController.Instance.Grid.XSize && 
                    offset.y + j >= 0 && offset.y + j < GameController.Instance.Grid.YSize)
                {
                    mask[i, j] = new PathNode(GameController.Instance.Grid.nodeList[offset.x + i, offset.y + j], int.MaxValue);
                }
            }
        }

        centerNode = mask[maskCenter.x, maskCenter.y];
        centerNode.SetLength(0);
        area.Add(centerNode);

        if (maxRange > 0)
        {
            if (GameController.Instance.Grid.NodeExists(start.Coords + Vector2Int.left))
            {
                mask[maskCenter.x - 1, maskCenter.y].SetLength(1);
                mask[maskCenter.x - 1, maskCenter.y].SetPrevious(centerNode);
                area.Add(mask[maskCenter.x - 1, maskCenter.y]);

                if (!GameController.Instance.Grid.NodeOccupied(start.Coords + Vector2Int.left) ||
                    (GameController.Instance.Grid.GetUnitOnNode(start.Coords + Vector2Int.left) != null &&
                     GameController.Instance.SceneController.SelectedUnit.TeamId == GameController.Instance.Grid.GetUnitOnNode(start.Coords + Vector2Int.left).TeamId))
                    // offset + maskCenter
                {
                    RecPathfinding(mask[maskCenter.x - 1, maskCenter.y], maxRange, area, offset, mask);
                }
            }

            if (GameController.Instance.Grid.NodeExists(start.Coords + Vector2Int.right))
            {
                mask[maskCenter.x + 1, maskCenter.y].SetLength(1);
                mask[maskCenter.x + 1, maskCenter.y].SetPrevious(centerNode);
                area.Add(mask[maskCenter.x + 1, maskCenter.y]);

                if (!GameController.Instance.Grid.NodeOccupied(start.Coords + Vector2Int.right) ||
                    (GameController.Instance.Grid.GetUnitOnNode(start.Coords + Vector2Int.right) != null &&
                     GameController.Instance.SceneController.SelectedUnit.TeamId == GameController.Instance.Grid.GetUnitOnNode(start.Coords + Vector2Int.right).TeamId))
                {
                    RecPathfinding(mask[maskCenter.x + 1, maskCenter.y], maxRange, area, offset, mask);
                }
            }

            if (GameController.Instance.Grid.NodeExists(start.Coords + Vector2Int.up))
            {
                mask[maskCenter.x, maskCenter.y + 1].SetLength(1);
                mask[maskCenter.x, maskCenter.y + 1].SetPrevious(centerNode);
                area.Add(mask[maskCenter.x, maskCenter.y + 1]);

                if (!GameController.Instance.Grid.NodeOccupied(start.Coords + Vector2Int.up) ||
                    (GameController.Instance.Grid.GetUnitOnNode(start.Coords + Vector2Int.up) != null &&
                     GameController.Instance.SceneController.SelectedUnit.TeamId == GameController.Instance.Grid.GetUnitOnNode(start.Coords + Vector2Int.up).TeamId))
                {
                    RecPathfinding(mask[maskCenter.x, maskCenter.y + 1], maxRange, area, offset, mask);
                }
            }

            if (GameController.Instance.Grid.NodeExists(start.Coords + Vector2Int.down))
            {
                mask[maskCenter.x, maskCenter.y - 1].SetLength(1);
                mask[maskCenter.x, maskCenter.y - 1].SetPrevious(centerNode);
                area.Add(mask[maskCenter.x, maskCenter.y - 1]);

                if (!GameController.Instance.Grid.NodeOccupied(start.Coords + Vector2Int.down) ||
                    (GameController.Instance.Grid.GetUnitOnNode(start.Coords + Vector2Int.down) != null &&
                     GameController.Instance.SceneController.SelectedUnit.TeamId == GameController.Instance.Grid.GetUnitOnNode(start.Coords + Vector2Int.down).TeamId))
                {
                    RecPathfinding(mask[maskCenter.x, maskCenter.y - 1], maxRange, area, offset, mask);
                }
            }
        }

        // Убираем слишком близкие ячейки (на расстоянии меньше минимального)
        List<PathNode> newArea = new List<PathNode>();
        foreach (PathNode pathNode in area)
        {
            if (pathNode.length >= minRange)
            {
                newArea.Add(pathNode);
            }
        }
        
        area.Clear();
        
        return newArea;
    }
    private static void RecPathfinding(PathNode pathNode, int maxRange, List<PathNode> area, Vector2Int offset, PathNode[,] mask)
    {
        if (pathNode.length < maxRange)
        {
            if (GameController.Instance.Grid.NodeExists(pathNode.node.Coords + Vector2Int.left) &&
                mask[pathNode.node.Coords.x - offset.x - 1, pathNode.node.Coords.y - offset.y].length > pathNode.length + 1)
            {
                mask[pathNode.node.Coords.x - offset.x - 1, pathNode.node.Coords.y - offset.y].SetLength(pathNode.length + 1);
                mask[pathNode.node.Coords.x - offset.x - 1, pathNode.node.Coords.y - offset.y].SetPrevious(pathNode);
                area.Add(mask[pathNode.node.Coords.x - offset.x - 1, pathNode.node.Coords.y - offset.y]);

                if (!GameController.Instance.Grid.NodeOccupied(pathNode.node.Coords + Vector2Int.left) ||
                    (GameController.Instance.Grid.GetUnitOnNode(pathNode.node.Coords + Vector2Int.left) != null &&
                    GameController.Instance.SceneController.SelectedUnit.TeamId == GameController.Instance.Grid.GetUnitOnNode(pathNode.node.Coords + Vector2Int.left).TeamId))
                {
                    RecPathfinding(mask[pathNode.node.Coords.x - offset.x - 1, pathNode.node.Coords.y - offset.y], maxRange, area, offset, mask);
                }
            }

            if (GameController.Instance.Grid.NodeExists(pathNode.node.Coords + Vector2Int.right) &&
                mask[pathNode.node.Coords.x - offset.x + 1, pathNode.node.Coords.y - offset.y].length > pathNode.length + 1)
            {
                mask[pathNode.node.Coords.x - offset.x + 1, pathNode.node.Coords.y - offset.y].SetLength(pathNode.length + 1);
                mask[pathNode.node.Coords.x - offset.x + 1, pathNode.node.Coords.y - offset.y].SetPrevious(pathNode);
                area.Add(mask[pathNode.node.Coords.x - offset.x + 1, pathNode.node.Coords.y - offset.y]);

                if (!GameController.Instance.Grid.NodeOccupied(pathNode.node.Coords + Vector2Int.right) ||
                    (GameController.Instance.Grid.GetUnitOnNode(pathNode.node.Coords + Vector2Int.right) != null &&
                     GameController.Instance.SceneController.SelectedUnit.TeamId == GameController.Instance.Grid.GetUnitOnNode(pathNode.node.Coords + Vector2Int.right).TeamId))
                {
                    RecPathfinding(mask[pathNode.node.Coords.x - offset.x + 1, pathNode.node.Coords.y - offset.y], maxRange, area, offset, mask);
                }
            }

            if (GameController.Instance.Grid.NodeExists(pathNode.node.Coords + Vector2Int.up) &&
                mask[pathNode.node.Coords.x - offset.x, pathNode.node.Coords.y - offset.y + 1].length > pathNode.length + 1)
            {
                mask[pathNode.node.Coords.x - offset.x, pathNode.node.Coords.y - offset.y + 1].SetLength(pathNode.length + 1);
                mask[pathNode.node.Coords.x - offset.x, pathNode.node.Coords.y - offset.y + 1].SetPrevious(pathNode);
                area.Add(mask[pathNode.node.Coords.x - offset.x, pathNode.node.Coords.y - offset.y + 1]);

                if (!GameController.Instance.Grid.NodeOccupied(pathNode.node.Coords + Vector2Int.up) ||
                    (GameController.Instance.Grid.GetUnitOnNode(pathNode.node.Coords + Vector2Int.up) != null && 
                     GameController.Instance.SceneController.SelectedUnit.TeamId == GameController.Instance.Grid.GetUnitOnNode(pathNode.node.Coords + Vector2Int.up).TeamId))
                {
                    RecPathfinding(mask[pathNode.node.Coords.x - offset.x, pathNode.node.Coords.y - offset.y + 1], maxRange, area, offset, mask);
                }
            }

            if (GameController.Instance.Grid.NodeExists(pathNode.node.Coords + Vector2Int.down) &&
                mask[pathNode.node.Coords.x - offset.x, pathNode.node.Coords.y - offset.y - 1].length > pathNode.length + 1)
            {
                mask[pathNode.node.Coords.x - offset.x, pathNode.node.Coords.y - offset.y - 1].SetLength(pathNode.length + 1);
                mask[pathNode.node.Coords.x - offset.x, pathNode.node.Coords.y - offset.y - 1].SetPrevious(pathNode);
                area.Add(mask[pathNode.node.Coords.x - offset.x, pathNode.node.Coords.y - offset.y - 1]);

                if (!GameController.Instance.Grid.NodeOccupied(pathNode.node.Coords + Vector2Int.down) ||
                    (GameController.Instance.Grid.GetUnitOnNode(pathNode.node.Coords + Vector2Int.down) != null &&
                     GameController.Instance.SceneController.SelectedUnit.TeamId == GameController.Instance.Grid.GetUnitOnNode(pathNode.node.Coords + Vector2Int.down).TeamId))
                {
                    RecPathfinding(mask[pathNode.node.Coords.x - offset.x, pathNode.node.Coords.y - offset.y - 1], maxRange, area, offset, mask);
                }
            }
        }
    }

    public static List<PathNode> GetNodesInImpulseRange(Node start, int minRange, int maxRange, bool isBlockedByAlly, bool isBlockedByEnemy, bool isBlockedByEntity)
    {
        var testNodes = GetNodesInAbsoluteRange(start, minRange, maxRange);
        
        var targetCoords = new List<Vector2Int>();
        
        for (var i = 0; i <= maxRange; i++)
        {
            targetCoords.Add(new Vector2Int(start.Coords.x + i, start.Coords.y + maxRange - i));
            targetCoords.Add(new Vector2Int(start.Coords.x + i, start.Coords.y - maxRange + i));
        }

        if (maxRange > 0)
        {
            for (var i = -1; i >= -maxRange; i--)
            {
                targetCoords.Add(new Vector2Int(start.Coords.x + i, start.Coords.y + maxRange + i));
                targetCoords.Add(new Vector2Int(start.Coords.x + i, start.Coords.y - maxRange - i));
            }
        }

        var impulseArea = new List<PathNode>();
        foreach (var target in targetCoords)
        {
            var nodes = GetLinePath(start, target, isBlockedByAlly, isBlockedByEnemy, isBlockedByEntity);

            foreach (var node in nodes)
            {
                if (!IsNodeDuplicate(impulseArea, node))
                {
                    impulseArea.Add(node);
                }
            }
        }

        foreach (var testNode in testNodes)
        {
            if (IsNodeDuplicate(impulseArea, testNode)) { continue; }
            
            var nodes = GetLinePath(start, testNode.node.Coords, isBlockedByAlly, isBlockedByEnemy, isBlockedByEntity);
        
            foreach (var node in nodes)
            {
                if (!IsNodeDuplicate(impulseArea, node))
                {
                    impulseArea.Add(node);
                }
            }
        }

        // Min range cut
        var result = new List<PathNode>();
        foreach (var impulseNode in impulseArea)
        {
            if (impulseNode.length == minRange)
            {
                impulseNode.SetPrevious(null);
            }
            
            if (impulseNode.length >= minRange)
            {
                result.Add(impulseNode);
            }
        }
        
        targetCoords.Clear();
        impulseArea.Clear();
        testNodes.Clear();

        return result;
    }

    private static bool IsNodeDuplicate(List<PathNode> area, PathNode searchedNode) =>
        area.Any(pNode => pNode.node == searchedNode.node);

    public static List<PathNode> GetLinePath(Node start, Vector2Int coords, bool isBlockedByAlly, bool isBlockedByEnemy, bool isBlockedByEntity)
    {
        int x0 = start.Coords.x, y0 = start.Coords.y;
        int x1 = coords.x, y1 = coords.y;
        var counter = 0;
        
        var path = new List<PathNode>();
        
        int dx = Math.Abs(x1 - x0), sx = x0 < x1 ? 1 : -1;
        int dy = Math.Abs(y1 - y0), sy = y0 < y1 ? 1 : -1;
        int err = (dx > dy ? dx : -dy) / 2, e2;

        for(;;)
        {
            if (!GameController.Instance.Grid.NodeExists(x0, y0))
            {
                break;
            }
            
            var newPathNode = new PathNode(GameController.Instance.Grid.nodeList[x0, y0], counter);
            if (path.Count > 0)
            {
                newPathNode.SetPrevious(path[path.Count - 1]);
            }
            path.Add(newPathNode);

            if (isBlockedByEntity && GameController.Instance.Grid.NodeOccupied(x0, y0) &&
                ((GameController.Instance.Grid.GetUnitOnNode(x0, y0) != null &&
                 GameController.Instance.Grid.GetUnitOnNode(x0, y0).TeamId == 0) ||
                (GameController.Instance.Grid.GetUnitOnNode(x0, y0) == null)))
            {
                break;
            }

            if (GameController.Instance.Grid.GetUnitOnNode(x0, y0) != null &&
                isBlockedByAlly && GameController.Instance.SceneController.SelectedUnit.TeamId ==
                GameController.Instance.Grid.GetUnitOnNode(x0, y0).TeamId)
            {
                break;
            }

            if (GameController.Instance.Grid.GetUnitOnNode(x0, y0) != null &&
                isBlockedByEnemy && GameController.Instance.SceneController.SelectedUnit.TeamId !=
                GameController.Instance.Grid.GetUnitOnNode(x0, y0).TeamId && 
                GameController.Instance.Grid.GetUnitOnNode(x0, y0).TeamId != 0)
            {
                break;
            }

            if (x0 == x1 && y0 == y1) break;
            e2 = err;
            if (e2 > -dx) 
            { 
                err -= dy; 
                x0 += sx;
                counter++;
            }
            if (e2 < dy)
            {
                err += dx; 
                y0 += sy;
                counter++;
            }
        }
        return path;
    }
    
    public static List<PathNode> GetNodesInAbsoluteRange(Node start, int minRange, int maxRange)
    {
        List<PathNode> area = new List<PathNode>();

        Vector2Int startCoords = start.Coords;

        for (int i = 0; i <= maxRange; i++)
        {
            for (int j = 0; j <= maxRange - i; j++)
            {
                //if (!SceneController.Instance.Grid.NodeOccupied(new Vector2Int(startCoords.x + i, startCoords.y + j)) && Mathf.Abs(i) + Mathf.Abs(j) >= minRange)
                if (GameController.Instance.Grid.NodeExists(new Vector2Int(startCoords.x + i, startCoords.y + j)) && Mathf.Abs(i) + Mathf.Abs(j) >= minRange)
                    area.Add(new PathNode(GameController.Instance.Grid.nodeList[startCoords.x + i, startCoords.y + j], Mathf.Abs(i) + Mathf.Abs(j)));
            }
        }
        for (int i = -1; i >= -maxRange; i--)
        {
            for (int j = 0; j <= maxRange + i; j++)
            {
                if (GameController.Instance.Grid.NodeExists(new Vector2Int(startCoords.x + i, startCoords.y + j)) && Mathf.Abs(i) + Mathf.Abs(j) >= minRange)
                    area.Add(new PathNode(GameController.Instance.Grid.nodeList[startCoords.x + i, startCoords.y + j], Mathf.Abs(i) + Mathf.Abs(j)));
            }
        }
        for (int i = -1; i >= -maxRange; i--)
        {
            for (int j = -1; j >= -maxRange - i; j--)
            {
                if (GameController.Instance.Grid.NodeExists(new Vector2Int(startCoords.x + i, startCoords.y + j)) && Mathf.Abs(i) + Mathf.Abs(j) >= minRange)
                    area.Add(new PathNode(GameController.Instance.Grid.nodeList[startCoords.x + i, startCoords.y + j], Mathf.Abs(i) + Mathf.Abs(j)));
            }
        }
        for (int i = 0; i <= maxRange; i++)
        {
            for (int j = -1; j >= -maxRange + i; j--)
            {
                if (GameController.Instance.Grid.NodeExists(new Vector2Int(startCoords.x + i, startCoords.y + j)) && Mathf.Abs(i) + Mathf.Abs(j) >= minRange)
                    area.Add(new PathNode(GameController.Instance.Grid.nodeList[startCoords.x + i, startCoords.y + j], Mathf.Abs(i) + Mathf.Abs(j)));
            }
        }

        return area;
    }

    public static List<PathNode> GetPath(PathNode target)
    {
        List<PathNode> path = new List<PathNode>();

        while (target.length > 0) 
        {
            path.Add(target);
            target = target.prev;
        }

        return path;
    }
}
