using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stress : MonoBehaviour
{
    [SerializeField] private float heartRate;
    public float calmHeartRate = 80;

    public float GetHeartRate()
    {
        return heartRate;
    }

    public void SetCalmHeartRate(float calmHeartRate)
    {
        // TODO: calibrate sensor
        this.calmHeartRate = calmHeartRate;
    }

    public void UpdateHeartRate()
    {
        // TODO fetch heart rate from sensor
    }
    public float StressVariation()
    {
        return heartRate - calmHeartRate;
    }

}
