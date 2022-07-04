using System;
using System.Runtime.InteropServices.ComTypes;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

namespace context.example {
  public class StarshipMouseController : SceneContextMonoBehaviour {
    [NotEditable][SerializeField][Inject]
    private Starship starship;

    private void Update() {
      if (starship == null) {
        return;
      }
      
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