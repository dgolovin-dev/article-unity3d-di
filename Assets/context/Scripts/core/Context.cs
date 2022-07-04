using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
#if UNITY_EDITOR
#endif


namespace context {
  [Serializable]
  public class Context {
    protected virtual Context parent => null;
    public readonly Dictionary<Type, object> contextObjects = new Dictionary<Type, object>();
    public readonly List<object> injectQueue = new List<object>();
    public readonly List<object> initQueue = new List<object>();
    
    private readonly List<object> iterableCopy = new List<object>();
    
    public void Add<T>(T obj) {
      Add(obj.GetType(), obj);
    }
    
    public void Add(Type t, object obj) {
      contextObjects.Add(t, obj);
      ProcessInjectQueue();
    }

    private void ProcessInjectQueue() {
      iterableCopy.Clear();
      foreach (var o in injectQueue) {
        iterableCopy.Add(o);
      }
      foreach (var o in iterableCopy) {
        if (InjectImmediate(o)) {
          injectQueue.Remove(o);
          initQueue.Add(o);
        }
      }
      iterableCopy.Clear();
      foreach (var o in initQueue) {
        iterableCopy.Add(o);
      }
      foreach (var o in iterableCopy) {
        if (AllDependenciesInjected(o)) {
          Init(o);
          initQueue.Remove(o);
        }
      }
    }

    public void Remove<T>(T obj) {
      contextObjects.Remove(obj.GetType());
    }

    private HashSet<object> dependencies = new HashSet<object>();
    
    private HashSet<object> w = new HashSet<object>();
    private HashSet<object> nw = new HashSet<object>();
    private void CollectDependenciesRecursive(object obj) {
      // uses Wave Front algorithm for dependency collection
      dependencies.Clear();
      w.Clear();
      dependencies.Add(obj);
      w.Add(obj);
      while (w.Count > 0) {
        nw.Clear();
        foreach (var o in w) {
          CollectDependenciesDirect(o, nw);
        }
        w.Clear();
        foreach (var o in nw) {
          if (dependencies.Contains(o)) continue;
          dependencies.Add(o);
          w.Add(o);
        }
      }
    }
    
    private void CollectDependenciesDirect(object obj, HashSet<object> outSet) {
      foreach (var f in obj.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)) {
        var attr = f.GetCustomAttribute<InjectAttribute>();
        if (attr == null) {
          continue;
        }
        var val = f.GetValue(obj);
        if (val != null) {
          outSet.Add(val);
        }
      }
    }

    private bool AllDependenciesInjected(object obj) {
      CollectDependenciesRecursive(obj);
      foreach (var o in dependencies) {
        if (injectQueue.Contains(o) || (parent?.injectQueue.Contains(o) ?? false)) {
          return false;
        }
      }
      return true;
    }
    
    private bool InjectImmediate(object obj){
      foreach (var f in obj.GetType().GetFields(
                 BindingFlags.NonPublic
                 | BindingFlags.Public
                 | BindingFlags.Instance
                 | BindingFlags.FlattenHierarchy)) {
        var attr = f.GetCustomAttribute<InjectAttribute>();
        if (attr == null) {
          continue;
        }
        var val = Find(f.FieldType);
        if (val == null) return false;

        f.SetValue(obj, val);
      }
      return true;
    }

    private void Init(object obj) {
      foreach (var m in obj.GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)) {
        var attr = m.GetCustomAttribute<AfterInjectAttribute>();
        if (attr == null) {
          continue;
        }
        try {
          m.Invoke(obj, Array.Empty<object>());
        }
        catch (Exception ex) {
          Debug.LogError(ex);
        }
      }
    }

    public void Inject<T>(T obj) {
      if (InjectImmediate(obj)) {
        if (AllDependenciesInjected(obj)) {
          Init(obj);
        } else {
          initQueue.Add(obj);
        }
      } else {
        if (!injectQueue.Contains(obj)) {
          injectQueue.Add(obj);
        }
      }
    }
    
    public T Find<T>() where T : class {
      var t = typeof(T);
      return Find(t) as T;
    }
    
    public virtual object Find(Type t) {
      var res = contextObjects.ContainsKey(t) ? contextObjects[t] : null;
      if (res == null && parent != null) {
        res = parent.Find(t);
      }
      return res;
    }
  }
  
}