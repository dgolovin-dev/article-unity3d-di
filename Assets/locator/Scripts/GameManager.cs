using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.PlayerLoop;

namespace locator {
  public class GameManager: MonoBehaviour {
    [SerializeField]
    private Vector2 starshipInitPosition;
    [SerializeField]
    private Vector2 firstAsteroidInitPosition;

    [SerializeField][NotEditable]
    private Starship starship;
    [SerializeField][NotEditable]
    private AsteroidManager asteroidManager;

    [SerializeField][NotEditable]
    public UnityEvent onStart = new UnityEvent();
    [SerializeField][NotEditable]
    public UnityEvent onWin = new UnityEvent();
    [SerializeField][NotEditable]
    public UnityEvent onLose = new UnityEvent();

    private void Start() {
      starship = GameObject.FindGameObjectWithTag("starship").GetComponent<Starship>();
      asteroidManager = GameObject.FindGameObjectWithTag("asteroidManager").GetComponent<AsteroidManager>();
      starship.onDeath.AddListener(LoseGame);
      asteroidManager.onAllDead.AddListener(WinGame);
      StartGame();
    }
    public void StartGame() {
      starship.Init(starshipInitPosition);
      asteroidManager.Init(firstAsteroidInitPosition);
      onStart?.Invoke();
    }
    public void WinGame() {
      StopGame();
      onWin?.Invoke();
    }
    public void LoseGame() {
      StopGame();
      onLose?.Invoke();
    }
    private void StopGame() {
      starship.Die(false);
      asteroidManager.Reset();
    }
  }
}