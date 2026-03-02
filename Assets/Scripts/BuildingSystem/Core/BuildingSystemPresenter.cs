using System;
using System.Collections.Generic;
using UnityEngine;

public class BuildingSystemPresenter
{
    private BuildingSystemManager _systemManager;
    private BuildingStateManager _stateManager;
    private BuildingUI _ui;

    public BuildingSystemPresenter(BuildingSystemManager systemManager, BuildingUI ui)
    {
        _systemManager = systemManager;
        _stateManager = new BuildingStateManager();
        _ui = ui;
    }

    public void Initialize()
    {
        SubscribeToEvents();
        _ui.SetPresenter(this);
        _ui.SetStateManager(_stateManager);
        _ui.UpdateMaterialDisplay();
        _ui.UpdateLevelDisplay();
    }

    private void SubscribeToEvents()
    {
        if (_systemManager.MaterialInventory != null)
        {
            _systemManager.MaterialInventory.OnMaterialChanged += OnMaterialChanged;
        }

        if (_systemManager.BuildingLevelManager != null)
        {
            _systemManager.BuildingLevelManager.OnWoodworkingLevelChanged += OnWoodworkingLevelChanged;
            _systemManager.BuildingLevelManager.OnConstructionLevelChanged += OnConstructionLevelChanged;
        }

        if (_systemManager.BuildingExecutor != null)
        {
            _systemManager.BuildingExecutor.OnBuildingStarted += OnBuildingStarted;
            _systemManager.BuildingExecutor.OnBuildingCompleted += OnBuildingCompleted;
            _systemManager.BuildingExecutor.OnBuildingProgress += OnBuildingProgress;
            _systemManager.BuildingExecutor.OnBuildingError += OnBuildingError;
            _systemManager.BuildingExecutor.OnBuildingPaused += OnBuildingPaused;
            _systemManager.BuildingExecutor.OnBuildingResumed += OnBuildingResumed;
        }

        _stateManager.OnModeChanged += HandleModeChanged;
        _stateManager.OnBlueprintSelected += HandleBlueprintSelectedFromState;
        _stateManager.OnMapPositionChanged += HandleMapPositionChanged;
    }

    private void UnsubscribeFromEvents()
    {
        if (_systemManager.MaterialInventory != null)
        {
            _systemManager.MaterialInventory.OnMaterialChanged -= OnMaterialChanged;
        }

        if (_systemManager.BuildingLevelManager != null)
        {
            _systemManager.BuildingLevelManager.OnWoodworkingLevelChanged -= OnWoodworkingLevelChanged;
            _systemManager.BuildingLevelManager.OnConstructionLevelChanged -= OnConstructionLevelChanged;
        }

        if (_systemManager.BuildingExecutor != null)
        {
            _systemManager.BuildingExecutor.OnBuildingStarted -= OnBuildingStarted;
            _systemManager.BuildingExecutor.OnBuildingCompleted -= OnBuildingCompleted;
            _systemManager.BuildingExecutor.OnBuildingProgress -= OnBuildingProgress;
            _systemManager.BuildingExecutor.OnBuildingError -= OnBuildingError;
            _systemManager.BuildingExecutor.OnBuildingPaused -= OnBuildingPaused;
            _systemManager.BuildingExecutor.OnBuildingResumed -= OnBuildingResumed;
        }

        _stateManager.OnModeChanged -= HandleModeChanged;
        _stateManager.OnBlueprintSelected -= HandleBlueprintSelectedFromState;
        _stateManager.OnMapPositionChanged -= HandleMapPositionChanged;
    }

    public void OnBuildButtonClicked()
    {
        if (_stateManager.SelectedBlueprint == null)
        {
            _ui.ShowError("Please select a blueprint first");
            return;
        }

        if (_systemManager.BuildingExecutor == null)
        {
            _ui.ShowError("Building system not initialized");
            return;
        }

        if (!_systemManager.BuildingExecutor.CheckCanBuild(_stateManager.SelectedBlueprint))
        {
            return;
        }

        _systemManager.BuildingExecutor.StartBuilding(_stateManager.SelectedBlueprint, _stateManager.CurrentMapPosition);
    }

