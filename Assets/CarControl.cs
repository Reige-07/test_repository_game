using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarControl : MonoBehaviour
{
    [SerializeField]
    private WheelInfo _FL, _FR, _BL, _BR;

    [SerializeField]
    private AudioSource _idleS, _breakS, _nitroS, _moveS;

    [SerializeField]
    private List<ParticleSystem> _breakeP = new List<ParticleSystem>();

    [SerializeField]
    private List<ParticleSystem> _nitroP = new List<ParticleSystem>();

    [SerializeField]
    private float _motor = 800f, _steer = 50f, _break = 440f, _NITROOO = 1600, _actualMotorSpeed;

    private float _vertical;
    private float _horizontal;

    private Vector3 _position;
    private Quaternion _rotation;


    private void Update()
    {
        _vertical = Input.GetAxis("Vertical");
        _horizontal = Input.GetAxis("Horizontal");

        if(_vertical != 0)
        {
            _idleS.Pause();
            _moveS.UnPause();
        }
        else
        {
            _idleS.UnPause();
            _moveS.Pause();
        }
    }


    private void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.LeftControl))
        {
            _actualMotorSpeed = _NITROOO;
            for (int i = 0; i < _nitroP.Count; i++)
            {
                _nitroP[i].Play();
                
            }
            if (!_nitroS.isPlaying)
            {
                _nitroS.PlayOneShot(_nitroS.clip);
            }
        }
        else
        {
            _actualMotorSpeed = _motor;
            for (int i = 0; i < _nitroP.Count; i++)
            {
                _nitroP[i].Stop();
            }
        }

        if (Input.GetKey(KeyCode.Space))
        {
            _FL.wheelCollider.brakeTorque = _break;
            _FR.wheelCollider.brakeTorque = _break;
            _BL.wheelCollider.brakeTorque = _break;
            _BR.wheelCollider.brakeTorque = _break;

            if (!_breakS.isPlaying)
            {
                _breakS.PlayOneShot(_breakS.clip);
            }

            for(int i = 0; i < _breakeP.Count; i++)
            {
                _breakeP[i].Play();
            }
        }
        else
        {
            _FL.wheelCollider.brakeTorque = 0;
            _FR.wheelCollider.brakeTorque = 0;
            _BL.wheelCollider.brakeTorque = 0;
            _BR.wheelCollider.brakeTorque = 0;

            for (int i = 0; i < _breakeP.Count; i++)
            {
                _breakeP[i].Stop();
            }
        }

        _BL.wheelCollider.motorTorque = _actualMotorSpeed * _vertical;
        _BR.wheelCollider.motorTorque = _actualMotorSpeed * _vertical;

        _FL.wheelCollider.steerAngle = _steer * _horizontal;
        _FR.wheelCollider.steerAngle = _steer * _horizontal;

        UpdateVisualWheels();
    }

    private void UpdateVisualWheels()
    {

        _FL.wheelCollider.GetWorldPose(out _position, out _rotation);
        _FL.visualWheel.position = _position;
        _FL.visualWheel.rotation = _rotation;

        _FR.wheelCollider.GetWorldPose(out _position, out _rotation);
        _FR.visualWheel.position = _position;
        _FR.visualWheel.rotation = _rotation;

        _BL.wheelCollider.GetWorldPose(out _position, out _rotation);
        _BL.visualWheel.position = _position;
        _BL.visualWheel.rotation = _rotation;

        _BR.wheelCollider.GetWorldPose(out _position, out _rotation);
        _BR.visualWheel.position = _position;
        _BR.visualWheel.rotation = _rotation;
    }
}

[System.Serializable]
public struct WheelInfo
{
    public Transform visualWheel;
    public WheelCollider wheelCollider;
}