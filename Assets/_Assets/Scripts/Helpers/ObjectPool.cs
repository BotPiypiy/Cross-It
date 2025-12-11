using System.Collections.Generic;
using UnityEngine;

public interface IPoolable
{
    void OnSpawnFromPool();
    void OnReturnToPool();
    void SetPoolReference<T>(ObjectPool<T> pool) where T : Component, IPoolable;
}

public class ObjectPool<T> : MonoBehaviour where T : Component, IPoolable
{
    [Header("Pool Settings")]
    [SerializeField] private T _prefab;
    [SerializeField] private int _initialSize = 10;
    [SerializeField] private uint _maxSize = 50;

    private Queue<T> _inactiveObjects = new Queue<T>();
    private List<T> _activeObjects = new List<T>();
    private Transform _poolContainer;

    public int ActiveCount => _activeObjects.Count;
    public int InactiveCount => _inactiveObjects.Count;
    public int TotalCount => ActiveCount + InactiveCount;

    private void Awake()
    {
        InitializePool();
    }

    protected void InitializePool()
    {
        _poolContainer = new GameObject($"{typeof(T).Name}_Pool").transform;
        _poolContainer.SetParent(transform);
        _poolContainer.localPosition = Vector3.one * 1000;

        for (int i = 0; i < _initialSize; i++)
        {
            CreateNewObject();
        }
    }

    protected T CreateNewObject()
    {
        T newObject = Instantiate(_prefab, _poolContainer);
        newObject.gameObject.SetActive(false);
        newObject.SetPoolReference(this);
        _inactiveObjects.Enqueue(newObject);

        return newObject;
    }

    public T GetObject()
    {
        T obj;

        if (_inactiveObjects.Count > 0)
        {
            obj = _inactiveObjects.Dequeue();
        }
        else if (TotalCount < _maxSize)
        {
            obj = CreateNewObject();
        }
        else
        {
            Debug.LogWarning($"ObjectPool<{typeof(T).Name}> reached max size: {_maxSize}");
            return null;
        }

        obj.gameObject.SetActive(true);
        _activeObjects.Add(obj);

        obj.OnSpawnFromPool();

        return obj;
    }

    public T GetObject(Vector3 position, Quaternion rotation)
    {
        T obj = GetObject();
        if (obj != null)
        {
            obj.transform.position = position;
            obj.transform.rotation = rotation;
        }
        return obj;
    }

    public virtual void ReturnObject(T obj)
    {
        if (obj == null) return;

        if (!_activeObjects.Contains(obj))
        {
            Debug.LogWarning($"Trying to return object to wrong pool: {obj.name}");
            return;
        }

        obj.gameObject.SetActive(false);
        obj.transform.SetParent(_poolContainer);
        obj.transform.localPosition = Vector3.zero;

        _activeObjects.Remove(obj);
        _inactiveObjects.Enqueue(obj);

        obj.OnReturnToPool();
    }

    public virtual void ReturnAllObjects()
    {
        foreach (T obj in _activeObjects.ToArray())
        {
            ReturnObject(obj);
        }
    }

    public void Prewarm(int count)
    {
        int objectsToCreate = (int)Mathf.Min(count, _maxSize - TotalCount);

        for (int i = 0; i < objectsToCreate; i++)
        {
            CreateNewObject();
        }
    }

    private void OnDestroy()
    {
        ReturnAllObjects();
    }
}