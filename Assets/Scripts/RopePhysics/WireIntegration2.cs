using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RopePhysics{

    public interface IRopeSegment{
        float Mass{ get; }
        Vector2 Velocity{ get; }
        Vector2 Position{ get; }
        Vector2 Acceleration{ get; }
        Vector2 Force{ set; get; }
        void UpdateState(float deltaTIme);
    }
    
    public class WireIntegration2 : MonoBehaviour{
        public class RopeSegment: IRopeSegment{
            public RopeSegment(Vector3 initPos){
                Position = initPos;
            }

            public float Mass{ get; set; } = 1;
            public Vector2 Velocity{ get; set; } = Vector2.zero;
            public Vector2 Position{ get; set; }
            public Vector2 Acceleration => Force * (1 / Mass);

            public Vector2 Force{ set; get; } = Vector2.zero;
            public float ColRadius = 0.15f;
            public float KPenetration = 2000f;

            public void UpdateState(float deltaTIme){
                var newVelocity = Velocity + Force * (deltaTIme / Mass);
                var newPos = Velocity * deltaTIme + Position;
                var col = Physics2D.OverlapCircle(newPos, ColRadius, LayerMask.GetMask("Ground"));
                if (col != null){
                    var closestPoint = col.ClosestPoint(Position);
                    var dir = Position - closestPoint;
                    var penetration = ColRadius - (closestPoint - newPos).magnitude;
                    Force += dir.normalized * (penetration * KPenetration);
                    newVelocity = Velocity + Force * (deltaTIme / Mass);
                    newPos = Position + newVelocity * deltaTIme;
                }
                Position = newPos;
                Velocity = newVelocity;
            }
        }

        public class FixedSegment : IRopeSegment{

            public float Mass{ get; set; } = 1;
            public Vector2 Velocity{ get; set; } = Vector2.zero;
            public Vector2 Position{ get; set; } = Vector2.zero;
            public Vector2 Acceleration => Force * (1 / Mass);

            public Vector2 Force{ set; get; } = Vector2.zero;
            public void UpdateState(float deltaTIme){ }
        }

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
        public float kPenetration = 2000f;

        public float segColRadius = 0.05f;
        
        


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
            for (int i = 0; i < _segmentCount; i++){
                var initPosition = Vector2.Lerp(startPosition, endPosition, (float)(i + 1) / (_segmentCount + 1));
                _segments.Add(new RopeSegment(initPosition){
                    Mass = segmentMass,
                    ColRadius = segColRadius,
                    KPenetration = kPenetration,
                });
            }
            _segments.Add(EndSegment);
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
            for (int i = 0; i < _solveStep; i++){
                Euler(_segments);
            }
        }

        private void Rk4(){
            
        }

        private void Euler(List<IRopeSegment> segments){
            foreach (var seg in segments){
                seg.Force = Vector2.zero;
            }

            for (var i = 0; i < segments.Count; i++){
                var cur = _segments[i];
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
                seg.Force += ComputeGravity(seg);
                seg.UpdateState(Time.fixedDeltaTime/_solveStep);
            }
        }
        
        // Force on a;
        private Vector2 ComputeForce(IRopeSegment b, IRopeSegment a){
            var posDiff = a.Position - b.Position;
            var posDir = posDiff.normalized;
            var lenDiff = Mathf.Max(posDiff.magnitude - _segmentLength, 0);
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

        public float GetTheoreticalLength(){
            return (_segmentCount + 1) * _segmentLength;
        }
    }
}