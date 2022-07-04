using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace context.example {
    public class Asteroid : SceneContextMonoBehaviour {
        protected override bool expose => false;

        public int level;
        public int damage;
        [SerializeField]
        private new Rigidbody2D rigidbody;
        [SerializeField]
        public float[] nextAngles;
        [SerializeField]
        public float nextOffset;
        [SerializeField]
        public float nextVelocity;
        public event Action<Asteroid, Vector3> onDeath;

        [Inject][NotEditable]
        private AsteroidManager asteroidManager; 

        [AfterInject]
        public void AfterInject() {
            asteroidManager.Register(this);
        }
        
        public void OnTriggerEnter2D(Collider2D other) { // Bullet hit
            var hitVector = transform.position - other.transform.position;
            Die(true, hitVector);
        }
        public void Init(Vector3 pos, Vector3 velocity) {
            gameObject.SetActive(true);
            transform.position = pos;
            transform.eulerAngles = new Vector3(0, 0, Random.Range(0f, 360f));
            rigidbody.velocity = velocity;
        }
        public void Die(bool fireEvent = false, Vector3 hitVector = default(Vector3)) {
            gameObject.SetActive(false);
            if (fireEvent) {
                onDeath?.Invoke(this, hitVector);
            }
        }
    }
}
