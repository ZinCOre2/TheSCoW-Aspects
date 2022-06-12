using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

public class EntityManager : MonoBehaviour
{
    public List<Entity> Entities = new List<Entity>();
    public List<PhysicalEntity> PhysicalEntities = new List<PhysicalEntity>();
    public List<Unit> Units = new List<Unit>();
    public List<MasterUnit> MasterUnits = new List<MasterUnit>();

    public List<int> UniqueIds = new List<int>();
    
    public int GenerateUniqueId()
    {
        var value = 0;

        do
        {
            value = Random.Range(Int32.MinValue, Int32.MaxValue);
        } 
        while (UniqueIds.Contains(value));
        
        UniqueIds.Add(value);

        return value;
    }
    
    private void Awake()
    {
        var entities = FindObjectsOfType<Entity>();

        foreach (var entity in entities)
        {
            AddEntity(entity);
        }
    }

    public void CreateEntity(Entity entity, Vector3 position)
    {
        var rock = Instantiate(entity, position, Quaternion.identity);
        AddEntity(rock);
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
        
        if (entity is MasterUnit)
        {
            MasterUnits.Add((MasterUnit)entity);
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
        
        if (entity is MasterUnit)
        {
            MasterUnits.Remove((MasterUnit)entity);
        }
        
        GameController.Instance.SceneController.ResetUsageArea();
    }

    public bool FindMasterUnitByMasterId(int masterId, out MasterUnit foundUnit)
    {
        for (var i = 0; i < MasterUnits.Count; i++)
        {
            var masterUnit = MasterUnits[i];
            
            if (masterUnit.UnitStats.MasterId != masterId) { continue; }
            
            foundUnit = masterUnit;
            return true;
        }

        foundUnit = null;
        return false;
    }
}
