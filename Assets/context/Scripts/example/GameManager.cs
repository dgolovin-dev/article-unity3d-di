using UnityEngine;
using UnityEngine.Events;

namespace context.example {
  public class GameManager: SceneContextMonoBehaviour {
    [SerializeField]
    private Vector2 starshipInitPosition;
    [SerializeField]
    private Vector2 firstAsteroidInitPosition;

    [Inject][SerializeField][NotEditable]
    private Starship starship;
    [Inject][SerializeField][NotEditable]
    private AsteroidManager asteroidManager;

    [SerializeField][NotEditable]
    public UnityEvent onStart = new UnityEvent();
    [SerializeField][NotEditable]
    public UnityEvent onWin = new UnityEvent();
    [SerializeField][NotEditable]
    public UnityEvent onLose = new UnityEvent();

    [AfterInject]
    private void AfterInject() {
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