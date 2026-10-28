using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;

public class ModuleClassCycle : BuildingClass
{
   
    public bool isModuleON;   
    public string moduleName="ThisModule";
    protected Cycle cycleMain;
    public int moduleIndex;
    private bool isRegistered = false;
    private ModuleCycleManager manager;
    [SerializeField] GameObject OrderUi;
    private Transform OrderUiScrollContent;
    private GameObject linkedOrderUi;
    public float efficiency=1f;
protected int takenFuel;

    [Header("Fuel")]

    public int _fuelConsumption;
    public int fuelConsumption
    {
        get => _fuelConsumption;
        set
        {
            // Clamp the value
            _fuelConsumption = Math.Clamp(value, 0, 999);
        }
    }
    public fuelStorage fuelSupplier;
    [Header("Durability")]
    public float durabilityMax;
    private float durabilityMin = 0;
    [SerializeField] private float _durabilityCurrent;
    public float durabilityCurrent
    {
        get => _durabilityCurrent;
        set
        {
            // Clamp the value
            _durabilityCurrent = Mathf.Clamp(value, durabilityMin, durabilityMax);
        }
    }

    public float wearAndTearRate;
    [Header("Water")]
    public bool isCoolingWithDirtyWater;
    public float wearAndTearWithDirtyWater;
    public float waterConsuptiuonRate;
    private float weakCoolingModifier;
    public float waterSupplied;

    [Header("Tick if Module Produces Temperature")]
    public bool isTempGenerator;
    public float tempDamage;
    public float tempGain;
    private float tempretureMin = 0;
    public float tempretureMax;
    public float tempretureDanger;
    [SerializeField] private float _tempretureCurrent;
    public float tempretureCurrent
    {
        get => _tempretureCurrent;
        set
        {
            // Clamp the value
            _tempretureCurrent = Mathf.Clamp(value, tempretureMin, tempretureMax);
        }
    }

    [Header("Cooling")]
    public bool isCooling;
    public float coolingRate;

    private void Start()
    {

        cycleMain = GameObject.Find("Cycle").GetComponent<Cycle>();
        //TEMPORY!!!
        manager = GameObject.Find("ModuleManager").GetComponent<ModuleCycleManager>();
        OrderUiScrollContent = GameObject.Find("OrderUiScrollContent").GetComponent<Transform>();
        //TEMPORY!!!
        if (!manager.GetAllModules().Contains(this))
        {
            RegisterThisModule();
        }
        GameObject pannel = Instantiate(OrderUi, OrderUiScrollContent);
        pannel.GetComponent<ModuleOrderUi>().module = this;
    }   
   
  
    private void RegisterThisModule()
    {
        manager.AddModuleToEnd(this);
    }
    public bool isFueled()
    {
        if (ResourceManager.OVERRIDE_FUEL)
        {
            return true;
        }
        takenFuel = ResourceManager.FuelScrap(fuelConsumption, out efficiency);
        if (efficiency == 0) { return false; }
        else {  return true; }
     
        /////// ****Future Work For Propper Fuel System
        //if (fuelConsumption == 0) { return true; }
        //if (fuelSupplier!=null&&fuelSupplier.currentFuel>=fuelConsumption&& fuelSupplier.fuelRate>=fuelConsumption)
        //{
        //    fuelSupplier.currentFuel-= fuelConsumption;
        //    fuelSupplier.fuelRate-= fuelConsumption;
        // return true;
        //}
        //Debug.Log("Out of Fuel!" + gameObject.name);
        //return false;
    }


