using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public PlayerController PlayerController => _playerController;

    [SerializeField] private PlayerController _playerController;

    public enum GameState
    {
        ClassicMode,
        CombatMode,
        Puzzle,
    }

    public GameState State
    {
        get => _gameState;
        private set
        {
            GameState oldValue = _gameState;
            _gameState = value;
            OnGameStateChanged(oldValue);
        }
    }

    private GameState _gameState;

    public Action OnClassicModeEnter;
    public Action OnCombatModeEnter;
    public Action OnPuzzleEnter;

    public Action OnClassicModeExit;
    public Action OnCombatModeExit;
    public Action OnPuzzleExit;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        State = GameState.ClassicMode;
    }

    public void Restart()
    {
        State = GameState.ClassicMode;
    }

    public void StartCombat()
    {
        State = GameState.CombatMode;
    }

    public void StartPuzzle()
    {
        State = GameState.Puzzle;
    }

    private void OnGameStateChanged(GameState previousState)
    {
        switch (previousState)
        {
            case GameState.ClassicMode:
                {
                    OnClassicModeExit?.Invoke();
                    break;
                }
            case GameState.CombatMode:
                {
                    OnCombatModeExit?.Invoke();
                    break;
                }
            case GameState.Puzzle:
                {
                    OnPuzzleExit?.Invoke();
                    break;
                }
            default:
                {
                    Debug.LogWarning($"Unexpected value of GameState: {_gameState}");
                    break;
                }
        }

        switch (_gameState)
        {
            case GameState.ClassicMode:
                {
                    OnClassicModeEnter?.Invoke();
                    break;
                }
            case GameState.CombatMode:
                {
                    OnCombatModeEnter?.Invoke();
                    break;
                }
            case GameState.Puzzle:
                {
                    OnPuzzleEnter?.Invoke();
                    break;
                }
            default:
                {
                    Debug.LogWarning($"Unexpected value: {_gameState}");
                    break;
                }
        }
    }
}
