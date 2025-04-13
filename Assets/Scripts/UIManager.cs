using TMPro;
using UnityEngine;

/// <summary>
/// A singleton class to control in game HUD
/// holds public attributes to set score, escaped enemies, ammo count, remaining time
/// </summary>
public class UIManager : SingletonBehaviour<UIManager>
{
    [SerializeField]
    private TMP_Text _scoreText;
    [SerializeField]
    private TMP_Text _ammoCountText;
    [SerializeField]
    private TMP_Text _enemyEscapedCountText;

    private int _score;
    public int Score { get => _score; set => UpdateScore(value); }
    private int _ammo;
    public int Ammo { get => _ammo; set => UpdateAmmo(value); }
    private int _enemyEscapedCount;
    public int EnemyEscapedCount { get => _enemyEscapedCount; set => UpdateEnemyEscapedCount(value); }

    private void Start()
    {
        _score = 0;
        _scoreText.text = _score.ToString();
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
}
