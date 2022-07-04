using UnityEngine;

namespace context {
  public class SceneContextInjector : MonoBehaviour {
    public bool add = true;
    public bool inject = true;
    public Object target;
    
    private void Awake() {
      if (target == null) {
        Debug.LogError("target == null", gameObject);
        return;
      }
      var ctx = SceneContextHolder.GetContext(gameObject.scene);
      if (inject) {
        ctx.Inject(target);
      }
      if (add) {
        ctx.Add(target);
      }
    }
  }
}