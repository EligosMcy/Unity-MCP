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
        if (requiredLevel == null) return "无等级要求";

        string text = "等级要求:\n";
        text += $"木工等级: {requiredLevel.woodworkingLevel} (当前: {_woodworkingLevel})";
        if (_woodworkingLevel < requiredLevel.woodworkingLevel)
        {
            text += " [不足]";
        }
        text += "\n";
        text += $"建筑等级: {requiredLevel.constructionLevel} (当前: {_constructionLevel})";
        if (_constructionLevel < requiredLevel.constructionLevel)
        {
            text += " [不足]";
        }

        return text;
    }
}