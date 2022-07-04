namespace context {
  public class ApplicationContextMonoBehaviour : ContextMonoBehaviour<ApplicationContext> {
    protected override void Awake() {
      if (ApplicationContextHolder.context.Find(GetType()) != null) {
        Destroy(this);
        return;
      }
      DontDestroyOnLoad(gameObject);
      base.Awake();
    }

    protected override ApplicationContext context => ApplicationContextHolder.context;
  }
}