using UnityEngine;

public class Entity : MonoBehaviour
{
    [SerializeField] protected Vector2Int coords;
    public Vector2Int Coords { get { return coords; } private set { coords = value; } }

    public void SetCoords(Vector2Int coords) { Coords = coords; }
    public void SetCoords(int x, int y) { Coords = new Vector2Int(x, y); }
}
