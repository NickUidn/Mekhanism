using UnityEngine;

public class GeneratorAMP: ModuleClassCycle
{
    [Header("Module Controlls")]
    private int _affectedModule;
    public int affectedModule
    {
        get => _affectedModule;
        set
        {
            _affectedModule = Mathf.Clamp(value, 1, 100);
        }
    }
    private int _ampGenerationValue;
    public int ampGenerationValue
    {
        get => _ampGenerationValue;
        set
        {
            _ampGenerationValue = Mathf.Clamp(value, 1, 100);
        }
    }


    public override void DoModuelCycle()
    {
        
        Debug.Log("Module Activated " + gameObject.name);
       if (efficiency<0.3f)
        {
            ResourceManager.GainScrap(takenFuel);
            return;
        }
        int affectedByEfficiency = Mathf.RoundToInt((cycleMain.tics[affectedModule].GetNormalizedAmplitude() + ampGenerationValue) * efficiency);
        cycleMain.tics[affectedModule].SetAmplitude(affectedByEfficiency);
            }
}
