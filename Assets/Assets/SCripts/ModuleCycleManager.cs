using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

public class ModuleCycleManager : MonoBehaviour
{
    // List to hold all modules in order
    public List<ModuleClassCycle> modules = new List<ModuleClassCycle>();
    [SerializeField] Cycle mainCylce;
    [SerializeField] OrderUiPanel orderUi;
    bool isCycleRunning;
    public event Action PreCycleActions;
    public event Action AfterSuccessfulCycleActions;

    private void Awake()
    {
        QualitySettings.vSyncCount = 0;

     
        Application.targetFrameRate = 60;
    }
    private void Start()
    {
        ResourceManager.scrap = 100;
      
        isCycleRunning = false; 
        // Find all ModuleClassCycle components in the scene
        ModuleClassCycle[] foundModules = FindObjectsOfType<ModuleClassCycle>();
        // Add them to the list (no duplicates)
        foreach (ModuleClassCycle module in foundModules)
        {
            if (!modules.Contains(module))
            {
                modules.Add(module);
                module.SetManager(this);
            }
        }

        // Sort by moduleIndex (initial order from Inspector)
        modules = modules.OrderBy(m => m.moduleIndex).ToList();

        // Assign sequential indices starting from 1 based on sorted order
        for (int i = 0; i < modules.Count; i++)
        {
            modules[i].moduleIndex = i + 1;
        }
      
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            Debug.Log("pressed");
            StartCoroutine(StartCyle());
        }
    }
    public void StartCylceAction()
    {
        StartCoroutine(StartCyle());
    }
    public IEnumerator StartCyle()
    {
        if (isCycleRunning)
        {
            yield break;
        }

        isCycleRunning = true;
        PreCycleActions?.Invoke();
        try
        {
            mainCylce?.ResetToDefault();

            foreach (ModuleClassCycle module in modules)
            {
                if (!module.DoModuel())
                    yield break;

                yield return new WaitForSeconds(0.5f);
            }
        }
        finally
        {
            isCycleRunning = false;
            AfterSuccessfulCycleActions?.Invoke();
        }
    }
    // Called when a new module is instantiated at runtime
    public void AddModule(ModuleClassCycle newModule)
    {
        if (modules.Contains(newModule))
        {
            Debug.LogWarning("Module already exists in list. Skipping duplicate.");
            return;
        }

        modules.Add(newModule);
        newModule.SetManager(this);
        RefreshIndices();
    }

    // Move a module up in the list (towards the end / higher index)
    public void MoveModuleUp(ModuleClassCycle module)
    {
        int currentPosition = modules.IndexOf(module);

        // Can't move up if already at the end
        if (currentPosition >= modules.Count - 1)
        {
            Debug.LogWarning("Cannot increment index - module is already at the end of the list");
            return;
        }

        // Swap positions in the list
        modules.RemoveAt(currentPosition);
        modules.Insert(currentPosition + 1, module);

        // Refresh indices based on new order
        RefreshIndices();
    }
    public void MoveModuleToTop(ModuleClassCycle module)
    {
        int currentPosition = modules.IndexOf(module);

        // Can't move to top if already at the beginning
        if (currentPosition <= 0)
        {
            Debug.LogWarning("Cannot move to top - module is already at the beginning of the list");
            return;
        }

        // Remove from current position and insert at the beginning
        modules.RemoveAt(currentPosition);
        modules.Insert(0, module);

        // Refresh indices based on new order
        RefreshIndices();
    }

    public void MoveModuleToBottom(ModuleClassCycle module)
    {
        int currentPosition = modules.IndexOf(module);

        // Can't move to bottom if already at the end
        if (currentPosition >= modules.Count - 1)
        {
            Debug.LogWarning("Cannot move to bottom - module is already at the end of the list");
            return;
        }

        // Remove from current position and add to the end
        modules.RemoveAt(currentPosition);
        modules.Add(module);

        // Refresh indices based on new order
        RefreshIndices();
    }
    // Move a module down in the list (towards the beginning / lower index)
    public void MoveModuleDown(ModuleClassCycle module)
    {
        int currentPosition = modules.IndexOf(module);

        // Can't move down if already at the beginning
        if (currentPosition <= 0)
        {
            Debug.LogWarning("Cannot decrement index - module is already at the beginning of the list");
            return;
        }

        // Swap positions in the list
        modules.RemoveAt(currentPosition);
        modules.Insert(currentPosition - 1, module);

        // Refresh indices based on new order
        RefreshIndices();
    }

    // Refresh indices - assign sequential indices based on current list order
    private void RefreshIndices()
    {
        for (int i = 0; i < modules.Count; i++)
        {
            modules[i].moduleIndex = i + 1;
        }
        orderUi.ReorderOrdeUI();
    }

    // Public method to remove a module from the list
    public void RemoveModule(ModuleClassCycle module)
    {
        if (modules.Contains(module))
        {
            modules.Remove(module);
            RefreshIndices();
        }
    }

    // Public method to get module by list position (0-based)
    public ModuleClassCycle GetModule(int index)
    {
        if (index >= 0 && index < modules.Count)
        {
            return modules[index];
        }
        return null;
    }

    // Public method to get module by its moduleIndex (1-based)
    public ModuleClassCycle GetModuleByModuleIndex(int moduleIndex)
    {
        return modules.FirstOrDefault(m => m.moduleIndex == moduleIndex);
    }

    // Public method to get all modules
    public List<ModuleClassCycle> GetAllModules()
    {
        return modules;
    }

    // Get the position of a module in the list (0-based)
    public int GetModulePosition(ModuleClassCycle module)
    {
        return modules.IndexOf(module);
    }

    // For debugging
    public void PrintList()
    {
        string output = "";
        for (int i = 0; i < modules.Count; i++)
        {
            output += $"Position {i}: {modules[i].gameObject.name} (index: {modules[i].moduleIndex})\n";
        }
        Debug.Log(output);
    }
    public void AddModuleToEnd(ModuleClassCycle newModule)
    {
        if (modules.Contains(newModule))
        {
            Debug.LogWarning("Module already exists in list. Skipping duplicate.");
            return;
        }

        // Find the highest current index
        int maxIndex = 0;
        foreach (ModuleClassCycle module in modules)
        {
            if (module.moduleIndex > maxIndex)
            {
                maxIndex = module.moduleIndex;
            }
        }

        // Add to the list at the end
        modules.Add(newModule);
        newModule.SetManager(this);

        // Give it the next index
        newModule.moduleIndex = maxIndex + 1;

        // Refresh indices based on new order (sort by moduleIndex)
        modules = modules.OrderBy(m => m.moduleIndex).ToList();
        RefreshIndices();
    }
}