using UnityEngine;

public class Node : Entity
{
    [SerializeField] private MeshRenderer rend;

    public void MarkCustom(Color customColor) { if (rend) rend.material.color = customColor; }
    public void Unmark() { if (rend) rend.material.color = Color.white; }
}