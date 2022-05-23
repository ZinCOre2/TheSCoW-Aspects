using UnityEngine;

public class Node : Entity
{
    [SerializeField] private MeshRenderer rend;
    [SerializeField] private Material basicMaterial;

    private void Awake()
    {
        if (!basicMaterial)
        {
            basicMaterial = rend.material;
        }
    }
    
    public void MarkCustom(Material customMaterial) { if (rend) rend.material = customMaterial; }
    public void MarkCustom(Color customColor) { if (rend) rend.material.color = customColor; }
    public void Unmark() { if (rend) rend.material = basicMaterial; }
}