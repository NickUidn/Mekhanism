using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ModuleOrderUi : MonoBehaviour
{
    [SerializeField] Button up;
    [SerializeField] Button down;
    [SerializeField] Button top;
    [SerializeField] Button bottom;
    [SerializeField] Toggle onToggle;
    private TMP_Text moduleName;   
    public ModuleClassCycle module;
    private ModuleUIManager uiManager;

    void Start()
    {
        up.onClick.RemoveAllListeners();
        down.onClick.RemoveAllListeners();
        top.onClick.RemoveAllListeners();
        bottom.onClick.RemoveAllListeners();
        onToggle.onValueChanged.RemoveAllListeners();

        
        moduleName = GetComponentInChildren<TMP_Text>();
      
        // cycleManager = GameObject.Find("ModuleManager").GetComponent<ModuleCycleManager>();
        up.onClick.AddListener(module.PlusIndex);
        down.onClick.AddListener(module.MinusIndex);
        top.onClick.AddListener(module.TopIndex);
        bottom.onClick.AddListener(module.BottomIndex);

        up.onClick.AddListener(CallmanagerOnscreen);
        down.onClick.AddListener(CallmanagerOnscreen);
        top.onClick.AddListener(CallmanagerOnscreen);
        bottom.onClick.AddListener(CallmanagerOnscreen);

        onToggle.onValueChanged.AddListener(state=> module.isModuleON = state);
        uiManager = FindAnyObjectByType<ModuleUIManager>();
    }
    void Update()
    {
        if (module == null) return;
        moduleName.text = module.moduleName;
        if (onToggle.isOn != module.isModuleON)
            onToggle.SetIsOnWithoutNotify(module.isModuleON);
    }

    public void CallmanagerOnscreen()
    {
        uiManager.currentModule = module;
        uiManager.MatchUiAndModule();
        uiManager.panelMover.MoveOnScreen();
        uiManager.currentModule.SelectedOnMap();
    }
}
