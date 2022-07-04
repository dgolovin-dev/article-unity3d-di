using UnityEngine;

namespace context {
  public abstract class ContextMonoBehaviour<C> : MonoBehaviour where C:Context {
    protected virtual bool expose => true;
    protected virtual bool inject => true;
    
    protected abstract C context { get; }

    protected virtual void Awake() {
      if(!Application.isPlaying) return;
      if (inject) {
        context.Inject(this);
      }
      if (expose) {
        context.Add(this);
      }
    }
  }
}