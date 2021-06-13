using Oasez.Extensions.Generics.Singleton;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Oasez.Extensions.Generics.ItemPool
{
    public class PoolManager : GenericSingleton<PoolManager, IPoolManager>, IPoolManager
    {
        #region Private Fields
        private List<(GameObject, Coroutine)> _coroutines = new List<(GameObject, Coroutine)>();
        #endregion


        #region Public Methods
        public void TimedPool(GameObject _gameObject, float _time)
        {
            _coroutines.Add((_gameObject, StartCoroutine(poolObject(_gameObject, _time))));
        }

        public void StopTimedPool(GameObject _gameObject)
        {
            RemoveCoroutine(_gameObject);
        }
        #endregion

        #region Private Methods
        private IEnumerator poolObject(GameObject _gameObject, float _time)
        {
            yield return new WaitForSeconds(_time);
            if (_gameObject.activeInHierarchy)
            {
                _gameObject.SetActive(false);
                RemoveCoroutine(_gameObject);
            }
        }

        private void RemoveCoroutine(GameObject _gameObject)
        {
            var _coroutine = _coroutines.Find(x => (x.Item1 == _gameObject));
            if (_coroutine.Equals(null)) return;

            _coroutines.Remove(_coroutine);
            StopCoroutine(_coroutine.Item2);
        } 
        #endregion
    }

    public interface IPoolManager
    {
        /// <summary>
        /// Pools(deactivates) the object after time (in seconds)
        /// </summary>
        /// <param name="_gameObject">GameObject to pool</param>
        /// <param name="_time">Time in seconds</param>
        void TimedPool(GameObject _gameObject, float _time);

        /// <summary>
        /// Stops a timed pool
        /// </summary>
        /// <param name="_gameObject">GameObject to stop being pooled</param>
        void StopTimedPool(GameObject _gameObject);
    }
}
