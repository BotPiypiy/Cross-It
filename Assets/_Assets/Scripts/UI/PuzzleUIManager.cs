using System.Collections.Generic;
using TMPro;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleUIManager : MonoBehaviour
{
    [SerializeField] private GameObject _puzzlePanel;
    [SerializeField] private RawImage _lockImage;
    [SerializeField] private TMP_Text _keyCounterText;
    [SerializeField] private GridLayoutGroup _keysGrid;
    [SerializeField] private GameObject _victoryPanel;
    [SerializeField] private Button _restartButton;

    [SerializeField] private List<Color> _availableColors;
    [SerializeField] private GameObject _keyPrefab;

    [SerializeField] private int _gridSize = 6;
    [SerializeField] private int _requiredKeys = 3;

    private Color _lockColor;
    private int _correctKeysCollected = 0;
    private List<KeyUIItem> _allKeys = new List<KeyUIItem>();

    private void Start()
    {
        _puzzlePanel.SetActive(false);
        _victoryPanel.SetActive(false);

        SubscribeToEvents();
    }

    private void SubscribeToEvents()
    {
        GameManager.Instance.OnPuzzleEnter += StartPuzzle;
        GameManager.Instance.OnPuzzleExit += HideAll;

        _restartButton.onClick.AddListener(GameManager.Instance.Restart);
    }

    private void UnsubscribeFromEvents()
    {
        GameManager.Instance.OnPuzzleEnter -= StartPuzzle;
        GameManager.Instance.OnPuzzleExit -= HideAll;

        _restartButton.onClick.RemoveListener(GameManager.Instance.Restart);
    }

    public void StartPuzzle()
    {
        _puzzlePanel.SetActive(true);
        _victoryPanel.SetActive(false);

        _correctKeysCollected = 0;

        _lockColor = _availableColors[Random.Range(0, _availableColors.Count)];
        _lockImage.color = _lockColor;

        UpdateKeyCounter();
        GenerateKeysGrid();
    }

    private void GenerateKeysGrid()
    {
        foreach (Transform child in _keysGrid.transform)
        {
            Destroy(child.gameObject);
        }
        _allKeys.Clear();

        List<Color> colorsToPlace = new List<Color>();

        for (int i = 0; i < _requiredKeys; i++)
        {
            colorsToPlace.Add(_lockColor);
        }
        int totalKeys = _gridSize * _gridSize;
        for (int i = _requiredKeys; i < totalKeys; i++)
        {
            Color randomColor;
            do
            {
                randomColor = _availableColors[Random.Range(0, _availableColors.Count)];
            } while (randomColor == _lockColor);

            colorsToPlace.Add(randomColor);
        }

        ShuffleList(colorsToPlace);

        for (int i = 0; i < totalKeys; i++)
        {
            GameObject keyObj = Instantiate(_keyPrefab, _keysGrid.transform);
            KeyUIItem keyItem = keyObj.GetComponent<KeyUIItem>();
            keyItem.Initialize(colorsToPlace[i], this);
            _allKeys.Add(keyItem);
        }
    }

    private void ShuffleList<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            T temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    public void OnKeyDroppedOnLock(Color keyColor)
    {
        if (keyColor == _lockColor)
        {
            _correctKeysCollected++;
            UpdateKeyCounter();

            if (_correctKeysCollected >= _requiredKeys)
            {
                ShowVictoryScreen();
            }
        }
    }

    private void UpdateKeyCounter()
    {
        _keyCounterText.text = $"{_correctKeysCollected} / {_requiredKeys}";
    }

    private void ShowVictoryScreen()
    {
        _puzzlePanel.SetActive(false);
        _victoryPanel.SetActive(true);
    }

    private void HideAll()
    {
        _puzzlePanel.SetActive(false);
        _victoryPanel.SetActive(false);
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }
}
