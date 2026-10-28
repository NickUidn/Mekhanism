using System;
using UnityEngine;

public static class ResourceManager
{
    //Debug Stuff
   public static bool TESTING_FUNCTIONALITY;
   public static bool OVERRIDE_COOLING;
   public static bool OVERRIDE_FUEL;
    public static bool OVERRIDE_WEAR_AND_TEAR;
    //

    private const int SCRAP_MAX = 9999;   
    private static int _scrap;           
    public static int scrap
    {
        get => _scrap;
        set => _scrap = Mathf.Clamp(value, 0, SCRAP_MAX);
    }

    public static int FuelScrap(int taken, out float percent)
    {      
        if(TESTING_FUNCTIONALITY || OVERRIDE_FUEL)
        {
            percent = 1f;
            return 0;
        }
        if (taken <= 0)
        { 
            percent = 0f;           
            return 0;
        }       
        int actualTake = Math.Min(taken, scrap);       
        scrap -= actualTake;        
        percent = (float)actualTake / taken;
        
        return actualTake;
    }
    public static void GainScrap(int gained)
    {
        scrap += gained;
    }
}
