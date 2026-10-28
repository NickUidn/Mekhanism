using UnityEngine;

public class FuelSupplier : fuelStorage
{
   
    void  OnEnable() => GameEvents.NewCycleStartedAction += ResetFuel;
    void OnDisable() => GameEvents.NewCycleStartedAction -= ResetFuel;
    private void Awake()
    {
        fuelRate = fuelRateDefault;
    }
    void ResetFuel()
    {
      
      currentFuel=defaultFuel;
      fuelRate = fuelRateDefault;
    }
}
