using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// A base GameManager singleton class
/// holds main Action delegates for win and lose events
/// has a simple FSM
/// configures win and lose conditions
/// </summary>
public class GameManager : SingletonBehaviour<GameManager>
{
    public enum GameState
    {
        InGame = 0,
        Won = 1,
        Lost = 2
    }

    [Header("Game Rules")]
    [SerializeField]
    private int _timeLimitInSecods = 120;
    [SerializeField]
    private int _enemyKillLimit = 20;
    [SerializeField]
    private int _enemyEscapedLimit = 20;

    private GameState _state;
    public GameState State { get => _state; set => UpdateGameState(value); }
    public Action OnGameWin, OnGameLose; // main delegates for other classes
    public Action<int> OnEnemyKilled, OnEnemyEscaped; // additional delegates for other classes

    private int _score;
    public int Score { get => _score; set => ScoreKill(value); }
    private int _enemiesEscaped;
    public int EnemiesEscaped { get => _enemiesEscaped; set => EnemyEscaped(value); }

    private IEnumerator _countTimeRoutine;

    private void Start()
    {
        UIManager.Instance.UpdateInstructionsText(
            $"Prevent Aliens from escaping\r\n\r\nStop at least {_enemyKillLimit} Alien(s)\r\n\r\nFinish in {_timeLimitInSecods} second(s)"
            );
        Invoke(nameof(StartTimeCount), 4f); // giving some time to instruction text fade away before counting starts
        _countTimeRoutine = UpdateTimeRoutine();
    }

    private void UpdateGameState(GameState newState) // main FSM functions
    {
        _state = newState;
        switch (newState)
        {
            case GameState.Won:
                OnGameWin?.Invoke();
                break;
            case GameState.Lost:
                OnGameLose?.Invoke();
                break;
            default:
                break;
        }
    }

    private void ScoreKill(int totalScore)
    {
        _score = totalScore;
        OnEnemyKilled?.Invoke(totalScore);
        if (_score >= _enemyKillLimit)
        {
            WinByScore();
        }
    }

    private void EnemyEscaped(int totalEscapedCount)
    {
        _enemiesEscaped = totalEscapedCount;
        OnEnemyEscaped?.Invoke(totalEscapedCount);
        if (_enemiesEscaped >= _enemyEscapedLimit)
        {
            LoseByEnemiesEscaped();
        }
    }

    private void StartTimeCount()
    {
        StartCoroutine(_countTimeRoutine);
    }

    IEnumerator UpdateTimeRoutine()
    {
        while(_timeLimitInSecods > 0)
        {
            UIManager.Instance.UpdateTimeLeft(_timeLimitInSecods);
            yield return new WaitForSeconds(1f);
            _timeLimitInSecods--;
        }
        LoseByTimeLimt();
    }

    private void LoseByTimeLimt()
    {
        OnGameLose?.Invoke();
        UIManager.Instance.UpdateLoseReasonText("Time limit reached");
    }

    private void LoseByEnemiesEscaped()
    {
        StopCoroutine(_countTimeRoutine);
        UIManager.Instance.UpdateLoseReasonText("Too much aliens escaped");
        OnGameLose?.Invoke();
    }

    private void WinByScore()
    {
        StopCoroutine(_countTimeRoutine);
        OnGameWin?.Invoke();
    }

    public void RestartLevel() // need to assign UI Buttons' OnClick to this from inspector
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
