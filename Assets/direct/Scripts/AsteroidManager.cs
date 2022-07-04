using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using System.Linq;

namespace direct {
  public class AsteroidManager : MonoBehaviour {
    [SerializeField]
    private List<Asteroid> asteroids;
    
    [SerializeField]
    public UnityEvent onAllDead;
    private void Awake() {
      foreach (var a in asteroids) {
        a.onDeath.AddListener(OnAsteroidDead);
      }
    }

    public void Reset() {
      foreach (var a in asteroids) {
        a.Die(false);
      }
    }
    
    public void Init(Vector3 initPosition) {
      Reset();
      asteroids.OrderBy(a=>a.level)
        .First()
        .Init(initPosition, Vector3.zero);
    }

    private void OnAsteroidDead(Asteroid asteroid, Vector3 hitVector) {
      InitNextAsteroids(asteroid, hitVector);
      CheckAllAsteroidsDead();
    }

    private void InitNextAsteroids(Asteroid asteroid, Vector3 hitVector) {
      var currentLevelAsteroids = asteroids.Where(i => i.level == asteroid.level)
        .ToList();
      var nextLevelAsteroids = asteroids.Where(i => i.level == asteroid.level + 1)
        .ToList();
      var spawnCount = nextLevelAsteroids.Count / currentLevelAsteroids.Count;
      var index = currentLevelAsteroids.IndexOf(asteroid);
      var ni = 0;
      foreach (var nextAsteroid in nextLevelAsteroids.Skip(index*spawnCount).Take(spawnCount)) {
        Vector2 dir = Quaternion.AngleAxis(asteroid.nextAngles[ni], Vector3.forward) * hitVector.normalized;
        nextAsteroid.Init(
          asteroid.transform.position + (Vector3)dir * asteroid.nextOffset, 
          dir * asteroid.nextVelocity
        );
        ni++;
      }
    }

    private void CheckAllAsteroidsDead() {
      var allDead = true;
      foreach (var a in asteroids) {
        if (a.isActiveAndEnabled) {
          allDead = false;
        }
      }
      if (allDead) {
        onAllDead.Invoke();
      }
    }
  }
}