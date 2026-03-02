using System;
using System.Collections.Generic;
using UnityEngine;

public interface IBuildingExecutor
{
    void StartBuilding(BlueprintData blueprint, Vector2Int mapPosition);
    void ClearBuilding(Vector2Int mapPosition);
    void ClearAllBuildings();
    bool IsBuilding();
    bool CheckCanBuild(BlueprintData blueprint);
    float GetSessionProgress(Vector2Int mapPosition);
    Vector2Int? GetActiveSessionKey();
    BlueprintData GetCurrentBlueprint();

    event Action<BlueprintData, Vector2Int> OnBuildingStarted;
    event Action<BlueprintData, Vector2Int> OnBuildingCompleted;
    event Action<float, Vector2Int> OnBuildingProgress;
    event Action<string> OnBuildingError;
    event Action<BlueprintData, Vector2Int> OnBuildingPaused;
    event Action<BlueprintData, Vector2Int> OnBuildingResumed;
}
