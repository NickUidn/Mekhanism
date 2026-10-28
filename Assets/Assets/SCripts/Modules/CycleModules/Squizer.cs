//using UnityEngine;

//public class Squizer : ModuleClassCycle
//{
//    [Header("Module Controlls")]
//    public int affectedModule;
//    [Header("Squizer")]
//    public float squizerValueLeft;
//    public float squizerValueToSides;
//    public override void DoModuelCycle()
//    {              
        
//        float mainAMP= Mathf.Abs(cycleMain.tics[affectedModule].amplitude);
//        float newMainAmp = mainAMP * squizerValueLeft;
//        float newSidesAmp =mainAMP * squizerValueToSides;
//        cycleMain.tics[affectedModule].SetAmplitude(Mathf.Abs(newMainAmp));
//        if (cycleMain.tics[affectedModule].nextTic != null)
//        {
//            cycleMain.tics[affectedModule].nextTic.SetAmplitude(Mathf.Abs(cycleMain.tics[affectedModule].nextTic.amplitude)+ newSidesAmp);
//        }
//        if (cycleMain.tics[affectedModule].previousTic != null)
//        {
//            cycleMain.tics[affectedModule].previousTic.SetAmplitude(Mathf.Abs(cycleMain.tics[affectedModule].previousTic.amplitude)+ newSidesAmp);
//        }
//    }

  
//}
