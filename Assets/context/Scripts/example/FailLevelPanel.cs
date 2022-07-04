using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace context.example {
  public class FailLevelPanel : SceneContextMonoBehaviour, IPointerClickHandler {
    [Inject] [NotEditable] 
    private GameManager gameManager;

    [AfterInject]
    private void AfterInject() {
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