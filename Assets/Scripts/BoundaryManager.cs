using UnityEngine;

public class BoundaryManager : MonoBehaviour
{
    [SerializeField]
    private GameSettingScriptableObject _gameSetting;
    
    private void Awake()
    {
        if (_gameSetting == null)
        {
            _gameSetting = Resources.Load<GameSettingScriptableObject>("GameSettings/DefaultGameSetting");
        }
        
        if (_gameSetting != null)
        {
            GenerateBoundary();
        }
    }
    
    // 生成边界
    private void GenerateBoundary()
    {
        if (_gameSetting == null)
        {
            Debug.LogError("GameSetting is not assigned");
            return;
        }
        
        float boundarySize = _gameSetting.boundary;
        GameObject boundaryPrefab = _gameSetting.boundaryPrefab;
        string boundaryTag = _gameSetting.boundaryTag;
        
        if (boundaryPrefab == null)
        {
            Debug.LogError("Boundary prefab is not assigned in GameSetting");
            return;
        }
        
        // 边界位置（向外扩展0.5，确保蛇头不会直接碰到边界）
        float extendedBoundary = boundarySize + 0.5f;
        
        // Generate front boundary (Z positive)
        for (float x = -extendedBoundary; x <= extendedBoundary; x += 1.0f)
        {
            GameObject boundary = Instantiate(boundaryPrefab, new Vector3(x, 0, extendedBoundary), Quaternion.identity, transform);
            boundary.tag = boundaryTag;
        }
        
        // Generate back boundary (Z negative)
        for (float x = -extendedBoundary; x <= extendedBoundary; x += 1.0f)
        {
            GameObject boundary = Instantiate(boundaryPrefab, new Vector3(x, 0, -extendedBoundary), Quaternion.identity, transform);
            boundary.tag = boundaryTag;
        }
        
        // Generate left boundary (X negative)
        for (float z = -extendedBoundary; z <= extendedBoundary; z += 1.0f)
        {
            GameObject boundary = Instantiate(boundaryPrefab, new Vector3(-extendedBoundary, 0, z), Quaternion.identity, transform);
            boundary.tag = boundaryTag;
        }
        
        // Generate right boundary (X positive)
        for (float z = -extendedBoundary; z <= extendedBoundary; z += 1.0f)
        {
            GameObject boundary = Instantiate(boundaryPrefab, new Vector3(extendedBoundary, 0, z), Quaternion.identity, transform);
            boundary.tag = boundaryTag;
        }
    }
}