using UnityEngine;

namespace context {
  public class ApplicationContextHolder: MonoBehaviour {
    public ApplicationContext ctx = new ApplicationContext();
    
    private void Awake() {
      gameObject.hideFlags = HideFlags.DontSave;
    }
    
    private static ApplicationContextHolder _instance;

    public static ApplicationContextHolder instance {
      get {
        if (_instance == null) {
          var go = new GameObject("AppContext");
          go.hideFlags = HideFlags.DontSave;
          DontDestroyOnLoad(go);
          var holder = go.AddComponent<ApplicationContextHolder>();
          _instance = holder;
        }
        return _instance;
      }
    }

    public static ApplicationContext context => instance.ctx;
  }
}