    public void OnClearButtonClicked()
    {
        _systemManager.BuildingExecutor?.ClearBuilding(_stateManager.CurrentMapPosition);
        _ui.SetProgressUI(0f);
    }

    public void OnAddMaterialClicked(MaterialType materialType)
    {
        _systemManager.MaterialInventory?.AddMaterial(materialType, 10);
    }

    public void OnAddAllMaterialsClicked()
    {
        foreach (MaterialType materialType in Enum.GetValues(typeof(MaterialType)))
        {
            _systemManager.MaterialInventory?.AddMaterial(materialType, 200);
        }
    }

    public void OnIncreaseWoodworkingClicked()
    {
        _systemManager.BuildingLevelManager?.AddWoodworkingLevel(1);
    }

    public void OnIncreaseConstructionClicked()
    {
        _systemManager.BuildingLevelManager?.AddConstructionLevel(1);
    }

    public void OnModeToggleClicked()
    {
        _stateManager.ToggleMode();
    }

    public void OnBlueprintSelected(BlueprintData blueprint)
    {
        _stateManager.SelectBlueprint(blueprint);
    }

    public void OnMapPositionChanged(int x, int y)
    {
        _stateManager.SetMapPosition(new Vector2Int(x, y));
    }

    public void OnColorApplied(Color color)
    {
        if (_stateManager.CurrentMode != BuildingMode.Color) return;

        if (_systemManager.BlockCustomizer != null && _systemManager.BlockCustomizer.HasSelectedBlock())
        {
            _systemManager.BlockCustomizer.ApplyColor(color);
        }
        else
        {
            _ui.ShowError("No block selected. Hover over a block to change its color.");
        }
    }

    private void OnMaterialChanged(MaterialType type, int count)
    {
        _ui.UpdateMaterialDisplay();
        _ui.UpdateMaterialStatus();
    }

    private void OnWoodworkingLevelChanged(int level)
    {
        _ui.UpdateLevelDisplay();
        _ui.UpdateLevelStatus();
    }

    private void OnConstructionLevelChanged(int level)
    {
        _ui.UpdateLevelDisplay();
        _ui.UpdateLevelStatus();
    }

    private void OnBuildingStarted(BlueprintData blueprint, Vector2Int position)
    {
        if (position == _stateManager.CurrentMapPosition)
        {
            _ui.SetBuildButtonInteractable(false);
        }
    }

    private void OnBuildingCompleted(BlueprintData blueprint, Vector2Int position)
    {
        if (position == _stateManager.CurrentMapPosition)
        {
            _ui.SetBuildButtonInteractable(true);
        }
    }

    private void OnBuildingProgress(float progress, Vector2Int position)
    {
        if (position == _stateManager.CurrentMapPosition)
        {
            _ui.SetProgressUI(progress);
        }
    }

    private void OnBuildingError(string error)
    {
        _ui.ShowError(error);
    }

    private void OnBuildingPaused(BlueprintData blueprint, Vector2Int position)
    {
        if (position == _stateManager.CurrentMapPosition)
        {
            _ui.SetBuildButtonInteractable(true);
        }
    }

    private void OnBuildingResumed(BlueprintData blueprint, Vector2Int position)
    {
        if (position == _stateManager.CurrentMapPosition)
        {
            _ui.SetBuildButtonInteractable(false);
        }
    }

    private void HandleModeChanged(BuildingMode mode)
    {
        _ui.UpdateModeDisplay(mode);
    }

    private void HandleBlueprintSelectedFromState(BlueprintData blueprint)
    {
        _ui.UpdateBlueprintInfo(blueprint);
        _ui.UpdateMaterialStatus();
        _ui.UpdateLevelStatus();
        _ui.UpdateProgressDisplay();
    }

    private void HandleMapPositionChanged(Vector2Int position)
    {
        _ui.UpdateProgressDisplay();
    }

    public void Cleanup()
    {
        UnsubscribeFromEvents();
    }
}
