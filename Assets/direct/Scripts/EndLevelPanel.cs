using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace direct {
  public class EndLevelPanel : MonoBehaviour, IPointerClickHandler {
    [SerializeField]
    private UnityEvent onClose;

    public void Show() {
      gameObject.SetActive(true);
    }

    public void Hide() {
      gameObject.SetActive(false);
      onClose?.Invoke();
    }
    
    public void OnPointerClick(PointerEventData eventData) {
      Hide();
    }
  }
}