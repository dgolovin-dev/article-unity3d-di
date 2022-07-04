using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace locator {
  public class FailLevelPanel : MonoBehaviour, IPointerClickHandler {
    [SerializeField] [NotEditable] private GameManager gameManager;

    public void Start() {
      gameManager = GameObject.FindWithTag("gameManager").GetComponentInParent<GameManager>();
      gameManager.onLose.AddListener(Show);
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