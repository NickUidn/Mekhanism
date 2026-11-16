using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;

public class ContractVis : MonoBehaviour
{
    public ContractClass contract;
    public LineRenderer myLineRenderer;
    public ContractSegment draggerPasser;
    // Track old values to see if something actually changed
    private float lastAmplitude;
    private float lastFrequency;
    private float lastLength;

    static float Amp_MAX_VALUE = 6f;
    static float Amp_MIN_VALUE => -Amp_MAX_VALUE;
    static float Freq_MIN_VALUE = 0f;
    static float Freq_MAX_VALUE = 3f;

    private float _amplitude;
    public float amplitude
    {
        get => _amplitude;
        set
        {
            float clampedValue = Mathf.Clamp(value, Amp_MIN_VALUE, Amp_MAX_VALUE);

            if (contract != null && contract.isOdd)
            {
                if (draggerPasser.positionInSequance % 2 != 0)
                {
                   
                    _amplitude = -Mathf.Abs(clampedValue);
                }
                else
                {
                  
                    _amplitude = Mathf.Abs(clampedValue);
                }

            }
            else if (contract != null && contract.isOdd==false)
            {
                if (draggerPasser.positionInSequance % 2 != 0)
                {
                  
                    _amplitude = Mathf.Abs(clampedValue);
                }
                else
                {
                   
                    _amplitude = -Mathf.Abs(clampedValue);
                }
            }

            }
    }
    private float _frequency;
    public float frequency
    {
        get => _frequency;
        set
        {
            _frequency = Mathf.Clamp(value, Freq_MIN_VALUE, Freq_MAX_VALUE);
         
        }
    }
    public float length = 5f;
    void Awake()
    {
        if (myLineRenderer == null)
            
            myLineRenderer = GetComponent<LineRenderer>();
    }

    void Start()
    {
        //contract=gameObject.GetComponentInParent<ContractClass>();
        //// Draw it once at the beginning
        //DrawStaticWave();
        //SaveCurrentValues();
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
    public void SetAmplitude(int amplitudeValue)
    {
      
        float mapped = MapAmplitude(amplitudeValue);       
            amplitude = mapped;
            return;
        }  

    public void SetFrequency(int frequencyValue)
    {
        float mapped = MapFrequency(frequencyValue);       
            frequency = mapped;
              }

    private static float MaxAmplitudeMagnitude => Mathf.Abs(Amp_MAX_VALUE);
    private static float MapAmplitude(int value)
    {
        // Magnitude only (0..Amp_MAX_VALUE). Sign is applied by the setter via ticIndex parity.
        return Mathf.Lerp(0f, MaxAmplitudeMagnitude, NormalizeInput(value));
    }
    private static float MapFrequency(int value)
    {
        return Mathf.Lerp(Freq_MIN_VALUE, Freq_MAX_VALUE, NormalizeInput(value));
    }
    public const int INPUT_MIN = 1;
    public const int INPUT_MAX = 100;
    private static float NormalizeInput(int value)
    {
        int v = Mathf.Clamp(value, INPUT_MIN, INPUT_MAX);
        return (v - INPUT_MIN) / (float)(INPUT_MAX - INPUT_MIN); // 0..1
    }
    void SaveCurrentValues()
    {
        lastAmplitude = amplitude;
        lastFrequency = frequency;
        lastLength = length;
    }

   public void DrawStaticWave()
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