    public bool CheckDurabilityOk()
    {
        if (ResourceManager.OVERRIDE_WEAR_AND_TEAR) { return true; }
        if (durabilityCurrent==0f) { Debug.Log("Broke! Durab"); return false;}
        return true;
    }
    public bool CheckTempOk()
    {
        if (ResourceManager.OVERRIDE_COOLING) { return true; }
        if (tempretureCurrent==tempretureMax ) { Debug.Log("Broke! Temp"); return false; }
        return true;
    }
    public void DurabilityLoss(float durabilitySingleDmage)
    {
        if (ResourceManager.OVERRIDE_WEAR_AND_TEAR) { return; }
        durabilityCurrent = durabilityCurrent - durabilitySingleDmage;
       
    }
    public void FlameOut()
    {
        if (ResourceManager.OVERRIDE_COOLING) { return; }
        durabilityCurrent =0f;
    }
    public void TempDamage()
    {
        if (ResourceManager.OVERRIDE_COOLING) { return; }
        DurabilityLoss(tempDamage);
    }  
    
    public void Cooling()
    {
        if (ResourceManager.OVERRIDE_COOLING) { return; }
        if (!isCooling)
        {
            
            if (isTempGenerator)
            {
                tempretureCurrent = tempretureCurrent + tempGain;
            }
            else if (tempretureCurrent >= tempretureDanger && tempretureCurrent != tempretureMax)
            {
                TempDamage();
            }
            else if (tempretureCurrent == tempretureMax)
            {
               FlameOut();
            }

        }
        else
        {
            if (isCoolingWithDirtyWater)
            {
                DurabilityLoss(wearAndTearWithDirtyWater);
            }
            if (waterSupplied < waterConsuptiuonRate)
            {
                weakCoolingModifier = coolingRate - waterSupplied / waterConsuptiuonRate;
                tempretureCurrent = tempretureCurrent + tempGain -  weakCoolingModifier;
            }
            
            tempretureCurrent = tempretureCurrent + tempGain - coolingRate;

            if (tempretureCurrent >= tempretureDanger && tempretureCurrent != tempretureMax)
            {
                TempDamage();
            }
          if (tempretureCurrent == tempretureMax)
            {
                FlameOut();
            }
        }

    }
    public void WearAndTear()
    {
        durabilityCurrent = durabilityCurrent - wearAndTearRate;
    }
    public virtual bool DoModuel()
    {
        GameEvents.NewCycleStarted();
        if (!isModuleON)
        { return true;}
        if (ResourceManager.TESTING_FUNCTIONALITY)
        { DoModuelCycle(); return true; }
        Cooling();
         if (CheckDurabilityOk()==false || CheckTempOk() == false)
        { Debug.Log("Broke!");
            return false;
                      }
        if (isFueled() == false)
        {
            return false;
        }
        DoModuelCycle();
       WearAndTear();
        Debug.Log(gameObject.name);
        return true;
    }
    public virtual void DoModuelCycle()
    {
        
    }
    #region Index&Registration&Destraction
    // Called by the manager when this module is added to the list
    public void SetManager(ModuleCycleManager newManager)
    {
        if (!isRegistered)
        {
            manager = newManager;
            isRegistered = true;
        }
    }
    private void OnDestroy()
    {
        manager.RemoveModule(this);
        Destroy(linkedOrderUi);
    }
    public void PlusIndex()
    {
        if (manager == null)
        {
            Debug.LogWarning("Manager not set for " + gameObject.name);
            return;
        }
        manager.MoveModuleUp(this);

    }

    public void MinusIndex()
    {
        if (manager == null)
        {
            Debug.LogWarning("Manager not set for " + gameObject.name);
            return;
        }
        manager.MoveModuleDown(this);

    }
    public void TopIndex()
    { 
        if (manager == null)
        {
            Debug.LogWarning("Manager not set for " + gameObject.name);
            return;
        }
        manager.MoveModuleToTop(this);
            }
    public void BottomIndex()
    {
        if (manager == null)
        {
            Debug.LogWarning("Manager not set for " + gameObject.name);
            return;
        }
        manager.MoveModuleToBottom(this);
    }
    public void RegisterWithManager(bool addToEnd = true)
    {
        if (manager == null)
        {
            manager = FindObjectOfType<ModuleCycleManager>();
            if (manager == null)
            {
              
                return;
            }
        }

        if (addToEnd)
        {
            manager.AddModuleToEnd(this);
        }
        else
        {
            manager.AddModule(this);
        }
        isRegistered = true;
    }
    #endregion
}
