using System;

namespace context {
  [Serializable]
  public class SceneContext : Context {
    public bool ready => initQueue.Count == 0 && injectQueue.Count == 0;

    protected override Context parent => ApplicationContextHolder.context;
  }
}

