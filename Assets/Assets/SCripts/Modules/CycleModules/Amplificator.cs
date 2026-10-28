using System;
using UnityEngine;

public class Amplificator : ModuleClassCycle
{
    [Header("Module Controlls")]
    public int affectedModule;

    public int amplificatorValue;
    Tic affectedTic;

    public override void DoModuelCycle()
    {
        affectedTic = cycleMain.tics[affectedModule];

        int nextPotentialAmp = 0;
        int previousPotentialAmp = 0;

        int newAffectedAmp = affectedTic.GetNormalizedAmplitude() + amplificatorValue;

        if (affectedTic.nextTic != null)
        {
            nextPotentialAmp = Mathf.RoundToInt(
                affectedTic.nextTic.GetNormalizedAmplitude() + amplificatorValue * 0.5f);
        }

        if (affectedTic.previousTic != null)
        {
            previousPotentialAmp = Mathf.RoundToInt(
                affectedTic.previousTic.GetNormalizedAmplitude() + amplificatorValue * 0.5f);
        }

        // --- Next tic ---
        if (affectedTic.nextTic != null)
        {
            if (newAffectedAmp >= nextPotentialAmp)
            {
                affectedTic.nextTic.SetAmplitude(nextPotentialAmp);
            }
            else if (affectedTic.GetNormalizedAmplitude() > affectedTic.nextTic.GetNormalizedAmplitude())
            {
                affectedTic.nextTic.SetAmplitude(newAffectedAmp);
            }
        }

        // --- Previous tic ---
        if (affectedTic.previousTic != null)
        {
            if (newAffectedAmp >= previousPotentialAmp)
            {
                affectedTic.previousTic.SetAmplitude(previousPotentialAmp);
            }
            else if (affectedTic.GetNormalizedAmplitude() > affectedTic.previousTic.GetNormalizedAmplitude())
            {
                affectedTic.previousTic.SetAmplitude(newAffectedAmp);
            }
        }

        affectedTic.SetAmplitude(newAffectedAmp);
    }
}