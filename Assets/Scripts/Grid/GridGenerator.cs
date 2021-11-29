using UnityEngine;

[ExecuteInEditMode]
public class GridGenerator : MonoBehaviour
{
    [Header("Grid Properties")]
    [SerializeField] private Node nodePrefab;
    [Range(5, 100)]
    [SerializeField] private int xSize = 10, ySize = 10;
    [SerializeField] private float nodeSize;
    [SerializeField] private bool isGridGenerated = true;

    private Node[,] _nodeList;

    private void Awake()
    {
        _nodeList = new Node[xSize, ySize];
    }
    private void Update()
    {
        if (!isGridGenerated)
        {
            GenerateGrid();
            isGridGenerated = true;
        }
    }

    private void GenerateGrid()
    {
        _nodeList = new Node[xSize, ySize];
        for (int i = 0; i < xSize; i++)
        {
            for (int j = 0; j < ySize; j++)
            {
                Node node = Instantiate(nodePrefab, transform);
                node.transform.position += new Vector3(i * nodeSize, 0, j * nodeSize);
                node.SetCoords(i, j);
                node.transform.localScale *= nodeSize;
                _nodeList[i, j] = node;
            }
        }
    }
}
