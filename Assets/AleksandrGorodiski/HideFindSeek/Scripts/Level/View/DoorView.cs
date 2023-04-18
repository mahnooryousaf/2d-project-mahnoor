using System;
using UnityEngine;


namespace Game
{
    public sealed class DoorView : MonoBehaviour
    {
        public Action<Collider> COLLISION;

        private const float kColliderRadius = .8f;

        [SerializeField] private BoxCollider _box;
        [SerializeField] private float _angleOffset;
        [SerializeField] private float _lerpFactor;
        [SerializeField] private float _speedFading;

        private float _speed;
        private float _angle;
        private float _startAngle;
        private Transform _lastDetectedCollisionTarget;
        private int _layerMask;

        private void Awake()
        {
            _startAngle = transform.eulerAngles.y;
            _layerMask = LayerMask.GetMask("Player", "Enemy");
        }

        public float GetAngle(Vector3 start, Vector3 end)
        {
            return Mathf.Atan2(start.z - end.z, start.x - end.x) * Mathf.Rad2Deg;
        }

        public  int Sign(double value)
        {
            return (value >= 0) ? 1 : -1;
        }

        private void FixedUpdate()
        {
            var start = transform.localToWorldMatrix.MultiplyPoint(_box.center - new Vector3(_box.size.x * .5f, 0, 0));
            var end = transform.localToWorldMatrix.MultiplyPoint(_box.center + new Vector3(_box.size.x * .5f, 0, 0));

            float angle;
            RaycastHit hit;

            if (Physics.Linecast(start, end, out hit, _layerMask, QueryTriggerInteraction.Ignore) ||
                Physics.Linecast(end, start, out hit, _layerMask, QueryTriggerInteraction.Ignore))
            {
                if (hit.collider is CapsuleCollider)
                {
                    _lastDetectedCollisionTarget = hit.transform;

                    angle = GetAngle(hit.transform.position, start);

                    var direction = DirectionPointByTheLine(hit.transform.position, start, end);

                    _speed = Mathf.DeltaAngle(-angle - direction * _angleOffset, _startAngle);
                    _speed = Mathf.Clamp(_speed, -90, 90);

                    transform.eulerAngles = new Vector3(0, _startAngle - _speed, 0);

                    _angle = 0;

                    COLLISION.SafeInvoke(hit.collider);
                    return;
                }
            }

            if (null != _lastDetectedCollisionTarget)
            {
                start.y = _lastDetectedCollisionTarget.position.y;

                if (Vector3.Distance(_lastDetectedCollisionTarget.position, start)
                    <= kColliderRadius + _box.size.x)
                    return;
            }

            _angle += _lerpFactor * Time.fixedDeltaTime;
            angle = _startAngle - Mathf.Cos(_angle) * _speed;
            _speed *= _speedFading;

            transform.eulerAngles = new Vector3(0, angle, 0);
        }

        private float DirectionPointByTheLine(Vector3 position, Vector3 start, Vector3 end)
        {
            return Sign((position.x - start.x) * (end.z - start.z) - (position.z - start.z) * (end.x - start.x));
        }
    }
}