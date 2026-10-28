using UnityEngine;

public class Generator : ModuleClassCycle
{
    [Header("Module Controlls")]
    public int affectedModule;
    public override void DoModuelCycle()
    {
        Debug.Log("Module Activated " + gameObject.name);
        cycleMain.tics[affectedModule].frequency = cycleMain.tics[affectedModule].frequency + 0.5f;
        if (affectedModule % 2 != 0) // If odd
        {
            cycleMain.tics[affectedModule].amplitude = -(Mathf.Abs(cycleMain.tics[affectedModule].amplitude )+1f);
        }
        else
        {
            cycleMain.tics[affectedModule].amplitude++;
        }
    }
}
