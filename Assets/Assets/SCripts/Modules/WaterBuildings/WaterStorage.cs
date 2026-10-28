using UnityEngine;

public class WaterStorage : WaterClass
{
    public WaterClass waterSource;
    public int waterRefillRate;
    private int _waterStoraged;
    public int waterStoraged
    {
        get => _waterStoraged;
        set
        {
            _waterStoraged = Mathf.Clamp(value, 0, waterStoragedMAX);

        }
    }
    public int waterStoragedMAX;
    public override void PreCycleActionWaterSupplier()
    {
        waterStoraged += PullWaterInStorage(waterRefillRate);
    }
    private int PullWaterInStorage(int pull)
    {
        return waterSource.SupplyWater(pull);
    }
}
