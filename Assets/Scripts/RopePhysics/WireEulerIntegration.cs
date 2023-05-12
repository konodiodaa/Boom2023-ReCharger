using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.Mathematics;
using System;
using JetBrains.Annotations;
using UnityEngine.Assertions;
public class WireEulerIntegration : MonoBehaviour
{
    public struct RopeSegment
    {
        public Vector2 CurrPos;
        public Vector2 PrevPos;

        public RopeSegment(Vector3 initPos)
        {
            CurrPos = initPos;
            PrevPos = initPos;
        }


    }

    [SerializeField] private Transform _startPoint;
    [SerializeField] private Transform _endPoint;
    [SerializeField] private LayerMask _collisionMask;

    [SerializeField] private float _width = 0.1f;
    [SerializeField] private Color _color = Color.black;

    [SerializeField] private int _segmentCount = 30;
    [SerializeField] private float _segmentLength = 0.25f;

    [SerializeField] private Vector2 _forceGravity = new Vector2(0f, -1.0f);
    [SerializeField] private int _solveStep = 30;
    
    private List<RopeSegment> _segments = new List<RopeSegment>();
    private LineRenderer _cuttedLineRenderer;
    private LineRenderer _lineRenderer;
    
    private int _wireCuttedIndex = -2;
    public Action OnWireCut;

    private void Awake()
    {
        LineRenderer[] lineRenderers = GetComponentsInChildren<LineRenderer>();
        if (lineRenderers[0].gameObject == this.gameObject)
        {
            _lineRenderer = lineRenderers[0];
            _cuttedLineRenderer = lineRenderers[1];
        }
        else
        {
            _lineRenderer = lineRenderers[1];
            _cuttedLineRenderer = lineRenderers[0];
        }

        
        _lineRenderer.positionCount = _segmentCount;
        _lineRenderer.startWidth = _width;
        _lineRenderer.endWidth = _width;
        _lineRenderer.startColor = _color;
        _lineRenderer.endColor = _color;
        _lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        
        
        
        _cuttedLineRenderer.positionCount = 0;
        _cuttedLineRenderer.startWidth = _width;
        _cuttedLineRenderer.endWidth = _width;
        _cuttedLineRenderer.startColor = _color;
        _cuttedLineRenderer.endColor = _color;
        _cuttedLineRenderer.material = new Material(Shader.Find("Sprites/Default"));

        Debug.Log(_lineRenderer.gameObject == _cuttedLineRenderer.gameObject);
    }

    private void Start()
    {
        Vector2 startPosition = _startPoint.position;
        Vector2 endPosition = _endPoint.position;
        Vector2 initPosition;
        for(int i = 0; i < _segmentCount; i++)
        {
            initPosition = Vector2.Lerp(startPosition, endPosition, (float)i / _segmentCount);
            _segments.Add(new RopeSegment(initPosition));
        }
    }
    private void FixedUpdate()
    {
        Simulate();
        DrawRope();
    }
   
    private void DrawRope()
    {
        
        if (_wireCuttedIndex == -2)
        {
            Vector3[] segmentPositions = new Vector3[_segmentCount];
            for (int i = 0; i < _segmentCount; i++)
            {
                segmentPositions[i] = _segments[i].CurrPos;
            }
            _lineRenderer.SetPositions(segmentPositions);
        }
        else
        {
            Vector3[] wireFirstPart = new Vector3[_wireCuttedIndex + 1];
            Vector3[] wireSecondPart = new Vector3[_segmentCount - _wireCuttedIndex - 1];

            _lineRenderer.positionCount = wireFirstPart.Length;
            _cuttedLineRenderer.positionCount = wireSecondPart.Length;

            for (int i = 0; i < _wireCuttedIndex + 1; i++)
            {
                wireFirstPart[i] = _segments[i].CurrPos;
                
            }
            for(int j = _wireCuttedIndex + 1; j < _segmentCount; j ++)
            {
                wireSecondPart[j - (_wireCuttedIndex + 1)] = _segments[j].CurrPos;
            
            }

            _lineRenderer.SetPositions(wireFirstPart);
            _cuttedLineRenderer.SetPositions(wireSecondPart);
        }

    }

    private void Simulate()
    {
        for(int i = 0; i < _segmentCount; i++)
        {
            RopeSegment segment = _segments[i];
            Vector2 velocity = segment.CurrPos - segment.PrevPos;
            segment.PrevPos = segment.CurrPos;
            segment.CurrPos += velocity;
            segment.CurrPos += _forceGravity * Time.deltaTime;
            _segments[i] = segment;
        }
        for(int i = 0; i < _solveStep; i++)
        {
            ApplyConstraint();
        }
    }

