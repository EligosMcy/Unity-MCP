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
        blueprint.blueprintName = "Simple House";
        blueprint.width = 5;
        blueprint.height = 3;
        blueprint.depth = 5;
        blueprint.description = "A simple 3x3 house with a roof";
        blueprint.blocks = new List<BlockData>();

        for (int x = 0; x < 5; x++)
        {
            for (int z = 0; z < 5; z++)
            {
                for (int y = 0; y < 3; y++)
                {
                    if (y == 0 || x == 0 || x == 4 || z == 0 || z == 4)
                    {
                        MaterialType materialType = y == 0 ? MaterialType.Stone : MaterialType.Wood;
                        Color color = materialType == MaterialType.Stone ? Color.gray : new Color(0.6f, 0.4f, 0.2f);
                        blueprint.blocks.Add(new BlockData(x, y, z, materialType, color));
                    }
                }

                if (x == 2 && z == 2)
                {
                    blueprint.blocks.Add(new BlockData(x, 3, z, MaterialType.Wood, new Color(0.6f, 0.4f, 0.2f)));
                }
            }
        }

        blueprint.CalculateMaterialRequirements();

        string path = "Assets/Resources/Blueprints/SimpleHouse.asset";
        AssetDatabase.CreateAsset(blueprint, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"已创建简单房子蓝图: {path}");
    }

    private void CreateTowerBlueprint()
    {
        BlueprintData blueprint = ScriptableObject.CreateInstance<BlueprintData>();
        blueprint.blueprintName = "Tower";
        blueprint.width = 3;
        blueprint.height = 8;
        blueprint.depth = 3;
        blueprint.description = "An 8-story tower";
        blueprint.blocks = new List<BlockData>();

        for (int y = 0; y < 8; y++)
        {
            for (int x = 0; x < 3; x++)
            {
                for (int z = 0; z < 3; z++)
                {
                    if (x == 0 || x == 2 || z == 0 || z == 2)
                    {
                        MaterialType materialType = y < 4 ? MaterialType.Stone : MaterialType.Metal;
                        Color color = materialType == MaterialType.Stone ? Color.gray : Color.blue;
                        blueprint.blocks.Add(new BlockData(x, y, z, materialType, color));
                    }
                }
            }

            if (y == 7)
            {
                blueprint.blocks.Add(new BlockData(1, 7, 1, MaterialType.Glass, Color.cyan));
            }
        }

        blueprint.CalculateMaterialRequirements();

        string path = "Assets/Resources/Blueprints/Tower.asset";
        AssetDatabase.CreateAsset(blueprint, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"已创建塔楼蓝图: {path}");
    }

    private void CreateBridgeBlueprint()
    {
        BlueprintData blueprint = ScriptableObject.CreateInstance<BlueprintData>();
        blueprint.blueprintName = "Bridge";
        blueprint.width = 7;
        blueprint.height = 2;
        blueprint.depth = 3;
        blueprint.description = "A bridge across a river";
        blueprint.blocks = new List<BlockData>();

        for (int x = 0; x < 7; x++)
        {
            for (int z = 0; z < 3; z++)
            {
                blueprint.blocks.Add(new BlockData(x, 0, z, MaterialType.Stone, Color.gray));
            }

            if (x == 0 || x == 6)
            {
                for (int z = 0; z < 3; z++)
                {
                    blueprint.blocks.Add(new BlockData(x, 1, z, MaterialType.Wood, new Color(0.6f, 0.4f, 0.2f)));
                }
            }
        }

        blueprint.CalculateMaterialRequirements();

        string path = "Assets/Resources/Blueprints/Bridge.asset";
        AssetDatabase.CreateAsset(blueprint, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"已创建桥梁蓝图: {path}");
    }

    private void CreateAllLevelRequirements()
    {
        BuildingLevelData level1 = ScriptableObject.CreateInstance<BuildingLevelData>();
        level1.level = 1;
        level1.woodworkingLevel = 1;
        level1.constructionLevel = 1;
        level1.description = "Beginner builder, can build simple structures";

        string path1 = "Assets/Resources/BuildingLevels/Level1Requirements.asset";
        AssetDatabase.CreateAsset(level1, path1);

        BuildingLevelData level2 = ScriptableObject.CreateInstance<BuildingLevelData>();
        level2.level = 2;
        level2.woodworkingLevel = 3;
        level2.constructionLevel = 3;
        level2.description = "Intermediate builder, can build more complex structures";

        string path2 = "Assets/Resources/BuildingLevels/Level2Requirements.asset";
        AssetDatabase.CreateAsset(level2, path2);

        BuildingLevelData level3 = ScriptableObject.CreateInstance<BuildingLevelData>();
        level3.level = 3;
        level3.woodworkingLevel = 5;
        level3.constructionLevel = 5;
        level3.description = "Advanced builder, can build large structures";

        string path3 = "Assets/Resources/BuildingLevels/Level3Requirements.asset";
        AssetDatabase.CreateAsset(level3, path3);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("已创建所有等级要求");
    }
}