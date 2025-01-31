using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    private float MaxTime;
    private float timer;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI bpmText;
    private float life;
    public Image greenLife;
    private float maxLife = 100;

    public void Initialize(float maxTime)
    {
        MaxTime = maxTime;
        life = maxLife;
        timer = MaxTime;
        SetTimeText();
    }

    void SetTimeText()
    {
        timerText.text = timer.ToString("F0") + "S LEFT";
    }

    void Update()
    {
        timer -= Time.deltaTime;
        SetTimeText();
        SetLifeBar();
    }

    public bool Loose()
    {
        return (timer <= 0 || life <= 0);
    }

    public void Hit(int damage)
    {
        life -= damage;
    }

    public void Heal(int heal)
    {
        life += heal;
    }

    private void SetLifeBar()
    {
        greenLife.fillAmount = life / maxLife;
    }

    public void SetBpmText(float bpm)
    {
        bpmText.text = bpm.ToString("F0") + "BPM";
    }

}
