using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;


public class EcgUI : MonoBehaviour
{
    // Sampling frequency in Hz
    private const float SamplingFrequency = 100f;

    // Filter settings
    public float HighPassCutoff = 0.5f;
    public float LowPassCutoff = 40f;

    // Visualization settings
    public int MaxPoints = 500;
    public float Scale = 0.005f;

    // UI Components
    public RectTransform graphContainer;
    public GameObject pointPrefab; // Prefab for UI points (Image or UI Element)
    private List<GameObject> points = new List<GameObject>();
    private Queue<float> signalPoints = new Queue<float>();
    private Queue<float> rawSignalPoints = new Queue<float>();

    // HeartRate
    private float heartRate = 0f;
    
    public float GetHeartRate()
    {
        return heartRate;
    }

    void Start()
    {
        StartCoroutine(ProcessECGSignal());
    }

    private IEnumerator ProcessECGSignal()
    {
        while (true)
        {
            for (int i = 0; i < 10; i++) // Process multiple samples per frame
            {
                float rawECGSample = GetRawECGSample();
                if (rawECGSample == 0) continue;

                float filteredSample = FilterSignal(rawECGSample);
                AddSignalPoint(filteredSample);
            }
            UpdateGraph();
            yield return new WaitForSeconds(1 / SamplingFrequency);
        }
    }

    private float GetRawECGSample()
    {
        return rawSignalPoints.Count > 0 ? rawSignalPoints.Dequeue() : 0;
    }

    private float FilterSignal(float ecgSample)
    {
        float highPassed = HighPassFilter(ecgSample);
        return LowPassFilter(highPassed);
    }

    private Queue<float> inputBuffer = new Queue<float>();
    private Queue<float> highPassBuffer = new Queue<float>();
    private Queue<float> lowPassBuffer = new Queue<float>();

    private float HighPassFilter(float sample)
    {
        float RC = 1.0f / (2 * Mathf.PI * HighPassCutoff);
        float alpha = RC / (RC + 1.0f / SamplingFrequency);

        if (highPassBuffer.Count > 1)
            highPassBuffer.Dequeue();

        float previousSample = highPassBuffer.Count > 0 ? highPassBuffer.Peek() : 0;
        float highPassValue = alpha * (previousSample + sample - (inputBuffer.Count > 0 ? inputBuffer.Peek() : 0));
        highPassBuffer.Enqueue(highPassValue);

        inputBuffer.Enqueue(sample);
        if (inputBuffer.Count > 2)
            inputBuffer.Dequeue();

        return highPassValue;
    }

    private float LowPassFilter(float sample)
    {
        float RC = 1.0f / (2 * Mathf.PI * LowPassCutoff);
        float alpha = 1.0f / (RC * SamplingFrequency + 1);

        if (lowPassBuffer.Count > 1)
            lowPassBuffer.Dequeue();

        float previousSample = lowPassBuffer.Count > 0 ? lowPassBuffer.Peek() : 0;
        float lowPassValue = alpha * (sample + previousSample);
        lowPassBuffer.Enqueue(lowPassValue);

        return lowPassValue;
    }


    private void AddSignalPoint(float sample)
    {
        // Scale and add to queue
        signalPoints.Enqueue(sample * Scale);

        // Remove oldest point if exceeding MaxPoints
        if (signalPoints.Count > MaxPoints)
        {
            signalPoints.Dequeue();
        }
    }


    public void AddRawSignalPoint(float sample)
    {
        rawSignalPoints.Enqueue(sample);
    }

    private void UpdateGraph()
    {
        float graphWidth = graphContainer.rect.width / 2;
        float graphHeight = graphContainer.rect.height;
        float xStep = graphWidth / MaxPoints;

        // Ensure we only have MaxPoints elements
        while (points.Count < signalPoints.Count)
        {
            GameObject newPoint = Instantiate(pointPrefab, graphContainer);
            points.Add(newPoint);
        }

        // Move existing points instead of recreating them
        int i = 0;
        foreach (var sample in signalPoints)
        {
            RectTransform pointTransform = points[i].GetComponent<RectTransform>();
            pointTransform.anchoredPosition = new Vector2(i * xStep, sample * graphHeight);
            i++;
        }

        // Remove excess points from the list
        while (points.Count > MaxPoints)
        {
            Destroy(points[0]);
            points.RemoveAt(0);
        }
        DetectHeartbeats();
    }

    private List<float> peakTimestamps = new List<float>();
    private float lastPeakTime = 0f;
    private float threshold = 0.01f; // Adjust based on signal

    private void DetectHeartbeats()
    {
        if (signalPoints.Count < 3) return; // Need at least 3 points to detect peaks

        float[] signalArray = signalPoints.ToArray();
        float avSignal = signalArray.Average();
        threshold = (avSignal >0) ? avSignal * 4 : -avSignal * 4;
        int lastIndex = signalArray.Length - 2; // Avoid out-of-bounds access

        // Peak detection (simple threshold & local max)
        if (signalArray[lastIndex] >= signalArray[lastIndex - 1] &&
            signalArray[lastIndex] >= signalArray[lastIndex + 1] &&
            signalArray[lastIndex] >= threshold)
        {
            float currentTime = Time.time;

            // Ensure at least 300ms (200 BPM max) between beats to avoid false positives
            if (currentTime - lastPeakTime > 0.3f)
            {
                //Debug.Log("Peak detected at: " + currentTime);
                peakTimestamps.Add(currentTime);
                lastPeakTime = currentTime;
                CalculateHeartRate();
            }
        }
    }


    private void CalculateHeartRate()
    {
        if (peakTimestamps.Count < 2) return; // Need at least 2 peaks

        // Get RR Intervals (time differences between consecutive peaks)
        List<float> rrIntervals = new List<float>();
        for (int i = 1; i < peakTimestamps.Count; i++)
        {
            rrIntervals.Add(peakTimestamps[i] - peakTimestamps[i - 1]);
        }

        // Compute the average RR interval
        float averageRR = rrIntervals.Average();

        // Convert to BPM (60 / avg RR interval in seconds)
        heartRate = 60f / averageRR;

        // Keep only the last few peaks to maintain real-time updates
        if (peakTimestamps.Count > 15)
        {
            peakTimestamps.RemoveAt(0);
        }

        //Debug.Log("Heart Rate: " + heartRate + " BPM");
    }



}
