using UnityEngine;

public class CameraBehavior : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private float _trackingSpeed = 1f;
    [SerializeField] private Vector3 _offset;

    private enum CameraMode
    {
        Static,
        Track,
    }

    [SerializeField] private CameraMode _mode;

    private void Awake()
    {
        _mode = CameraMode.Track;
    }

    private void Start()
    {
        _offset = _offset == Vector3.zero ? transform.position - _target.position : _offset;
        transform.position = _target.position + _offset;
    }

    private void LateUpdate()
    {
        if (_mode == CameraMode.Track)
        {
            transform.position = Vector3.Lerp(transform.position, _target.position + _offset, _trackingSpeed * Time.deltaTime);
        }
    }
}
