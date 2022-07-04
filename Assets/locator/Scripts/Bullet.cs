using System.Security.Cryptography;
using UnityEngine;

namespace locator {
    public class Bullet : MonoBehaviour {
        [SerializeField]
        private float lifetime;
        [SerializeField]
        private float velocity = 5;
        [SerializeField]
        private Rigidbody2D rigidBody;
        [SerializeField]
        private float initTime;
        public void Init(Vector2 position, Vector2 direction, Vector2 shipVelocity) {
            initTime = Time.time;
            transform.position = position;
            gameObject.SetActive(true);
            rigidBody.velocity = shipVelocity + direction * velocity;
        }
        private void Update() {
            if (initTime + lifetime < Time.time) {
                Destroy(gameObject);
            }
        }
        public void OnTriggerEnter2D(Collider2D other) {
            Destroy(gameObject);
        }
    }
}
