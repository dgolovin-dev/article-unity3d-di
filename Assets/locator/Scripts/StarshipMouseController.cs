using System;
using System.Runtime.InteropServices.ComTypes;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

namespace locator {
  public class StarshipMouseController : MonoBehaviour {
    [SerializeField][NotEditable]
    private Starship starship;

    private bool accelerate = false;

    private void Start() {
      starship = GameObject.FindWithTag("starship").GetComponentInParent<Starship>();
    }

    private void Update() {
      RotateToMouse();

      if (Input.GetMouseButtonDown(0)) {
        starship.Fire();
      }

      if (Input.GetMouseButton(1)) {
        starship.Accelerate();
      }
    }

    private void RotateToMouse() {
      var mousePos = (Vector2) Camera.main.ScreenToWorldPoint(Input.mousePosition);
      starship.RotateToVector(mousePos - (Vector2) starship.transform.position);
    }
    
  }
}