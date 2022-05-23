using System.Collections.Generic;
using UnityEngine;


public class EntityManager : MonoBehaviour
{
    public List<Entity> Entities = new List<Entity>();
    public List<PhysicalEntity> PhysicalEntities = new List<PhysicalEntity>();
    public List<Unit> Units = new List<Unit>();

    private void Awake()
    {
        var entities = FindObjectsOfType<Entity>();

        foreach (var entity in entities)
        {
            AddEntity(entity);
        }
    }

    public void AddEntity(Entity entity)
    {
        Entities.Add(entity);

        if (entity is PhysicalEntity)
        {
            PhysicalEntities.Add((PhysicalEntity)entity);
        }

        if (entity is Unit)
        {
            Units.Add((Unit)entity);
        }
    }

    public void RemoveEntity(Entity entity)
    {
        Entities.Remove(entity);

        if (entity is PhysicalEntity)
        {
            PhysicalEntities.Remove((PhysicalEntity)entity);
        }

        if (entity is Unit)
        {
            Units.Remove((Unit)entity);
        }
    }
}
