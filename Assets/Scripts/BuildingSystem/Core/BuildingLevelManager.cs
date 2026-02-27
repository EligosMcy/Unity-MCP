using System;
using System.Collections.Generic;
using UnityEngine;

public class BuildingLevelManager : MonoBehaviour
{
    public static BuildingLevelManager Instance { get; private set; }

    private int _woodworkingLevel = 1;
    private int _constructionLevel = 1;

    public event Action<int> OnWoodworkingLevelChanged;
    public event Action<int> OnConstructionLevelChanged;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public int GetWoodworkingLevel()
    {
        return _woodworkingLevel;
    }

    public int GetConstructionLevel()
    {
        return _constructionLevel;
    }

    public void SetWoodworkingLevel(int level)
    {
        if (level < 1) level = 1;
        if (level != _woodworkingLevel)
        {
            _woodworkingLevel = level;
            OnWoodworkingLevelChanged?.Invoke(_woodworkingLevel);
        }
    }

    public void SetConstructionLevel(int level)
    {
        if (level < 1) level = 1;
        if (level != _constructionLevel)
        {
            _constructionLevel = level;
            OnConstructionLevelChanged?.Invoke(_constructionLevel);
        }
    }

    public void AddWoodworkingLevel(int amount)
    {
        SetWoodworkingLevel(_woodworkingLevel + amount);
    }

    public void AddConstructionLevel(int amount)
    {
        SetConstructionLevel(_constructionLevel + amount);
    }

    public bool CanBuildWithLevel(BuildingLevelData requiredLevel)
    {
        if (requiredLevel == null) return true;
        
        return _woodworkingLevel >= requiredLevel.woodworkingLevel && 
               _constructionLevel >= requiredLevel.constructionLevel;
    }

    public string GetLevelRequirementsText(BuildingLevelData requiredLevel)
    {
        if (requiredLevel == null) return "No Level Requirements";

        string text = "Level Requirements:\n";
        text += $"Woodworking Level: {requiredLevel.woodworkingLevel} (Current: {_woodworkingLevel})";
        if (_woodworkingLevel < requiredLevel.woodworkingLevel)
        {
            text += " [Insufficient]";
        }
        text += "\n";
        text += $"Construction Level: {requiredLevel.constructionLevel} (Current: {_constructionLevel})";
        if (_constructionLevel < requiredLevel.constructionLevel)
        {
            text += " [Insufficient]";
        }

        return text;
    }
}