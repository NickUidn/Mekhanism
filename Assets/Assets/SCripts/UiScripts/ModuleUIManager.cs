using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using UnityEngine;
using static Unity.VisualScripting.Member;

public class ModuleUIManager : MonoBehaviour
{
    [SerializeField] KeyPresses keyMonitor;
    [SerializeField]public PanelMover panelMover;
    public BuildingClass currentModule;

    [SerializeField] AmpGeneratorUI AmpGeneratorUI;
    [SerializeField] AmplificatorUI AmplificatorUI;
    [SerializeField] FreqwenserUI FreqwenserUI;
    private void Awake()
    {
        allModuleUIs = new MonoBehaviour[] { AmpGeneratorUI, AmplificatorUI,FreqwenserUI };
    }
    private MonoBehaviour[] allModuleUIs;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            DeselectModue();
        }
    }

    public void DeselectModue()
    {
        if (currentModule != null)
        {
            currentModule.DeSelectedOnMap();
        }
        panelMover.MoveOffScreen();
    }
    public void MatchUiAndModule()
    {
        foreach (var ui in allModuleUIs)
            ui.gameObject.SetActive(false);

        switch (currentModule)
        {
            case GeneratorAMP module:
                AmpGeneratorUI.gameObject.SetActive(true);
                AmpGeneratorUI.LinkControllsAndFields(module);
                break;

            case Amplificator module:
                AmplificatorUI.gameObject.SetActive(true);
                AmplificatorUI.LinkControllsAndFields(module);
                break;
            case Freqwenser module:
                FreqwenserUI.gameObject.SetActive(true);
                FreqwenserUI.LinkControllsAndFields(module);
                break;
        }

    }
}
