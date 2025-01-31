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
    private string username;
    private bool isUsernameSet = false;
    private bool isBitalinoConnected = false;
    private float minBpm = 200;


    private void Start()
    {
        startButton.interactable = false;
        restartCalibrationButton.SetActive(false);
        usernameInput.onValueChanged.AddListener(delegate { ValueChangeCheck(); });
    }


    private void ValueChangeCheck()
    {
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
            SetBpmText(stress.GetHeartRate());
            yield return new WaitForSeconds(0.3f);
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
}
