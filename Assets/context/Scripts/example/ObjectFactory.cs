using System;
using System.Collections;
using UnityEngine;

namespace context.example {
  public class ObjectFactory: MonoBehaviour {
    [SerializeField]
    private GameObject[] prefabs;

    void Start() {
      foreach (var p in prefabs) {
        Instantiate(p, transform, false);
      }
    }
  }
}