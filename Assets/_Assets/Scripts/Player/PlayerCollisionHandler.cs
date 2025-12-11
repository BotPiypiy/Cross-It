using UnityEngine;

[RequireComponent(typeof(CubeMovement))]
[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerCollisionHandler : MonoBehaviour
{
    [SerializeField] private string _combatTriggerTag;
    [SerializeField] private string _chestTag;

    [SerializeField] private LayerMask _enemyLayer;
    [SerializeField] private LayerMask _obstacleLayer;
    [SerializeField] private LayerMask _triggerLayer;

    [SerializeField] private CubeMovement _cubeMovement;

    private bool _ignoreCollision;

    private void Awake()
    {
        if (_cubeMovement == null)
        {
            _cubeMovement = GetComponent<CubeMovement>();
        }

        _ignoreCollision = false;
    }

    private void Start()
    {
        SubscribeToEvents();
    }

    private void SubscribeToEvents()
    {
        GameManager.Instance.OnPuzzleEnter += () => _ignoreCollision = true;
        GameManager.Instance.OnPuzzleExit += () => _ignoreCollision = false;
    }

    private void UnsubscribeFromEvents()
    {
        GameManager.Instance.OnPuzzleEnter -= () => _ignoreCollision = true;
        GameManager.Instance.OnPuzzleExit -= () => _ignoreCollision = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(_ignoreCollision)
        {
            return;
        }

        if (_enemyLayer == (_enemyLayer | (1 << other.gameObject.layer)))
        {
            GameManager.Instance.Restart();
        }

        if (_obstacleLayer == (_obstacleLayer | (1 << other.gameObject.layer)))
        {
            _cubeMovement.ReverseCurrentAnimation();

            if (other.CompareTag(_chestTag))
            {
                GameManager.Instance.StartPuzzle();
            }
        }

        if (other.CompareTag(_combatTriggerTag))
        {
            GameManager.Instance.StartCombat();
        }
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }
}
