using System.Collections;
using System.Collections.Generic;
using Switches;
using UnityEngine;
using Utility;

public class MovePlatform : MonoBehaviour, ISwitchControled
{

    [Header("MovePlane")]
    public float speed;

    public Transform endPosition;

    public Outliner outliner;

    private float cur_dist;
    private bool reverse;
    private Vector3 _startPosition;

    public Vector2 CurrentVelocity => _isActive ? (reverse ? -_direction * speed : _direction * speed) : Vector2.zero;

    public bool IsActive => _isActive;

    private Vector3 _endPosition;

    private void Awake()
    {
        cur_dist = 0;
        reverse = false;
        _startPosition = transform.position;
        _endPosition = endPosition.position;
        _direction = ((Vector2)(_endPosition - _startPosition)).normalized;
        _totalTime = (_endPosition - _startPosition).magnitude / speed;
        _body = GetComponent<Rigidbody2D>();
    }

    private float _totalTime = 0f;
    private Vector2 _direction;

    private bool _isActive = false;

    private float _curTime = 0;
    private Rigidbody2D _body;

    private void FixedUpdate(){
        if (!_isActive) return;
        _curTime += reverse ? -Time.fixedDeltaTime : Time.fixedDeltaTime;
        var i = Mathf.Clamp(_curTime / _totalTime, 0, 1);
        _body.MovePosition(Vector3.Lerp(_startPosition, _endPosition, i));
        if (_curTime < 0){
            _curTime = 0;
            reverse = false;
        } else if (_curTime > _totalTime){
            _curTime = _totalTime;
            reverse = true;
        }
    }

    private void ApplyVelocity(Collision2D collision){
        if (!_isActive) return; //mech.getActivate())
        var x = Time.fixedDeltaTime * speed * _direction;
        if (!reverse)
            collision.transform.Translate(x.x, x.y, 0f);
        else
            collision.transform.Translate(-x.x, -x.y, 0f);
    }

    private void OnCollisionEnter2D(Collision2D collision){
        ApplyVelocity(collision);
    }
    
    private void OnCollisionStay2D(Collision2D collision){
        ApplyVelocity(collision);
    }
    
    private void OnCollisionExit2D(Collision2D collision){
        ApplyVelocity(collision);
    }

    public void OnTurnedOn(){
        _isActive = true;
    }

    public void OnTurnedOff(){
        _isActive = false;
    }

    public void Highlight(){
        outliner.enabled = true;
    }

    public void RemoveHighlight(){
        outliner.enabled = false;
    }
}
