using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace locator {
  public class WinLevelPanel : MonoBehaviour, IPointerClickHandler {
    [SerializeField] [NotEditable] private GameManager gameManager;

    public void Start() {
      gameManager = GameObject.FindWithTag("gameManager").GetComponentInParent<GameManager>();
      gameManager.onWin.AddListener(Show);
      gameObject.SetActive(false);
    }
    public void Show() {
      gameObject.SetActive(true);
    }

    public void Hide() {
      gameObject.SetActive(false);
      gameManager.StartGame();
    }
    
    public void OnPointerClick(PointerEventData eventData) {
      Hide();
    }
  }
}