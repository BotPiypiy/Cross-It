using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIController : MonoBehaviour
{
    [SerializeField] private Button _bombButton;

    private PlayerController _playerController;

    private Coroutine _bombButttonCoroutine;

    private void Start()
    {
        _playerController = GameManager.Instance.PlayerController;

        SubscribeToEvents();
    }

    private void SubscribeToEvents()
    {
        GameManager.Instance.OnCombatModeEnter += OnCombatModeEnter;
        GameManager.Instance.OnCombatModeExit += OnCombatModeExit;

        _bombButton.onClick.AddListener(OnBombClicked);
    }

    private void OnCombatModeEnter()
    {
        _bombButton.gameObject.SetActive(true);
    }

    private void OnCombatModeExit()
    {
        _bombButton.gameObject.SetActive(false);
        StopCoroutine();
    }

    private void OnBombClicked()
    {
        _playerController.PlaceBomb();
        _bombButttonCoroutine = StartCoroutine(DisableBombButton());

    }

    private IEnumerator DisableBombButton()
    {
        _bombButton.gameObject.SetActive(false);
        yield return new WaitForSeconds(_playerController.BombReloadingTime);
        _bombButton.gameObject.SetActive(true);
    }

    private void StopCoroutine()
    {
        if (_bombButttonCoroutine != null)
        {
            StopCoroutine(_bombButttonCoroutine);
        }
    }

    private void UnsubscribeFromEvents()
    {
        _bombButton.onClick.RemoveAllListeners();
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
        StopCoroutine();
    }
}
