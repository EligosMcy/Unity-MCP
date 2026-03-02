using System;

public interface IBuildingLevelManager
{
    int GetWoodworkingLevel();
    int GetConstructionLevel();
    void SetWoodworkingLevel(int level);
    void SetConstructionLevel(int level);
    void AddWoodworkingLevel(int amount);
    void AddConstructionLevel(int amount);
    bool CanBuildWithLevel(BuildingLevelData requiredLevel);
    string GetLevelRequirementsText(BuildingLevelData requiredLevel);

    event Action<int> OnWoodworkingLevelChanged;
    event Action<int> OnConstructionLevelChanged;
}
