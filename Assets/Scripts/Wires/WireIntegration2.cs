using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

        public float stiffness = 1000;
        public float segmentMass = 1f;
        public float kDamp = 0.05f;
        public float drag = 0.05f;

        public float segColRadius = 0.05f;
        public float tautThreshold = 0.1f;

        public float bounceLength = 0.05f;
        public float minGravityFactor = 0.01f;
        
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
        }

        private List<IRopeSegment> _copySegs = new();

        private void Euler(List<IRopeSegment> segments){
            foreach (var seg in segments){
                seg.Force = Vector2.zero;
            }

            for (var i = 0; i < segments.Count; i++){
                var cur = segments[i];
                if (i < segments.Count - 1){
                    var next = segments[i + 1];
                    cur.Force += ComputeForce(next, cur);
                }

                if (i > 0){
                    var prev = segments[i - 1];
                    cur.Force += ComputeForce(prev, cur);
                }
            }

            foreach (var seg in segments){
                if (StartSegment is Plug2 startPlug && EndSegment is Plug2 endPlug){
                    if ((startPlug.state is Plug2.State.Free || endPlug.state is Plug2.State.Free)){
                        seg.Force += ComputeGravity(seg);
                    } else{
                        var deviate = (GetMaxLength() - CurrentLength - tautThreshold) / GetMaxLength();
                        deviate = Mathf.Clamp(deviate, 0, 1);
                        var factor = Mathf.Lerp(minGravityFactor, 1, deviate);
                        seg.Force += ComputeGravity(seg) * factor;
                    }
                } else{
                    seg.Force += ComputeGravity(seg);
                }
                seg.UpdateState(Time.fixedDeltaTime);
                seg.AvoidCollision();
            }
            
            UpdateCurrentLength();
        }
        
        private Vector2[] _curPosArray, _curVelArray, _curForceArray;
        private void ApplyConstraints(IReadOnlyList<IRopeSegment> segments){
            
            // TODO: avoid new arrays on the fly
            Vector2[] curPosArray = new Vector2[segments.Count], 
                curVelArray = new Vector2[segments.Count], 
                curForceArray = new Vector2[segments.Count];
            
            for(var i = 0; i<segments.Count; i++){
                var seg = segments[i];
                curPosArray[i] = seg.Position;
                curVelArray[i] = seg.Velocity;
                curForceArray[i] = seg.Force;
            }
            
            for (int i = 1; i < segments.Count; i++){
                var cur = segments[i];
                var prev = segments[i - 1];
                var prevPos = curPosArray[i - 1];
                var curPos = curPosArray[i];
                var posDiff = curPos - prevPos;
                var deviate = posDiff.magnitude - _segmentLength;
                if (deviate < 0) continue;
                // Position Constraint
                // Move both points closer a little bit
                var posDiffNorm = posDiff.normalized;
                var posMod = 0.5f * deviate * posDiffNorm;
                prev.Position += posMod;
                cur.Position -= posMod;

                // Velocity Constraint
                var t = Vector2.Dot(curVelArray[i], posDiffNorm);
                if (t > 0){
                    cur.Velocity -= t * posDiffNorm;
                }

                t = Vector2.Dot(curVelArray[i - 1], posDiffNorm);
                if (t < 0){
                    prev.Velocity -= t * posDiffNorm;
                }

                cur.AvoidCollision();
            }
            segments[0].AvoidCollision();
        }

        // Force on a;
        private Vector2 ComputeForce(IRopeSegment b, IRopeSegment a){
            var posDiff = a.Position - b.Position;
            var posDir = posDiff.normalized;
            var lenDiff = Mathf.Max(posDiff.magnitude - bounceLength, 0);
            var ret = - (lenDiff * lenDiff) * stiffness * posDir;
            var velDiff = a.Velocity - b.Velocity;
            ret += -kDamp * Vector2.Dot(velDiff, posDiff) / posDiff.magnitude * posDir;
            ret += -a.Velocity * drag;
            return ret;
        }
        
        private Vector2 ComputeGravity(IRopeSegment s){
            return Physics2D.gravity;
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