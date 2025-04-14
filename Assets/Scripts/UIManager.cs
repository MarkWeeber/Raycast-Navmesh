using TMPro;
using UnityEngine;

/// <summary>
/// A singleton class to control in game HUD
/// Assigns functions to GameManager to track when score changes and enemy escapes
/// provides public attribute to change ammo count bypassing Gamemanager, since by the terms of refernce the ammo count doesn't affect win or lose events
/// controls various UI panels for in-game, lose, win game states
/// </summary>
public class UIManager : SingletonBehaviour<UIManager>
{
    [SerializeField]
    private float _instructionsShowTime = 4f;
    [SerializeField]
    private TMP_Text _instructionsText;
    [SerializeField]
    private TMP_Text _loseReasonText;
    [SerializeField]
    private TMP_Text _scoreText;
    [SerializeField]
    private TMP_Text _timeLimitText;
    [SerializeField]
    private TMP_Text _ammoCountText;
    [SerializeField]
    private TMP_Text _enemyEscapedCountText;
    [SerializeField]
    private Transform _inGamePanel;
    [SerializeField]
    private Transform _instructionsPanel;
    [SerializeField]
    private Transform _losePanel;
    [SerializeField]
    private Transform _winPanel;

    private int _score;
    private int _enemyEscapedCount;
    private int _ammo;
    public int Ammo { get => _ammo; set => UpdateAmmo(value); }

    private void Start()
    {
        GameManager.Instance.OnGameWin += OnGameWin;
        GameManager.Instance.OnGameLose += OnGameLose;
        GameManager.Instance.OnEnemyKilled += UpdateScore;
        GameManager.Instance.OnEnemyEscaped += UpdateEnemyEscapedCount;
        UpdateScore(0);
        UpdateEnemyEscapedCount(0);
        SetActiveInstructionsUI(true);
        SetActiveInGameUI(false);
        SetActiveLoseUI(false);
        SetActiveWinUI(false);
        Invoke(nameof(HideInstructionsShowInGameUI), _instructionsShowTime);
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnGameWin -= OnGameWin;
        GameManager.Instance.OnGameLose -= OnGameLose;
        GameManager.Instance.OnEnemyKilled -= UpdateScore;
        GameManager.Instance.OnEnemyEscaped -= UpdateEnemyEscapedCount;
    }

    public void UpdateInstructionsText(string newText)
    {
        _instructionsText.text = newText;
    }

    public void UpdateTimeLeft(int secondsLeft)
    {
        _timeLimitText.text = secondsLeft.ToString();
    }

    public void UpdateLoseReasonText(string loseReasonText)
    {
        _loseReasonText.text = loseReasonText;
    }

    private void SetActiveInstructionsUI(bool enabled)
    {
        _instructionsPanel.gameObject.SetActive(enabled);
    }

    private void SetActiveInGameUI(bool enabled)
    {
        _inGamePanel.gameObject.SetActive(enabled);
    }

    private void SetActiveLoseUI(bool enabled)
    {
        _losePanel.gameObject.SetActive(enabled);
    }

    private void SetActiveWinUI(bool enabled)
    {
        _winPanel.gameObject.SetActive(enabled);
    }

    private void UpdateScore(int newScore)
    {
        _score = newScore;
        _scoreText.text = _score.ToString();
    }

    private void UpdateAmmo(int newAmmoCount)
    {
        _ammo = newAmmoCount;
        _ammoCountText.text = _ammo.ToString();
    }

    private void UpdateEnemyEscapedCount(int newEnemyCount)
    {
        _enemyEscapedCount = newEnemyCount;
        _enemyEscapedCountText.text = _enemyEscapedCount.ToString();
    }

    private void HideInstructionsShowInGameUI()
    {
        SetActiveInstructionsUI(false);
        SetActiveInGameUI(true);
    }

    private void OnGameWin()
    {
        SetActiveWinUI(true);
        SetActiveInGameUI(false);
        SetActiveInstructionsUI(false);
        SetActiveLoseUI(false);
    }

    private void OnGameLose()
    {
        SetActiveLoseUI(true);
        SetActiveInGameUI(false);
        SetActiveInstructionsUI(false);
        SetActiveWinUI(false);
    }
}
