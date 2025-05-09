using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadbobController : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private bool _enable = true;
    [SerializeField, Range(0, 0.1f)] private float _Amplitude = 0.015f;
    [SerializeField, Range(0, 30)] private float _frequency = 10.0f;
    [SerializeField] private Transform _camera = null;

    private float _toggleSpeed = 3.0f;
    private Vector3 _startPos;
    private CharacterController _controller;

    private void Awake()
    {
        _controller = GetComponentInParent<CharacterController>();
        if (_camera != null) _startPos = _camera.localPosition;
    }

    void Update()
    {
        if (!_enable || _camera == null || _controller == null) return;
        CheckMotion();
        ResetPosition();
        if (Input.GetKey(KeyCode.LeftShift))
        {
            _Amplitude = 0.1f;
            _frequency = 12f;
        }
        else 
        {
            _Amplitude = 0.05f;
            _frequency = 7f;
        } 
            
    }

    private Vector3 FootStepMotion()
    {
        Vector3 pos = Vector3.zero;
        pos.y = Mathf.Sin(Time.time * _frequency) * _Amplitude;
        pos.x = Mathf.Cos(Time.time * _frequency / 2) * _Amplitude * 2;
        return pos;
    }

    private void CheckMotion()
    {
        float forwardSpeed = transform.InverseTransformDirection(_controller.velocity).z;
        if (Mathf.Abs(forwardSpeed) < _toggleSpeed || !_controller.isGrounded) return;

        PlayMotion(FootStepMotion());
    }

    private void PlayMotion(Vector3 motion)
    {
        _camera.localPosition = _startPos + motion; 
    }

    private void ResetPosition()
    {
        _camera.localPosition = Vector3.Lerp(_camera.localPosition, _startPos, Time.deltaTime * 5f);
    }

}