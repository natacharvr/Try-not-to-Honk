using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stress : MonoBehaviour
{
    private float heartRate;
    private float previousHeartRate;
    public EcgUI ecgUI;
    public float calmHeartRate = 80;
    private float spiderPeak = 0;
    private float snakePeak = 0;
    [SerializeField] private GameUI gameUI;

    private void Start()
    {
        StartCoroutine(UpdateHeartRate());
    }

    public float GetHeartRate()
    {
        return heartRate;
    }

    public void SetCalmHeartRate(float calmHeartRate)
    {
        // TODO: calibrate sensor
        this.calmHeartRate = calmHeartRate;
    }

    private IEnumerator UpdateHeartRate()
    {   
        while (true)
        {
            previousHeartRate = heartRate;
            heartRate = ecgUI.GetHeartRate();
            gameUI.SetBpmText(heartRate);
            yield return new WaitForSeconds(1);
        }
    }

    public float StressVariationAbsolute()
    {
        return heartRate - calmHeartRate;
    }

    public float StressVariationInstant()
    {
        return heartRate - previousHeartRate;
    }

}
