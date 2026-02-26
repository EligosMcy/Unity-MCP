using System.Collections.Generic;
using UnityEngine;

public class FoodManager : MonoBehaviour
{
    [SerializeField]
    private GameSettingScriptableObject _gameSetting;

    [SerializeField]
    private GameObject _food;

    [SerializeField]
    private GameObject _bigFood;

    private GameObject _foodPrefab;
    private GameObject _bigFoodPrefab;

    private float _boundary = 4.5f;

    private GameObject _head;

    private List<GameObject> _bodyParts;

    private int _foodCount = 0;

    private void Awake()
    {
        if (_gameSetting == null)
        {
            _gameSetting = Resources.Load<GameSettingScriptableObject>("GameSettings/DefaultGameSetting");
        }
        
        if (_gameSetting != null)
        {
            _foodPrefab = _gameSetting.foodPrefab;
            _bigFoodPrefab = _gameSetting.bigFoodPrefab;
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
        // 增加食物计数
        _foodCount++;

        // 每10个小食物后生成一个大食物
        if (_foodCount % 10 == 0)
        {
            // 切换到大食物时，关闭小食物
            if (_food != null)
            {
                _food.SetActive(false);
            }
            SpawnBigFood();
        }
        else
        {
            // 生成小食物时，关闭大食物
            if (_bigFood != null)
            {
                _bigFood.SetActive(false);
            }
            SpawnSmallFood();
        }
    }

    private void SpawnSmallFood()
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

        // 激活食物
        _food.SetActive(true);

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

    private void SpawnBigFood()
    {
        // 如果大食物不存在，创建一个新的
        if (_bigFood == null)
        {
            if (_bigFoodPrefab != null)
            {
                _bigFood = Instantiate(_bigFoodPrefab);
                _bigFood.name = "BigFood";
                _bigFood.tag = _gameSetting.bigFoodTag;
            }
            else
            {
                // 如果预制体未设置，使用默认球体
                _bigFood = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                _bigFood.name = "BigFood";
                _bigFood.tag = _gameSetting.bigFoodTag;
                _bigFood.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);

                // 为大食物设置不同的材质
                Renderer renderer = _bigFood.GetComponent<Renderer>();
                if (renderer != null)
                {
                    // 尝试加载BigFoodMaterial材质
                    Material bigFoodMaterial = Resources.Load<Material>("Materials/BigFoodMaterial");
                    if (bigFoodMaterial != null)
                    {
                        renderer.material = bigFoodMaterial;
                    }
                    else
                    {
                        // 如果没有BigFoodMaterial，使用FoodMaterial
                        Material foodMaterial = Resources.Load<Material>("Materials/FoodMaterial");
                        if (foodMaterial != null)
                        {
                            renderer.material = foodMaterial;
                        }
                    }
                }
            }
        }

        // 激活大食物
        _bigFood.SetActive(true);

        // 随机生成大食物的位置（整数坐标）
        int x = Random.Range(Mathf.FloorToInt(-_boundary) + 1, Mathf.FloorToInt(_boundary));
        int z = Random.Range(Mathf.FloorToInt(-_boundary) + 1, Mathf.FloorToInt(_boundary));

        // 确保大食物不会生成在蛇身上
        bool validPosition = false;
        while (!validPosition)
        {
            validPosition = true;

            // 检查是否与蛇头重叠
            if (Vector3.Distance(new Vector3(x, 0, z), _head.transform.localPosition) < 0.8f)
            {
                validPosition = false;
            }

            // 检查是否与蛇身重叠
            foreach (GameObject bodyPart in _bodyParts)
            {
                if (Vector3.Distance(new Vector3(x, 0, z), bodyPart.transform.localPosition) < 0.8f)
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

        // 设置大食物的位置
        _bigFood.transform.position = new Vector3(x, 0, z);
    }

    public void ClearFood()
    {
        if (_food != null)
        {
            _food.SetActive(false);
        }
    }

    public void ClearBigFood()
    {
        if (_bigFood != null)
        {
            _bigFood.SetActive(false);
        }
    }

    public GameObject GetActiveFood()
    {
        if (_food != null && _food.activeSelf)
        {
            return _food;
        }
        
        if (_bigFood != null && _bigFood.activeSelf)
        {
            return _bigFood;
        }
        
        return null;
    }

    public GameObject GetFood()
    {
        return _food;
    }

    public GameObject GetBigFood()
    {
        return _bigFood;
    }

    public int GetSmallFoodScore()
    {
        return _gameSetting != null ? _gameSetting.smallFoodScore : 1;
    }

    public int GetBigFoodScore()
    {
        return _gameSetting != null ? _gameSetting.bigFoodScore : 5;
    }

    public void UpdateBodyPartsReference(System.Collections.Generic.List<GameObject> bodyParts)
    {
        _bodyParts = bodyParts;
    }
}