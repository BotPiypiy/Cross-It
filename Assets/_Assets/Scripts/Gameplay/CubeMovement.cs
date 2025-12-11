using DG.Tweening;
using UnityEngine;

public class CubeMovement : MonoBehaviour
{
    public const float ROLL_ANGLE = 90f;

    [SerializeField] private float _stepDistance = 1f;
    [SerializeField] private float _moveDuration = 0.3f;
    [SerializeField] private Ease _moveEase = Ease.OutCubic;

    private Vector3 _moveDirection = Vector3.zero;
    private Sequence _moveSequence;
    private Vector3 _targetPosition;

    public float MoveDuration => _moveDuration;
    public bool IsMoving => _moveDirection != Vector3.zero;

    public bool TryMove(Vector3 direction)
    {
        if (direction == Vector3.zero)
        {
            return false;
        }

        StartMove(direction);
        return true;
    }

    private void StartMove(Vector3 direction)
    {
        _moveDirection = direction.normalized;

        _targetPosition = transform.position + direction * _stepDistance;

        _moveSequence = DOTween.Sequence();

        CreateAnimation(direction);

        _moveSequence
            .OnComplete(() => CompleteMove())
            .OnKill(() => KillMove())
            .SetEase(_moveEase)
            .SetLink(gameObject);
    }


    private void CreateAnimation(Vector3 direction)
    {

        Vector3 rotationAxis = GetRotationAxis(direction);
        float rotationAngle = (direction == Vector3.back || direction == Vector3.right) ? -ROLL_ANGLE : ROLL_ANGLE;

        _moveSequence.Join(
            transform.DOMove(_targetPosition, _moveDuration)
        );

        _moveSequence.Join(
            transform.DORotate(rotationAxis * rotationAngle, _moveDuration, RotateMode.LocalAxisAdd)
                .SetEase(_moveEase)
        );

        _moveSequence.AppendCallback(() => SetFinalTransform());
    }

    public void ReverseCurrentAnimation()
    {
        _moveSequence.Flip();
        CompleteMove();
    }

    private Vector3 GetRotationAxis(Vector3 direction)
    {
        if (direction == Vector3.forward || direction == Vector3.back)
        {
            return Vector3.right;
        }
        else
        {
            return Vector3.forward;
        }
    }

    private void SetFinalTransform()
    {
        transform.position = _targetPosition;
        transform.rotation = Quaternion.identity;
    }

    private void CompleteMove()
    {
        SetFinalTransform();

        _moveDirection = Vector3.zero;
    }

    private void KillMove()
    {
        _moveDirection = Vector3.zero;
    }

    public void StopMovement()
    {
        if (_moveSequence != null && _moveSequence.IsActive())
        {
            _moveSequence.Kill();
            SetFinalTransform();
        }
    }

    public void ForceStop()
    {
        if (_moveSequence != null && _moveSequence.IsActive())
        {
            _moveSequence.Kill();
        }
        _moveDirection = Vector3.zero;
    }


    private void OnDestroy()
    {
        ForceStop();
    }
}
