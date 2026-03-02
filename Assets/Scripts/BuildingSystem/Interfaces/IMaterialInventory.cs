using System;
using System.Collections.Generic;

public interface IMaterialInventory
{
    int GetMaterialCount(MaterialType materialType);
    Dictionary<MaterialType, int> GetAllMaterials();
    void AddMaterial(MaterialType materialType, int amount);
    bool ConsumeMaterial(MaterialType materialType, int amount);
    bool CheckMaterialSufficient(Dictionary<MaterialType, int> requirements);
    bool ConsumeMaterials(Dictionary<MaterialType, int> requirements);
    void ResetMaterials();

    event Action<MaterialType, int> OnMaterialChanged;
}
