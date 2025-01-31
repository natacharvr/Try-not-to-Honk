using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreBoard : MonoBehaviour
{
    [SerializeField] StressManager stressManager;
    private string userName;
    private int maxStress = int.MinValue;
    private int minStress = int.MaxValue;

    private int previousMaxStress;
    private int previousMinStress;
    private int previousAvStress;

    private int valCount = 0;
    private int accumulatedBpm = 0;

    [SerializeField] private TMP_Text maxStressText;
    [SerializeField] private TMP_Text minStressText;
    [SerializeField] private TMP_Text avgStressText;

    public void BeginGame(string username)
    {
        userName = username;
        LoadStressUser(username);
    }

    public void SaveStressUser()
    {
        UpdateUIText();
        PlayerPrefs.SetInt(userName + "MaxStress", maxStress);
        PlayerPrefs.SetInt(userName + "MinStress", minStress);
        PlayerPrefs.SetInt(userName + "AvStress", GetAverageStress());

        previousAvStress = GetAverageStress();
        previousMaxStress = maxStress;
        previousMinStress = minStress;
    }

    public void LoadStressUser(string user)
    {
        userName = user;
        previousMaxStress = PlayerPrefs.GetInt(user + "MaxStress", 0);
        previousMinStress = PlayerPrefs.GetInt(user + "MinStress", 0);
        previousAvStress = PlayerPrefs.GetInt(user + "AvStress", 0);
    }

    public void GiveHeartRate(float heartRate)
    {
        int heartRateInt = Mathf.FloorToInt(heartRate); 

        if (heartRateInt > maxStress)
        {
            maxStress = heartRateInt;
        }
        if (heartRateInt < minStress)
        {
            minStress = heartRateInt;
        }

        accumulatedBpm += heartRateInt;
        valCount++;
    }

    public int GetAverageStress()
    {
        return (valCount > 0) ? accumulatedBpm / valCount : 0; 
    }

    private void UpdateUIText()
    {
        maxStressText.text = $"Max bpm : {previousMaxStress} -> {maxStress}";
        minStressText.text = $"Min bpm : {previousMinStress} -> {minStress}";
        avgStressText.text = $"Avg bpm : {previousAvStress} -> {GetAverageStress()}";
    }


}
