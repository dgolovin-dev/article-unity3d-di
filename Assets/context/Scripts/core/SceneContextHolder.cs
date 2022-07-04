using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace context {
  public class SceneContextHolder:MonoBehaviour {
    public SceneContext context = new SceneContext();

    private void Awake() {
      gameObject.hideFlags = HideFlags.DontSave;
    }

    private void OnDestroy() {
      contexts.Remove(gameObject.scene.handle);
    }
      
    private static Dictionary<int, SceneContext> contexts = new Dictionary<int, SceneContext>();
    
    public static SceneContext GetContext(Scene scene) {
      if (!contexts.ContainsKey(scene.handle)) {
        var go = new GameObject("SceneContext");
        SceneManager.MoveGameObjectToScene(go, scene);
        var holder = go.AddComponent<SceneContextHolder>();
        contexts.Add(scene.handle, holder.context);
      } 
      return contexts[scene.handle];
    }

/*   IEnumerator Start() {
      while (true) {
        yield return new WaitForSeconds(5);
        foreach (var obj in context.injectQueue) {
          Debug.Log("unresolved obj: " + obj.GetType().Name +  " "  + obj);
        }
        Debug.Log("---");
      }
    }
*/
  }
}