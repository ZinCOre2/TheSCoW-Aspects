using System;
using UnityEngine;


public class PhysicalEntity : Entity
{
    [SerializeField] protected Transform pivot;
    [SerializeField] protected GameObject marker;
    [SerializeField] protected MeshRenderer highlight;

    protected virtual void Start()
    {
        marker.SetActive(false);
        SetNearbyCoordsAndPosition();
    }

    private Vector2Int GetNearbyCoords(Vector3 startPoint, Vector2Int gridSize, float nodeSize)
    {
        Vector2Int newCoords = new Vector2Int(Mathf.RoundToInt((transform.position.x - startPoint.x) / nodeSize),
            Mathf.RoundToInt((transform.position.z - startPoint.z) / nodeSize));

        if (newCoords.x < 0 || newCoords.x >= gridSize.x || newCoords.y < 0 || newCoords.y >= gridSize.y) // if not in grid range
            newCoords = new Vector2Int(-1, -1);
        return newCoords;
    }

    private void SetNearbyCoordsAndPosition()
    {
        Vector2Int testCoords;

        testCoords = GetNearbyCoords(GameController.Instance.Grid.transform.position, new Vector2Int(GameController.Instance.Grid.XSize,
            GameController.Instance.Grid.YSize), GameController.Instance.Grid.NodeSize);
        if (testCoords != new Vector2Int(-1, -1))
        {
            coords = testCoords;
        }
        pivot.transform.position = GameController.Instance.Grid.transform.position +
                                   new Vector3(coords.x * GameController.Instance.Grid.NodeSize, pivot.transform.position.y, 
                                       coords.y * GameController.Instance.Grid.NodeSize);
    }
}
