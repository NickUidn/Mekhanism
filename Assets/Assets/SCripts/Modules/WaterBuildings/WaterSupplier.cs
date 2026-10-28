using UnityEngine;


public class WaterClass : BuildingClass
{

    private ModuleCycleManager manager;
    public WaterClass nextConection;
    public int waterFlowRateDefault;
    private int _waterFlowRate;
    public int waterFlowRate
    {
        get => _waterFlowRate;
        set
        {
            _waterFlowRate = Mathf.Clamp(value, 0, waterFlowRateDefault);

        }
    }
    private void OnEnable()
    {
        if (manager != null)
            manager.PreCycleActions += PreCycleActionWaterSupplier;
    }

    private void OnDisable()
    {
        if (manager != null)
            manager.PreCycleActions -= PreCycleActionWaterSupplier;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        manager = FindAnyObjectByType<ModuleCycleManager>();
    }
   virtual public void PreCycleActionWaterSupplier()
    {
        
    }
    public int SupplyWater(int request)
    {

        if (request <= waterFlowRate)
        {
            waterFlowRate = waterFlowRate - request;
            return request;

        }
        else if (request > waterFlowRate && waterFlowRate != 0)
        {
            return request - waterFlowRate;
        }
        else
        {
            return 0;
        }
    }
}
public class WaterSupplier : WaterClass
{  
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
       
   
    override public void PreCycleActionWaterSupplier()
    {
        waterStoraged = waterStoraged + waterRefillRate;
         waterFlowRate= waterFlowRateDefault;
    }
    
   
}