    private void ApplyConstraint()
    {
        RopeSegment firstSegment = _segments[0];
        firstSegment.CurrPos = _startPoint.position;
        _segments[0] = firstSegment;

        RopeSegment lastSegment = _segments[_segmentCount - 1];
        lastSegment.CurrPos = _endPoint.position;
        _segments[_segments.Count - 1] = lastSegment;

        for(int i = 0; i < _segmentCount - 1; i++)
        {
            RopeSegment currSegment = _segments[i];
            RopeSegment nextSegment = _segments[i + 1];

            //Rope 
            float dist = (currSegment.CurrPos - nextSegment.CurrPos).magnitude;
            float deviate = Mathf.Abs(dist - _segmentLength);
            Vector2 changeDir = Vector2.zero;

            if(dist > _segmentLength)
            {
                changeDir = (currSegment.CurrPos - nextSegment.CurrPos).normalized;
            }
            else if(dist < _segmentLength)
            {
                changeDir = (nextSegment.CurrPos - currSegment.CurrPos).normalized;
            }

            currSegment.CurrPos -= changeDir * deviate * 0.5f;
            nextSegment.CurrPos += changeDir * deviate * 0.5f;

            if (i != _wireCuttedIndex)
            {
                _segments[i] = currSegment;
                _segments[i + 1] = nextSegment;
            }
            //Collision Physics Simulation.
            Collider2D hitCollider = Physics2D.OverlapCircle(currSegment.CurrPos, _width, _collisionMask);
            if (hitCollider)
            {
                //Wire Cut
                WireCutter cutter = hitCollider.gameObject.GetComponent<WireCutter>();
                if(cutter && _wireCuttedIndex == -2)
                {
                    _wireCuttedIndex = i;
                    OnWireCut?.Invoke();
                    Debug.Log("Wire got cutted");
                }

                Transform hitTransform = hitCollider.transform;
                if (hitCollider is CircleCollider2D)
                {
                    CircleCollider2D circleCollider = (CircleCollider2D)hitCollider;
                    float hitColliderRadius = circleCollider.radius * hitTransform.lossyScale.x;
                    Vector2 hitColliderCenter = hitTransform.position;
                    Vector2 hitResolvePos = hitColliderCenter + (currSegment.CurrPos - hitColliderCenter).normalized * (hitColliderRadius + _width);

                    //Debug.DrawLine(p.posCurrent, hitResolvePos);
                    currSegment.CurrPos = hitResolvePos;
                }
                else if (hitCollider is BoxCollider2D)
                {
                    BoxCollider2D boxCollider = (BoxCollider2D)hitCollider;
                    Vector2 boxColliderSize = boxCollider.size * hitTransform.lossyScale;
                    Vector2 hitColliderCenter = hitTransform.position;

                    Vector2 hitResolvePos = currSegment.CurrPos;
                    Vector2 boxMin = hitColliderCenter - boxColliderSize / 2 - Vector2.one * _width;
                    Vector2 boxMax = hitColliderCenter + boxColliderSize / 2 + Vector2.one * _width;

                    float moveRight = boxMax.x - currSegment.CurrPos.x;
                    float moveLeft  = -boxMin.x + currSegment.CurrPos.x;
                    float moveTop   = boxMax.y - currSegment.CurrPos.y;
                    float moveDown  = -boxMin.y + currSegment.CurrPos.y;

                    float moveMin = math.min(math.min(moveRight, moveLeft), math.min(moveTop, moveDown));
                    if (moveMin == moveRight)
                    {
                        hitResolvePos.x += moveRight;
                    }
                    else if (moveMin == moveLeft)
                    {
                        hitResolvePos.x -= moveLeft;
                    }
                    else if (moveMin == moveTop)
                    {
                        hitResolvePos.y += moveTop;
                    }
                    else
                    {
                        hitResolvePos.y -= moveDown;
                    }

                    //Debug.DrawLine(p.posCurrent, hitResolvePos);
                    currSegment.CurrPos = hitResolvePos;
                }
                //Update Again.
                _segments[i] = currSegment;


            }
        }
    }
    public void OnDrawGizmos()
    {
        for (int i = 0; i < this._segments.Count; i++)
        {
            Gizmos.DrawSphere(_segments[i].CurrPos, _width / 2);
        }
    }

}
