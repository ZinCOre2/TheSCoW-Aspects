using UnityEngine;


public class PhysicalEntity : Entity
{
    [SerializeField] protected Transform pivot;
    [SerializeField] protected GameObject marker;
    [SerializeField] protected MeshRenderer highlight;
}
