using UnityEngine;
using UnityEngine.Events;

namespace direct {
  
  public class GameManager: MonoBehaviour {
    [SerializeField]
    private Starship starship;
    [SerializeField]
    private Vector2 starshipInitPosition;
    [SerializeField]
    private AsteroidManager asteroidManager;
    [SerializeField]
    private Vector3 firstAsteroidInitPosition;
    [SerializeField]
    private UnityEvent onStart;
    [SerializeField]
    private UnityEvent onWin;
    [SerializeField]
    private UnityEvent onLose;

    private void Start() {
      StopGame();
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