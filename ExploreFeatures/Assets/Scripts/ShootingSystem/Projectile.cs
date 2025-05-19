using System;
using UnityEngine;

namespace ShootingSystem
{
    public class Projectile : MonoBehaviour
    {
        public float _speed = 20f;
        public float _damage = 10f;
        public float _lifetime = 5f;
        private Rigidbody _rb;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
        }

        public void Fire(Vector3 direction)
        {
            _rb.velocity = direction*_speed;
            Invoke(nameof(Deactivate), _lifetime);
        }

        void Deactivate() => gameObject.SetActive(false);

        private void OnTriggerEnter(Collider other)
        {
            if(other.CompareTag("player") || other.CompareTag("Enemy"))
            {
                Deactivate();
            }
        }
    }
}
