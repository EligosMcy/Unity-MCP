using System.Collections.Generic;
using UnityEngine;

public class FoodManager : MonoBehaviour
{
    [SerializeField]
    private GameSettingScriptableObject _gameSetting;

    [SerializeField]
    private GameObject _food;

    private GameObject _foodPrefab;

    private float _boundary = 4.5f;

    private GameObject _head;

    private List<GameObject> _bodyParts;

    private void Awake()
    {
        if (_gameSetting == null)
        {
            _gameSetting = Resources.Load<GameSettingScriptableObject>("GameSettings/DefaultGameSetting");
        }
        
        if (_gameSetting != null)
        {
            _foodPrefab = _gameSetting.foodPrefab;
            _boundary = _gameSetting.boundary;
        }
    }

    public void Initialize(GameObject head, List<GameObject> bodyParts)
    {
        _head = head;
        _bodyParts = bodyParts;
    }

    public void SpawnFood()
    {
        // 如果食物不存在，创建一个新的
        if (_food == null)
        {
            if (_foodPrefab != null)
            {
                _food = Instantiate(_foodPrefab);
                _food.name = "Food";
                _food.tag = _gameSetting.foodTag;
            }
            else
            {
                // 如果预制体未设置，使用默认球体
                _food = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                _food.name = "Food";
                _food.tag = _gameSetting.foodTag;
                _food.transform.localScale = new Vector3(1, 1, 1);

                // 为食物设置FoodMaterial材质
                Renderer renderer = _food.GetComponent<Renderer>();
                if (renderer != null)
                {
                    Material foodMaterial = Resources.Load<Material>("Materials/FoodMaterial");
                    if (foodMaterial != null)
                    {
                        renderer.material = foodMaterial;
                    }
                }
            }
        }

        // 随机生成食物的位置（整数坐标）
        int x = Random.Range(Mathf.FloorToInt(-_boundary) + 1, Mathf.FloorToInt(_boundary));
        int z = Random.Range(Mathf.FloorToInt(-_boundary) + 1, Mathf.FloorToInt(_boundary));

        // 确保食物不会生成在蛇身上
        bool validPosition = false;
        while (!validPosition)
        {
            validPosition = true;

            // 检查是否与蛇头重叠
            if (Vector3.Distance(new Vector3(x, 0, z), _head.transform.localPosition) < 0.5f)
            {
                validPosition = false;
            }

            // 检查是否与蛇身重叠
            foreach (GameObject bodyPart in _bodyParts)
            {
                if (Vector3.Distance(new Vector3(x, 0, z), bodyPart.transform.localPosition) < 0.5f)
                {
                    validPosition = false;
                    break;
                }
            }

            // 如果位置无效，重新生成
            if (!validPosition)
            {
                x = Random.Range(Mathf.FloorToInt(-_boundary) + 1, Mathf.FloorToInt(_boundary));
                z = Random.Range(Mathf.FloorToInt(-_boundary) + 1, Mathf.FloorToInt(_boundary));
            }
        }

        // 设置食物的位置
        _food.transform.position = new Vector3(x, 0, z);
    }

    public void ClearFood()
    {
        if (_food != null)
        {
            Destroy(_food);
            _food = null;
        }
    }

    public GameObject GetFood()
    {
        return _food;
    }

    public void UpdateBodyPartsReference(System.Collections.Generic.List<GameObject> bodyParts)
    {
        _bodyParts = bodyParts;
    }
}