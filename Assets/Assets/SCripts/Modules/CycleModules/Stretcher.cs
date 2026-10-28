//using UnityEngine;

//public class Stretcher : ModuleClassCycle
//{
//    [Header("Module Controlls")]
//    public int affectedModule;  
//    public bool isGoingBackwards;
//    Tic affectedTic;


//    public override void DoModuelCycle()
//    {
//        affectedTic = cycleMain.tics[affectedModule];
//        float mainFreq = affectedTic.frequency;
//        Tic nextTic = affectedTic.nextTic;
//        Tic afterNextTic= affectedTic.nextTic.nextTic;
//        Tic prevTic= affectedTic.previousTic;
//        Tic afterPrevTic= affectedTic.previousTic.previousTic;
//        affectedTic.SetFrequency(mainFreq/3);
//        if (!isGoingBackwards) 
//        {
//            nextTic.SetFrequency(nextTic.frequency+ mainFreq/4);
//            afterNextTic.SetFrequency(afterNextTic.frequency + mainFreq / 5);
//        }
//        if(isGoingBackwards)
//        {
//            prevTic.SetFrequency(prevTic.frequency + mainFreq / 4);
//            afterPrevTic.SetFrequency(afterPrevTic.frequency + mainFreq / 5);
//        }
//    }
//}
