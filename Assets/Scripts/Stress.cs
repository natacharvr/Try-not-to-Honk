using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Stress : MonoBehaviour
{
    private float heartRate;
    private Queue<float> previousHeartRates; // 20 previous heart rates to calculate the average
    public EcgUI ecgUI;
    private float calmHeartRate = 80;
    
    [SerializeField] private GameUI gameUI;
    [SerializeField] private ScoreBoard scoreBoard;

    private void Start()
    {
        previousHeartRates = new Queue<float>();
        heartRate = 80;
        StartCoroutine(UpdateHeartRate());
    }

    public float GetHeartRate()
    {
        return heartRate;
    }

    public void SetCalmHeartRate(float calmHeartRate)
    {
        this.calmHeartRate = calmHeartRate;
    }

    private IEnumerator UpdateHeartRate()
    {   
        while (true)
        {
            previousHeartRates.Enqueue(heartRate);
            if (previousHeartRates.Count > 20)
            {
                previousHeartRates.Dequeue();
            }
            heartRate = ecgUI.GetHeartRate();
            scoreBoard.GiveHeartRate(heartRate);
            //Debug.Log(heartRate);
            gameUI.SetBpmText(heartRate);
            yield return new WaitForSeconds(1);
        }
    }

    public float StressVariationAbsolute()
    {
        return heartRate - calmHeartRate;
    }

    public float StressVariationTendancy()
    {
        return heartRate - previousHeartRates.Average();
    }

}
