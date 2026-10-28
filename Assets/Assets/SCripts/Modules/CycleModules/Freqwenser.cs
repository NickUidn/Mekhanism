using UnityEngine;

public class Freqwenser : ModuleClassCycle
{
    [Header("Module Controlls")]
    public int affectedModule;
    public int FreqwenserValue;
    public bool isGoingBackwards;
    Tic affectedTic;


    public override void DoModuelCycle()
    {
        affectedTic = cycleMain.tics[affectedModule];

        int newMainFreq = affectedTic.GetNormalizedFrequency() + FreqwenserValue;
        affectedTic.SetFrequency(newMainFreq);
        if (!isGoingBackwards)
        {
            if (affectedTic.nextTic != null)
            {
                affectedTic.nextTic.SetFrequency(Mathf.RoundToInt(affectedTic.nextTic.GetNormalizedFrequency() + FreqwenserValue * 0.5f));
            }
            if (affectedTic.nextTic.nextTic != null)
            {
                affectedTic.nextTic.nextTic.SetFrequency(Mathf.RoundToInt(affectedTic.nextTic.nextTic.GetNormalizedFrequency() + FreqwenserValue * 0.3f));
            }

        }
        if (isGoingBackwards)
        {
            if (affectedTic.previousTic != null)
            {
                affectedTic.previousTic.SetFrequency(Mathf.RoundToInt(affectedTic.previousTic.GetNormalizedFrequency() + FreqwenserValue * 0.5f));
            }
            if (affectedTic.previousTic.previousTic != null)
            {
                affectedTic.previousTic.previousTic.SetFrequency(Mathf.RoundToInt(affectedTic.previousTic.previousTic.GetNormalizedFrequency() + FreqwenserValue * 0.3f));
            }
        }
    }
}

