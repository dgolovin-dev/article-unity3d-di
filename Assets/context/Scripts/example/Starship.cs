using System;
using System.Numerics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Scripting;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace context.example {
   public class Starship : SceneContextMonoBehaviour {
      [SerializeField]
      private int initHp;
      
      [SerializeField]
      private int _hp;
      
      [SerializeField]
      private Vector2 forwardDirection;

      [SerializeField]
      private Rigidbody2D rigidBody;
      
      [SerializeField]
      private float acceleration;

      [SerializeField]
      private Bullet bulletProto;

      [SerializeField]
      public UnityEvent onHpChanged = new UnityEvent();

      [SerializeField][NotEditable]
      public UnityEvent onDeath = new UnityEvent();

      [SerializeField]
      private SpriteRenderer flame;

      [SerializeField]
      private bool accelerated;

      public void Reset() {
         gameObject.SetActive(false);
      }
      
      public void Init(Vector3 pos) {
         gameObject.SetActive(true);
         this.hp = initHp;
         transform.position = pos;
      }

      public void Die(bool fireEvent = true) {
         hp = 0;
         if (fireEvent) {
            onDeath?.Invoke();
         }
         gameObject.SetActive(false);
      }
      public int hp {
         get => _hp;
         set {
            if(_hp == value) return;
            _hp = value;
            onHpChanged?.Invoke();
         }
      }
      
      private Vector2 currentDirection => transform.localToWorldMatrix.MultiplyVector(forwardDirection);

      public void RotateToVector(Vector2 targetDir) {
         var angle = Vector2.SignedAngle(forwardDirection, targetDir);
         transform.eulerAngles = new Vector3(0, 0, angle);
      }
      
      public void Accelerate() {
         var dir = currentDirection;
         rigidBody.velocity += Time.deltaTime * acceleration * dir;
         accelerated = true;
      }
      
      public void Update() {
         var c = flame.color;
         c.a = Mathf.Clamp01(c.a + (accelerated?1:-1) * 5 * Time.deltaTime);
         flame.color = c;
         accelerated = false;
      }

      public void Fire() {
         if(hp <= 0) return;
         var bullet = Instantiate(bulletProto, transform.parent, true);
         bullet.Init(transform.position, currentDirection, rigidBody.velocity);
      }

      public void OnCollisionEnter2D(Collision2D col) {
         var a = col.gameObject.GetComponent<Asteroid>();
         if (a == null) return;
         var oldHp = hp;
         hp -= a.damage;
         if (oldHp > 0 && hp <= 0) {
            Die();
         } 
      }
   }
}
