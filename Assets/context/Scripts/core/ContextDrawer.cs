#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace context {
  [CustomPropertyDrawer(typeof(Context))]
  [CustomPropertyDrawer(typeof(SceneContext))]
  [CustomPropertyDrawer(typeof(ApplicationContext))]
  public class ContextDrawer : PropertyDrawer
  {
    const int lineHeight = 25;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
      Context obj = fieldInfo.GetValue(property.serializedObject.targetObject) as Context;
      int i = 0;

      if (obj.injectQueue.Count > 0) {
        EditorGUI.LabelField(GetLineRect(position, i++), "injectQueue");
        foreach (var uo in obj.injectQueue) {
          if (uo is Object) {
            var unityObject = uo as Object;
            EditorGUI.ObjectField(GetLineRect(position, i++), unityObject, unityObject.GetType(), true);
          } else {
            EditorGUI.TextField(GetLineRect(position, i++), uo.ToString());
          }
        }
      }
      if (obj.initQueue.Count > 0) {
        EditorGUI.LabelField(GetLineRect(position, i++), "initQueue");
        foreach (var uo in obj.initQueue) {
          if (uo is Object) {
            var unityObject = uo as Object;
            EditorGUI.ObjectField(GetLineRect(position, i++), unityObject, unityObject.GetType(), true);
          } else {
            EditorGUI.TextField(GetLineRect(position, i++), uo.ToString());
          }
        }
      }
      if (obj.contextObjects.Count > 0) {
        EditorGUI.LabelField(GetLineRect(position, i++), "contextObjects");
        foreach (var uo in obj.contextObjects.Values) {
          if (uo is Object) {
            var unityObject = uo as Object;
            EditorGUI.ObjectField(GetLineRect(position, i++), unityObject, unityObject.GetType(), true);
          } else {
            EditorGUI.TextField(GetLineRect(position, i++), uo.ToString());
          }
        }
      }
    }

    private Rect GetLineRect(Rect pos, int number) {
      return new Rect(pos.x, pos.y + lineHeight * number, pos.width, lineHeight);
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
      Context obj = fieldInfo.GetValue(property.serializedObject.targetObject) as Context;
      return (obj.injectQueue.Count + obj.contextObjects.Count + 2) * lineHeight;
    }
  }
}
#endif