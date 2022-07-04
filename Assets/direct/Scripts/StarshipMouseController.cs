using UnityEngine;


namespace direct {
  public class StarshipMouseController : MonoBehaviour {
    [SerializeField]
    private Starship starship;

    private bool accelerate = false;
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