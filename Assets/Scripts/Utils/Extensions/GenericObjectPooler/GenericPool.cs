using System.Collections.Generic;
using UnityEngine;

namespace Oasez.Extensions.Generics.ItemPool
{
    /// <summary>
    /// Generic item pool for pooling Objects
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public struct GenericPool<T> where T : MonoBehaviour
    {
        private readonly GameObject _prefab;
        private List<T> poolList;

        /// <summary>
        /// GenericPool Constructor
        /// </summary>
        /// <param name="prefab">Object of the item this pool will pool</param>
        public GenericPool(GameObject _prefab)
        {
            this._prefab = _prefab;
            poolList = new List<T>();
        }

        /// <summary>
        /// Gets you the pool item
        /// </summary>
        /// <returns>Item that has been pooled</returns>
        public T Get(bool _active = true)
        {
            for (int i = 0; i < poolList.Count; i++)
            {
                if (!poolList[i].gameObject.activeInHierarchy)
                {
                    poolList[i].gameObject.SetActive(_active);
                    return poolList[i];
                }
            }

            return CreateNew(_active);
        }

        /// <summary>
        /// Destroys all the pooled objects (Heavy Operation)
        /// </summary>
        public void DestroyPool()
        {
            for (int i = 0; i < poolList.Count; i++)
            {
                Object.Destroy(poolList[i].gameObject);
            }

            poolList.Clear();
        }

        private T CreateNew(bool _active)
        {
            var obj = Object.Instantiate(_prefab);
            obj.SetActive(_active);

            T _component = obj.GetComponent<T>();
            poolList.Add(_component);
            return _component;
        }

    }

    public class GameObjectPool
    {
        private readonly GameObject prefab;
        private List<GameObject> poolList;

        /// <summary>
        /// GenericPool constructor
        /// </summary>
        /// <param name="prefab">Object of the item this pool will pool</param>
        public GameObjectPool(GameObject prefab)
        {
            this.prefab = prefab;

            poolList = new List<GameObject>();
            poolList.Add(prefab);
        }

        /// <summary>
        /// Gets you the pool item
        /// </summary>
        /// <returns>Item that has been pooled in 'isActive' state</returns>
        public GameObject Get(bool isActive = false)
        {
            for (int i = 0; i < poolList.Count; i++)
            {
                if (!poolList[i].activeSelf)
                {
                    poolList[i].SetActive(isActive);
                    return poolList[i];
                }
            }

            return CreateNew();
        }

        private GameObject CreateNew()
        {
            GameObject obj = Object.Instantiate(prefab);
            obj.SetActive(false);
            poolList.Add(obj);
            return obj;
        }
    }
}
