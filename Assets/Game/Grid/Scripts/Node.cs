using UnityEngine;

public class Node : Entity
{
    [SerializeField] private MeshRenderer rend;
    [SerializeField] private Material basicMaterial;
    [SerializeField] private Material occupiedMaterial;
    [SerializeField] private Material selectedMaterial;

    private void Awake()
    {
        SceneController.Instance.Grid.nodeList[coords.x, coords.y] = this;
        if (!basicMaterial) basicMaterial = rend.material;
        if (!selectedMaterial)
        {
            selectedMaterial = new Material(basicMaterial);
            selectedMaterial.color *= 2;
        }
    }

    public void Mark() { if (rend) rend.material = selectedMaterial; }
    public void MarkOccupied() { if (rend) rend.material = occupiedMaterial; }
    public void MarkCustom(Material customMaterial) { if (rend) rend.material = customMaterial; }
    public void MarkCustom(Color customColor) { if (rend) rend.material.color = customColor; }
    public void Unmark() { if (rend) rend.material = basicMaterial; }
}
