using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadSpawner : MonoBehaviour
{
    [SerializeField] private float _maxEnemyCount;
    [SerializeField] private float _minSpawnDelay;
    [SerializeField] private float _maxSpawnDelay;
    [SerializeField] private float _stepDelay;

    [SerializeField] private EnemyControllerPool _enemyPool;

    [SerializeField] private int _obstacleCount;
    [SerializeField] private int _obstacleXOffset;
    [SerializeField] private GameObject _obstaclePrefab;

    private float _xSpawnCoordinate;
    private readonly float _ySpawnCoordinate = 0.5f;
    private float _zSpawnCoordinateMin;
    private float _zSpawnCoordinateMax;

    private Coroutine _spawnCoroutine;

    private void Awake()
    {
        if (_enemyPool == null)
        {
            _enemyPool = GetComponent<EnemyControllerPool>();
        }

        InitializeSpawner();
    }

    private void Start()
    {
        SpawnObstacles();

        StartSpawn();
    }

    private void InitializeSpawner()
    {
        _xSpawnCoordinate = transform.position.x + ((int)(transform.localScale.x * 10) / 2);
        _zSpawnCoordinateMax = transform.position.z + ((transform.localScale.z * 10 - 1) / 2);
        _zSpawnCoordinateMin = transform.position.z - ((transform.localScale.z * 10 - 1) / 2);
    }

    private void SpawnObstacles()
    {
        Vector3 position;
        HashSet<Vector3> reservedPositions = new HashSet<Vector3>();

        for (int i = 0; i < _obstacleCount; i++)
        {
            do
            {
                position = new Vector3(Mathf.Round(Random.Range(-_xSpawnCoordinate + _obstacleXOffset, _xSpawnCoordinate - _obstacleXOffset)),
                    _ySpawnCoordinate,
                    Mathf.Round(Random.Range(_zSpawnCoordinateMin, _zSpawnCoordinateMax)));
            }
            while (reservedPositions.Contains(position));

            reservedPositions.Add(position);

            Instantiate(_obstaclePrefab, position, Quaternion.identity);
        }
    }

    private void StartSpawn()
    {
        if (_spawnCoroutine != null)
        {
            StopCoroutine(_spawnCoroutine);
        }

        _spawnCoroutine = StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            float delay = Random.Range(_minSpawnDelay, _maxSpawnDelay);
            yield return new WaitForSeconds(delay);

            if (_enemyPool.ActiveCount < _maxEnemyCount)
            {

                bool moveLeft = Random.Range(0, 2) == 0;
                float xSpawnCoordinate = moveLeft ? _xSpawnCoordinate : -_xSpawnCoordinate;
                float zSpawnCoordinate = Mathf.Round(Random.Range(_zSpawnCoordinateMin, _zSpawnCoordinateMax));

                Vector3 spawnPosition = new Vector3(xSpawnCoordinate, _ySpawnCoordinate, zSpawnCoordinate);
                Vector3 moveDirection = moveLeft ? Vector3.left : Vector3.right;

                EnemyController enemy = _enemyPool.GetObject(spawnPosition, Quaternion.identity);
                enemy.Initialize(_stepDelay, moveDirection);
            }
        }
    }

    public void StopSpawning()
    {
        if (_spawnCoroutine != null)
        {
            StopCoroutine(_spawnCoroutine);
            _spawnCoroutine = null;
        }
    }

    private void OnDestroy()
    {
        StopSpawning();
    }
}