using System;
using UnityEngine;

public enum BuildingMode
{
    Build,
    Color
}

public class BuildingStateManager
{
    private BuildingMode _currentMode = BuildingMode.Build;
    private BlueprintData _selectedBlueprint;
    private Vector2Int _currentMapPosition = Vector2Int.zero;

    public BuildingMode CurrentMode => _currentMode;
    public BlueprintData SelectedBlueprint => _selectedBlueprint;
    public Vector2Int CurrentMapPosition => _currentMapPosition;

    public event Action<BuildingMode> OnModeChanged;
    public event Action<BlueprintData> OnBlueprintSelected;
    public event Action<Vector2Int> OnMapPositionChanged;

    public void SetMode(BuildingMode mode)
    {
        if (_currentMode != mode)
        {
            _currentMode = mode;
            OnModeChanged?.Invoke(mode);
        }
    }

    public void ToggleMode()
    {
        SetMode(_currentMode == BuildingMode.Build ? BuildingMode.Color : BuildingMode.Build);
    }

    public void SelectBlueprint(BlueprintData blueprint)
    {
        _selectedBlueprint = blueprint;
        OnBlueprintSelected?.Invoke(blueprint);
    }

    public void SetMapPosition(Vector2Int position)
    {
        if (_currentMapPosition != position)
        {
            _currentMapPosition = position;
            OnMapPositionChanged?.Invoke(position);
        }
    }
}
