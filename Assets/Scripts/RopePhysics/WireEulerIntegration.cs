using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private LineRenderer _lineRenderer;
    private List<RopeSegment> _segments = new List<RopeSegment>();
    [SerializeField] private Transform _startPoint;
    [SerializeField] private Transform _endPoint;

    [SerializeField] private float _width = 0.1f;
    [SerializeField] private Color _color = Color.black;

    [SerializeField] private int _segmentCount = 30;
    [SerializeField] private float _segmentLength = 0.25f;

    [SerializeField] private Vector2 _forceGravity = new Vector2(0f, -1.0f);
    [SerializeField] private int _solveStep = 30;

    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.positionCount = _segmentCount;
        _lineRenderer.startWidth = _width;
        _lineRenderer.endWidth = _width;
        _lineRenderer.startColor = _color;
        _lineRenderer.endColor = _color;
        _lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        
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
        Vector3[] segmentPositions = new Vector3[_segmentCount];
        for(int i = 0; i < _segmentCount; i++)
        {
            segmentPositions[i] = _segments[i].CurrPos;
        }
        _lineRenderer.SetPositions(segmentPositions);
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

            currSegment.CurrPos -= changeDir * (deviate * 0.5f);
            nextSegment.CurrPos += changeDir * (deviate * 0.5f);
            _segments[i] = currSegment;
            _segments[i + 1] = nextSegment;

        }
    }


}
