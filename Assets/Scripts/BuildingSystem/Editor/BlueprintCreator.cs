using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class BlueprintCreator : EditorWindow
{
    [MenuItem("Tools/Building System/Create Example Blueprints")]
    public static void ShowWindow()
    {
        GetWindow<BlueprintCreator>("Blueprint Creator");
    }

    private void OnGUI()
    {
        GUILayout.Label("创建示例蓝图", EditorStyles.boldLabel);

        if (GUILayout.Button("创建简单房子蓝图"))
        {
            CreateSimpleHouseBlueprint();
        }

        if (GUILayout.Button("创建塔楼蓝图"))
        {
            CreateTowerBlueprint();
        }

        if (GUILayout.Button("创建桥梁蓝图"))
        {
            CreateBridgeBlueprint();
        }

        if (GUILayout.Button("创建所有等级要求"))
        {
            CreateAllLevelRequirements();
        }
    }

    private void CreateSimpleHouseBlueprint()
    {
        BlueprintData blueprint = ScriptableObject.CreateInstance<BlueprintData>();
        blueprint.BlueprintName = "Simple House";
        blueprint.Width = 5;
        blueprint.Height = 3;
        blueprint.Depth = 5;
        blueprint.Description = "A simple 3x3 house with a roof";
        blueprint.Blocks = new List<BlockData>();

        for (int x = 0; x < 5; x++)
        {
            for (int z = 0; z < 5; z++)
            {
                for (int y = 0; y < 3; y++)
                {
                    if (y == 0 || x == 0 || x == 4 || z == 0 || z == 4)
                    {
                        MaterialType materialType = y == 0 ? MaterialType.Stone : MaterialType.Wood;
                        Color color = materialType == MaterialType.Stone ? new Color(0.8f, 0.2f, 0.2f) : new Color(1.0f, 0.6f, 0.2f);
                        blueprint.Blocks.Add(new BlockData(x, y, z, materialType, color));
                    }
                }

                if (x == 2 && z == 2)
                {
                    blueprint.Blocks.Add(new BlockData(x, 3, z, MaterialType.Wood, new Color(1.0f, 0.8f, 0.2f)));
                }
            }
        }

        blueprint.CalculateMaterialRequirements();

        string path = "Assets/Resources/Blueprints/SimpleHouse.asset";
        if (AssetDatabase.LoadAssetAtPath<BlueprintData>(path) != null)
        {
            AssetDatabase.DeleteAsset(path);
        }
        AssetDatabase.CreateAsset(blueprint, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"已创建简单房子蓝图: {path}");
    }

    private void CreateTowerBlueprint()
    {
        BlueprintData blueprint = ScriptableObject.CreateInstance<BlueprintData>();
        blueprint.BlueprintName = "Tower";
        blueprint.Width = 3;
        blueprint.Height = 8;
        blueprint.Depth = 3;
        blueprint.Description = "An 8-story tower";
        blueprint.Blocks = new List<BlockData>();

        for (int y = 0; y < 8; y++)
        {
            for (int x = 0; x < 3; x++)
            {
                for (int z = 0; z < 3; z++)
                {
                    if (x == 0 || x == 2 || z == 0 || z == 2)
                    {
                        MaterialType materialType = y < 4 ? MaterialType.Stone : MaterialType.Metal;
                        Color color = materialType == MaterialType.Stone ? new Color(0.2f, 0.6f, 0.8f) : new Color(0.4f, 0.4f, 1.0f);
                        blueprint.Blocks.Add(new BlockData(x, y, z, materialType, color));
                    }
                }
            }

            if (y == 7)
            {
                blueprint.Blocks.Add(new BlockData(1, 7, 1, MaterialType.Glass, new Color(0.6f, 1.0f, 1.0f)));
            }
        }

        blueprint.CalculateMaterialRequirements();

        string path = "Assets/Resources/Blueprints/Tower.asset";
        if (AssetDatabase.LoadAssetAtPath<BlueprintData>(path) != null)
        {
            AssetDatabase.DeleteAsset(path);
        }
        AssetDatabase.CreateAsset(blueprint, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"已创建塔楼蓝图: {path}");
    }

    private void CreateBridgeBlueprint()
    {
        BlueprintData blueprint = ScriptableObject.CreateInstance<BlueprintData>();
        blueprint.BlueprintName = "Bridge";
        blueprint.Width = 7;
        blueprint.Height = 2;
        blueprint.Depth = 3;
        blueprint.Description = "A bridge across a river";
        blueprint.Blocks = new List<BlockData>();

        for (int x = 0; x < 7; x++)
        {
            for (int z = 0; z < 3; z++)
            {
                blueprint.Blocks.Add(new BlockData(x, 0, z, MaterialType.Stone, new Color(0.2f, 0.8f, 0.2f)));
            }

            if (x == 0 || x == 6)
            {
                for (int z = 0; z < 3; z++)
                    {
                        blueprint.Blocks.Add(new BlockData(x, 1, z, MaterialType.Wood, new Color(1.0f, 0.4f, 0.8f)));
                    }
            }
        }

        blueprint.CalculateMaterialRequirements();

        string path = "Assets/Resources/Blueprints/Bridge.asset";
        if (AssetDatabase.LoadAssetAtPath<BlueprintData>(path) != null)
        {
            AssetDatabase.DeleteAsset(path);
        }
        AssetDatabase.CreateAsset(blueprint, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"已创建桥梁蓝图: {path}");
    }

    private void CreateAllLevelRequirements()
    {
        BuildingLevelData level1 = ScriptableObject.CreateInstance<BuildingLevelData>();
        level1.Level = 1;
        level1.WoodworkingLevel = 1;
        level1.ConstructionLevel = 1;
        level1.Description = "Beginner builder, can build simple structures";

        string path1 = "Assets/Resources/BuildingLevels/Level1Requirements.asset";
        if (AssetDatabase.LoadAssetAtPath<BuildingLevelData>(path1) != null)
        {
            AssetDatabase.DeleteAsset(path1);
        }
        AssetDatabase.CreateAsset(level1, path1);

        BuildingLevelData level2 = ScriptableObject.CreateInstance<BuildingLevelData>();
        level2.Level = 2;
        level2.WoodworkingLevel = 3;
        level2.ConstructionLevel = 3;
        level2.Description = "Intermediate builder, can build more complex structures";

        string path2 = "Assets/Resources/BuildingLevels/Level2Requirements.asset";
        if (AssetDatabase.LoadAssetAtPath<BuildingLevelData>(path2) != null)
        {
            AssetDatabase.DeleteAsset(path2);
        }
        AssetDatabase.CreateAsset(level2, path2);

        BuildingLevelData level3 = ScriptableObject.CreateInstance<BuildingLevelData>();
        level3.Level = 3;
        level3.WoodworkingLevel = 5;
        level3.ConstructionLevel = 5;
        level3.Description = "Advanced builder, can build large structures";

        string path3 = "Assets/Resources/BuildingLevels/Level3Requirements.asset";
        if (AssetDatabase.LoadAssetAtPath<BuildingLevelData>(path3) != null)
        {
            AssetDatabase.DeleteAsset(path3);
        }
        AssetDatabase.CreateAsset(level3, path3);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("已创建所有等级要求");
    }
}