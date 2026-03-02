using System;
using System.Collections.Generic;
using UnityEngine;

public class MaterialInventory : MonoBehaviour, IMaterialInventory
{
    private Dictionary<MaterialType, int> _materials;

    public event Action<MaterialType, int> OnMaterialChanged;

    private void Awake()
    {
        InitializeMaterials();
    }

    private void InitializeMaterials()
    {
        _materials = new Dictionary<MaterialType, int>();
        
        foreach (MaterialType materialType in Enum.GetValues(typeof(MaterialType)))
        {
            _materials[materialType] = 0;
        }
    }

    public int GetMaterialCount(MaterialType materialType)
    {
        if (_materials.ContainsKey(materialType))
        {
            return _materials[materialType];
        }
        return 0;
    }

    public Dictionary<MaterialType, int> GetAllMaterials()
    {
        return new Dictionary<MaterialType, int>(_materials);
    }

    public void AddMaterial(MaterialType materialType, int amount)
    {
        if (amount <= 0) return;

        if (_materials.ContainsKey(materialType))
        {
            _materials[materialType] += amount;
            OnMaterialChanged?.Invoke(materialType, _materials[materialType]);
        }
    }

    public bool ConsumeMaterial(MaterialType materialType, int amount)
    {
        if (amount <= 0) return true;

        if (_materials.ContainsKey(materialType))
        {
            if (_materials[materialType] >= amount)
            {
                _materials[materialType] -= amount;
                OnMaterialChanged?.Invoke(materialType, _materials[materialType]);
                return true;
            }
        }
        return false;
    }

    public bool CheckMaterialSufficient(Dictionary<MaterialType, int> requirements)
    {
        foreach (var requirement in requirements)
        {
            if (!_materials.ContainsKey(requirement.Key) || _materials[requirement.Key] < requirement.Value)
            {
                return false;
            }
        }
        return true;
    }

    public bool ConsumeMaterials(Dictionary<MaterialType, int> requirements)
    {
        if (!CheckMaterialSufficient(requirements))
        {
            return false;
        }

        foreach (var requirement in requirements)
        {
            _materials[requirement.Key] -= requirement.Value;
            OnMaterialChanged?.Invoke(requirement.Key, _materials[requirement.Key]);
        }
        return true;
    }

    public void ResetMaterials()
    {
        foreach (MaterialType materialType in Enum.GetValues(typeof(MaterialType)))
        {
            _materials[materialType] = 0;
            OnMaterialChanged?.Invoke(materialType, 0);
        }
    }
}
