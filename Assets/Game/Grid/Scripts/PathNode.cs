
public class PathNode
{
    public Node node { get; private set; }
    public int length { get; private set; }
    public PathNode prev { get; private set; }

    public PathNode(Node node, int pathLength)
    {
        this.node = node;
        this.length = pathLength;
    }

    public PathNode(Node node, int pathLength, PathNode prevNode)
    {
        this.node = node;
        this.length = pathLength;
        this.prev = prevNode;
    }

    public void SetLength(int length) { this.length = length; }
    public void SetPrevious(PathNode prev) { this.prev = prev; }
}
