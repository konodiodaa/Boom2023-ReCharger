using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace Wires{
    public partial class WireIntegration2 : MonoBehaviour{
        private LineRenderer _lineRenderer;
        private List<IRopeSegment> _segments = new List<IRopeSegment>();
        [SerializeField] private Transform _startPoint;
        [SerializeField] private Transform _endPoint;

        public IRopeSegment StartSegment = new FixedSegment();
        public IRopeSegment EndSegment = new FixedSegment();
        

        [SerializeField] private float _width = 0.1f;
        [SerializeField] private Color _color = Color.black;
        
        [Min(1)]
        [SerializeField] private int _segmentCount = 10;
        [SerializeField] private float _segmentLength = 0.25f;
        
        [SerializeField] private int _solveStep = 5;

        public float segmentMass = 1f;

        public float segColRadius = 0.05f;
        
        [NonSerialized]
        public float CurrentLength = 0;


        private void Awake(){
            _lineRenderer = GetComponent<LineRenderer>();
            _lineRenderer.positionCount = _segmentCount + 2;
            _lineRenderer.startWidth = _width;
            _lineRenderer.endWidth = _width;
            _lineRenderer.startColor = _color;
            _lineRenderer.endColor = _color;
            _lineRenderer.material = new Material(Shader.Find("Sprites/Default"));

        }

        private void Start(){
            if(_startPoint != null || _endPoint != null) CreateSegments();
        }

        public void CreateSegments(IRopeSegment start = null, IRopeSegment end = null){
            if (start != null){
                StartSegment = start;
            } else if (StartSegment is FixedSegment fixedSeg){
                fixedSeg.Position = _startPoint.position;
            }

            if (end != null){
                EndSegment = end;
            } else if (EndSegment is FixedSegment endSeg){
                endSeg.Position = _endPoint.position;
            }
            
            var startPosition = StartSegment.Position;
            var endPosition = EndSegment.Position;
            
            _segments.Add(StartSegment);
            var prev = StartSegment;
            for (int i = 0; i < _segmentCount; i++){
                var initPosition = Vector2.Lerp(startPosition, endPosition, (float)(i + 1) / (_segmentCount + 1));
                _segments.Add(new RopeSegment(initPosition){
                    PrevSeg = prev,
                    integrator = this
                });
                prev.NextSeg = _segments[^1];
                prev = _segments[^1];
            }
            _segments.Add(EndSegment);
            EndSegment.PrevSeg = prev;

            foreach (var s in _segments){
                s.NextPosition = s.Position;
            }
        }

        private void FixedUpdate(){
            if (_segments.Count == 0) return;
            Simulate();
            DrawRope();
        }

        private void DrawRope(){
            _lineRenderer.SetPositions(_segments.Select(s => new Vector3(s.Position.x, s.Position.y, transform.position.z)).ToArray());
        }

        private void Simulate(){
            
            Euler(_segments);
            for (int i = 0; i < _solveStep; i++){
                ApplyConstraints(_segments);
            }
            UpdateVelocity(_segments);
            UpdateCurrentLength();
        }

        private List<IRopeSegment> _copySegs = new();
        private readonly List<Vector2> _nextPositions = new();

        private void Euler(List<IRopeSegment> segments){
            foreach (var seg in segments){
                DampVelocity(seg);
                seg.Velocity += Physics2D.gravity * Time.fixedDeltaTime;
                seg.NextPosition = seg.Position + seg.Velocity * Time.fixedDeltaTime;
            }
        }

        public float dampingFactor = 0.005f;

        private void DampVelocity(IRopeSegment seg){
            var f = Mathf.Clamp01(1 - seg.Velocity.sqrMagnitude / seg.Mass * dampingFactor);
            seg.Velocity *= f;
        }
        

        private void ApplyConstraints(IReadOnlyList<IRopeSegment> segments){

            for (int i = 1; i < segments.Count; i++){
                ApplyDistanceConstraint(segments[i], segments[i-1]);
                segments[i].NextPosition = segments[i].AvoidCollision(segments[i].NextPosition);
            }
            segments[0].NextPosition = segments[0].AvoidCollision(segments[0].NextPosition);
        }

        private void ApplyDistanceConstraint(IRopeSegment cur, IRopeSegment prev){
            var posDiff = prev.NextPosition -  cur.NextPosition;
            var deviate = posDiff.magnitude - _segmentLength;
            if (deviate < 0) return;
            var posDiffNorm = posDiff.normalized;
            var posMod =  deviate * posDiffNorm;
            var totalMass = prev.Mass + cur.Mass;
            prev.NextPosition -= posMod * cur.Mass / totalMass;
            cur.NextPosition += posMod * prev.Mass / totalMass;
        }

        private void UpdateVelocity(IEnumerable<IRopeSegment> segments){
            foreach (var seg in segments){
                var newVel = (seg.NextPosition - seg.Position) / Time.fixedDeltaTime;
                var acc = (newVel - seg.Velocity) / Time.fixedDeltaTime;
                seg.Force = acc * seg.Mass;
                seg.Velocity = newVel;
                seg.Position = seg.NextPosition;
            }
        }

        private void PrintDetails(){
            var s = "";
            var s2 = "";
            foreach (var seg in _segments){
                s += seg.Position.ToString();
                s += ", ";
                s2 += seg.Velocity.ToString();
                s2 += ", ";
            }

            Debug.Log($"Positions: {s}");
            Debug.Log($"Velocities: {s2}");
        }

        public void SetColor(Color c){
            _lineRenderer.startColor = c;
            _lineRenderer.endColor = c;
        }

        public float GetMaxLength(){
            return (_segmentCount + 1) * _segmentLength;
        }

        private void UpdateCurrentLength(){
            CurrentLength = 0f;
            for (var i = 1; i < _segments.Count; i++){
                CurrentLength += (_segments[i].Position - _segments[i - 1].Position).magnitude;
            }
        }

        public List<Vector2> GetPositions() => _segments.Select(s => s.Position).ToList();
        public int GetSegmentsCount() => _segments.Count;

        public int GetPositions(List<Vector2> res){
            for (int i = 0; i < _segments.Count; i++){
                if (res.Count <= i){
                    res.Add(_segments[i].Position);
                } else{
                    res[i] = _segments[i].Position;
                }
            }

            return _segments.Count;
        }
    }
}