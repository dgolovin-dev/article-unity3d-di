namespace context {
  public class SceneContextMonoBehaviour : ContextMonoBehaviour<SceneContext> {
    protected override SceneContext context => SceneContextHolder.GetContext(gameObject.scene);
  }
}