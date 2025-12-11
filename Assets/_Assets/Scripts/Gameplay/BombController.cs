using System;
using System.Collections;
using UnityEngine;

public class BombController : MonoBehaviour, IPoolable
{
    [SerializeField] private float _liveTime;
    private Coroutine _liveCoroutine;

    private ObjectPool<BombController> _parentPool;

    public void OnSpawnFromPool()
    {
        _liveCoroutine = StartCoroutine(Live());
    }

    private IEnumerator Live()
    {
        yield return new WaitForSeconds(_liveTime);
        _parentPool.ReturnObject(this);
    }

    public void OnReturnToPool()
    {
        if(_liveCoroutine != null)
        {
            StopCoroutine(_liveCoroutine);
        }
    }

    public void SetPoolReference<T>(ObjectPool<T> pool) where T : Component, IPoolable
    {
        _parentPool = pool as ObjectPool<BombController>;
    }

}
