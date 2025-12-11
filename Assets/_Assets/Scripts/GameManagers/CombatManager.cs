using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(EnemyControllerPool))]
public class CombatManager : MonoBehaviour
{
    [SerializeField] private BoxCollider _combatTrigger;
    [SerializeField] private GameObject _bottomBorder;

    [SerializeField] private EnemyControllerPool _enemyControllerPool;
    [SerializeField] private List<Transform> _enemySpawnPoints;

    [SerializeField] private GameObject _chest;
    private float _xSpawnLimit;
    private float _zSpawnLimit;

    private void Awake()
    {
        InitializeLimits();
    }

    private void InitializeLimits()
    {
        _xSpawnLimit = transform.position.x + ((int)(transform.localScale.x * 10) / 2);
        _zSpawnLimit = transform.position.z + ((transform.localScale.z * 10 - 1) / 2);
    }

    private void Start()
    {
        SubscribeToEvents();
    }

    private void SubscribeToEvents()
    {
        GameManager.Instance.OnCombatModeEnter += OnCombatModeEnter;
        GameManager.Instance.OnCombatModeExit += OnCombatModeExit;

        _enemyControllerPool.AllEnemiesDead += OnAllEnemiesDead;
    }

    private void UnsubscribeFromEvents()
    {
        GameManager.Instance.OnCombatModeEnter -= OnCombatModeEnter;
        GameManager.Instance.OnCombatModeExit -= OnCombatModeExit;

        _enemyControllerPool.AllEnemiesDead -= OnAllEnemiesDead;
    }

    private void OnCombatModeEnter()
    {
        _combatTrigger.enabled = false;
        _bottomBorder.SetActive(true);

        _chest.SetActive(false);

        foreach (var transform in _enemySpawnPoints)
        {
            Vector3 position = new Vector3(Mathf.Round(transform.position.x), transform.position.y, Mathf.Round(transform.position.z));
            EnemyController enemy = _enemyControllerPool.GetObject(position, transform.rotation);
            enemy.Initialize(true);
        }
    }

    private void OnCombatModeExit()
    {
        _combatTrigger.enabled = true;
        _bottomBorder.SetActive(false);
        _chest.SetActive(false);

        _enemyControllerPool.ReturnAllObjects();
    }

    private void OnAllEnemiesDead()
    {
        if(GameManager.Instance.State == GameManager.GameState.CombatMode)
        {
            ActivateChest();
        }
    }

    private void ActivateChest()
    {
        _chest.transform.position = GameManager.Instance.PlayerController.transform.position;
        if(_chest.transform.position.x == _xSpawnLimit)
        {
            _chest.transform.position += Vector3.left;
        }
        else
        {
            _chest.transform.position += Vector3.right;
        }

        if (_chest.transform.position.z == _zSpawnLimit)
        {
            _chest.transform.position += Vector3.back;
        }
        else
        {
            _chest.transform.position += Vector3.forward;
        }

        _chest.transform.position = new Vector3(Mathf.Round(_chest.transform.position.x), _chest.transform.position.y, Mathf.Round(_chest.transform.position.z));

        _chest.SetActive(true);
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }
}
