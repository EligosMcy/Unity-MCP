using UnityEngine;
using System.Collections.Generic;

namespace TowerDefense
{
    public class BulletPool : MonoBehaviour
    {
        public static BulletPool Instance { get; private set; }

        [SerializeField] private GameObject _bulletPrefab;
        [SerializeField] private int _poolSize = 20;

        private Queue<Bullet> _pool = new Queue<Bullet>();
        private List<Bullet> _activeBullets = new List<Bullet>();

        private void Awake()
        {
            Instance = this;

            if (_bulletPrefab == null)
            {
                _bulletPrefab = Resources.Load<GameObject>("Prefabs/Bullet");
            }

            InitializePool();
        }

        private void InitializePool()
        {
            for (int i = 0; i < _poolSize; i++)
            {
                GameObject bulletObj = Instantiate(_bulletPrefab, transform);
                bulletObj.SetActive(false);
                Bullet bullet = bulletObj.GetComponent<Bullet>();
                _pool.Enqueue(bullet);
            }
        }

        public Bullet GetBullet()
        {
            Bullet bullet;
            if (_pool.Count > 0)
            {
                bullet = _pool.Dequeue();
            }
            else
            {
                GameObject bulletObj = Instantiate(_bulletPrefab, transform);
                bullet = bulletObj.GetComponent<Bullet>();
            }

            bullet.gameObject.SetActive(true);
            _activeBullets.Add(bullet);
            return bullet;
        }

        public void ReturnBullet(Bullet bullet)
        {
            if (bullet == null) return;
            
            bullet.gameObject.SetActive(false);
            _activeBullets.Remove(bullet);
            _pool.Enqueue(bullet);
        }

        public void ReturnAllBullets()
        {
            for (int i = _activeBullets.Count - 1; i >= 0; i--)
            {
                ReturnBullet(_activeBullets[i]);
            }
        }
    }
}
