using UnityEngine;

[RequireComponent(typeof(EnemyController))]
[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(Rigidbody))]
public class EnemyCollisionHandler : MonoBehaviour
{
    [SerializeField] private string _borderTag;
    [SerializeField] private LayerMask _enemyLayer;
    [SerializeField] private LayerMask _obstacleLayer;

    [SerializeField] private EnemyController _enemyController;

    private void Awake()
    {
        if (_enemyController == null)
        {
            _enemyController = GetComponent<EnemyController>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_enemyLayer == (_enemyLayer | (1 << other.gameObject.layer))
            || (_obstacleLayer == (_obstacleLayer | (1 << other.gameObject.layer))))
        {
            if (other.CompareTag(_borderTag))
            {
                _enemyController.ReturnToPool();
            }
            else
            {
                _enemyController.ReverseDirection();
            }
        }

        if (other.gameObject.TryGetComponent<BombController>(out _))
        {
            _enemyController.ReturnToPool();
        }
    }
}
