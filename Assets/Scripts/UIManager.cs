using TMPro;
using UnityEngine;

public class UIManager : SingletonBehaviour<UIManager>
{
    [SerializeField]
    private TMP_Text _scoreText;

    private int _score;
    public int Score { get => _score; set => UpdateScore(value); }

    private void Start()
    {
        _score = 0;
        _scoreText.text = _score.ToString();
    }

    public void UpdateScore(int newScore)
    {
        _score = newScore;
        _scoreText.text = _score.ToString();
    }
}
