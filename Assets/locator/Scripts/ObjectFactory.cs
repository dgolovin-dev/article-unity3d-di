using System;
using UnityEngine;

namespace locator {
  public class ObjectFactory: MonoBehaviour {
    [SerializeField]
    private GameObject[] prefabs;
    private void Awake() {
      foreach (var p in prefabs) {
        Instantiate(p, transform, false);
      }
    }
  }
}