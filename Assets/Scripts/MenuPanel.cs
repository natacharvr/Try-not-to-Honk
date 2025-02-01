using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MenuPanel : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private TMP_InputField usernameInput;
    [SerializeField] private TextMeshProUGUI connecting;
    [SerializeField] private TextMeshProUGUI bpmText;
    [SerializeField] private GameObject restartCalibrationButton;
    [SerializeField] private Stress stress;
    [SerializeField] private ScoreBoard scoreBoard;
    [SerializeField] private GameObject title;
    //[SerializeField] private GameObject scoreObject;
    [SerializeField] private TextMeshProUGUI scoreText;
    private string username;
    private bool isUsernameSet = false;
    private bool isBitalinoConnected = false;
    private float minBpm = 200;
    private int savedScore = 0;
    private bool stopCalib = false;

    private void Start()
    {
        startButton.interactable = false;
        restartCalibrationButton.SetActive(false);
        usernameInput.onValueChanged.AddListener(delegate { ValueChangeCheck(); });
        scoreText.gameObject.SetActive(false);
    }


    private void ValueChangeCheck()
    {
        if (usernameInput.text != null) {
            username = usernameInput.text;
            savedScore = scoreBoard.GetScore(username);
            if (savedScore != 0) {
                UpdateUser(savedScore);
            } else
            {
                scoreText.gameObject.SetActive(false);
            }
        }

        if (usernameInput.text.Length > 0 && isBitalinoConnected)
        {
            startButton.interactable = true;
            username = usernameInput.text;
        }
        else
        {
            startButton.interactable = false;
        }
    }

    public void bitalinoConnected()
    {
        connecting.text = "Calibration ... \nPlease don't move";

        StartCoroutine(Calibration());
    }

    public string GetUsername()
    {
        return username;
    }

    public void SetBpmText(float bpm)
    {
        title.SetActive(false);
        if (bpm < minBpm)
        {
            minBpm = bpm;
        }
        bpmText.text = "Your baseline for bpm is :\n" + minBpm.ToString("F0") + "BPM";
    }

    private IEnumerator Calibration()
    {
        isBitalinoConnected = false;
        ValueChangeCheck();
        restartCalibrationButton.SetActive(false);
        yield return new WaitForSeconds(5);
        for (int i = 0; i < 20; i++)
        {
            if (!stopCalib)
            {
                SetBpmText(stress.GetHeartRate());
                yield return new WaitForSeconds(0.3f);
            }
        }
        stress.SetCalmHeartRate(minBpm);
        isBitalinoConnected = true;
        ValueChangeCheck();
        connecting.text = "";
        restartCalibrationButton.SetActive(true);
    }

    public void RestartCalibration()
    {
        minBpm = 200;
        bitalinoConnected();
        stopCalib = false;
    }

    public void NoDevice()
    {
        isBitalinoConnected = !isBitalinoConnected;
        if (isBitalinoConnected)
        {
            connecting.text = "";
            BitalinoScript.instance.connect = false;
        }
        else
        {
            connecting.text = "Connecting ...";
            BitalinoScript.instance.connect = true;

        }
        ValueChangeCheck();
    }

    private void UpdateUser(int score)
    {
        scoreText.gameObject.SetActive(true);
        scoreText.text = username + " saved score : " + score;
    }

    public void useValueSaved()
    {
        stopCalib = true;
        minBpm = savedScore;
        SetBpmText(savedScore);
        stress.SetCalmHeartRate(minBpm);
    }
}
