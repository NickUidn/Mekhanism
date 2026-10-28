using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticWave : MonoBehaviour
{
    public LineRenderer myLineRenderer;
    public float amplitude = 1;
    public float frequency = 1;
    public float length = 10;

    // Track old values to see if something actually changed
    private float lastAmplitude;
    private float lastFrequency;
    private float lastLength;

    void Awake()
    {
        if (myLineRenderer == null)
            myLineRenderer = GetComponent<LineRenderer>();
    }

    void Start()
    {
        // Draw it once at the beginning
        DrawStaticWave();
        SaveCurrentValues();
    }

    private void Update()
    {
        // ONLY redraw if you actively tweak the sliders in the inspector during runtime
        if (amplitude != lastAmplitude || frequency != lastFrequency || length != lastLength)
        {
            DrawStaticWave();
            SaveCurrentValues();
        }
    }

    void SaveCurrentValues()
    {
        lastAmplitude = amplitude;
        lastFrequency = frequency;
        lastLength = length;
    }

    void DrawStaticWave()
    {
        float xStart = 0;
        float xFinish = length;
        float totalWidth = length;

        // Calculate the period based on frequency
        float period = totalWidth / frequency;

        List<Vector3> peakPositions = new List<Vector3>();

        // Add start point at y = 0
        peakPositions.Add(new Vector3(xStart, 0, 0));

        // Generate peaks for each cycle
        for (int cycle = 0; cycle < frequency; cycle++)
        {
            float cycleStartX = xStart + (cycle * period);

            // Zero crossing in the middle of each freq motion
            // (between the previous cycle's negative peak and this cycle's positive peak)
            if (cycleStartX > xStart && cycleStartX < xFinish)
            {
                peakPositions.Add(new Vector3(cycleStartX, 0, 0));
            }

            // Positive peak (at 1/4 of the period from the start of each cycle)
            float positivePeakX = cycleStartX + (period / 4f);
            if (positivePeakX > xStart && positivePeakX < xFinish)
            {
                peakPositions.Add(new Vector3(positivePeakX, amplitude, 0));
            }

            // Zero crossing (at 1/2 of the period)
            float zeroCrossingX = cycleStartX + (period / 2f);
            if (zeroCrossingX > xStart && zeroCrossingX < xFinish)
            {
                peakPositions.Add(new Vector3(zeroCrossingX, 0, 0));
            }

            // Negative peak (at 3/4 of the period from the start of each cycle)
            float negativePeakX = cycleStartX + (3f * period / 4f);
            if (negativePeakX > xStart && negativePeakX < xFinish)
            {
                peakPositions.Add(new Vector3(negativePeakX, -amplitude, 0));
            }
        }

        // Add end point at y = 0
        peakPositions.Add(new Vector3(xFinish, 0, 0));

        // Sort peaks by x position
        peakPositions.Sort((a, b) => a.x.CompareTo(b.x));

        // Set the line renderer points
        myLineRenderer.positionCount = peakPositions.Count;

        for (int i = 0; i < peakPositions.Count; i++)
        {
            myLineRenderer.SetPosition(i, peakPositions[i]);
        }
    }
}