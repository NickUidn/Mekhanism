using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static ModuleUIManager;

public class AmplificatorUI : MonoBehaviour
{
    [SerializeField] ModuleUIManager UIManager;
    [SerializeField] Amplificator amplificator;
    [SerializeField] TMP_InputField ampField;
    [SerializeField] TMP_InputField affectedModuleField;
    [SerializeField] Button plusIndexButton;
    [SerializeField] Button minusIdexButton;
    [SerializeField] TMP_Text indexOrder;
    [SerializeField] TMP_InputField moduleName;
    [SerializeField] Toggle isModuleOn;
    int _newAffModule;
    public int newAffModule
    {
        get => _newAffModule;
        set
        {
            _newAffModule = Mathf.Clamp(value, 0, 9);
        }
    }
    int _newAmpValue;
    public int newAmpValue
    {
        get => _newAmpValue;
        set
        {
            _newAmpValue = Math.Clamp(value, 1, 100);

        }
    }
    bool isLinked;

    public void LinkControllsAndFields(Amplificator Source)
    {
        // Always clear ALL listeners first
        plusIndexButton.onClick.RemoveAllListeners();
        minusIdexButton.onClick.RemoveAllListeners();
        ampField.onValueChanged.RemoveAllListeners();
        affectedModuleField.onValueChanged.RemoveAllListeners();
        isModuleOn.onValueChanged.RemoveAllListeners();

        amplificator = Source;
        newAffModule = amplificator.affectedModule;
        newAmpValue = amplificator.amplificatorValue;

        ampField.SetTextWithoutNotify(newAmpValue.ToString());
        affectedModuleField.SetTextWithoutNotify(newAffModule.ToString());
        isModuleOn.SetIsOnWithoutNotify(amplificator.isModuleON);
        moduleName.SetTextWithoutNotify(amplificator.moduleName);

        plusIndexButton.onClick.AddListener(plusIndexUI);
        minusIdexButton.onClick.AddListener(minusIndexUI);
        ampField.onEndEdit.AddListener(NewAmpGeneration);
        affectedModuleField.onEndEdit.AddListener(NewAffectedModule);
        isModuleOn.onValueChanged.AddListener(state => amplificator.isModuleON = state);
        moduleName.onEndEdit.AddListener(NewName);


        isLinked = true;
    }

    public void NewName(string name)
    {
        amplificator.moduleName = name;
    }

    public void NewAffectedModule(string newAffModuleString)
    {
        newAffModule = int.TryParse(newAffModuleString, out int parsed) ? parsed : 0;

        amplificator.affectedModule = newAffModule;
        affectedModuleField.SetTextWithoutNotify(amplificator.affectedModule.ToString());
    }

    public void NewAmpGeneration(string newAmpGenString)
    {
        newAmpValue = int.TryParse(newAmpGenString, out int parsed) ? parsed : 1;

        amplificator.amplificatorValue = newAmpValue;
        ampField.SetTextWithoutNotify(amplificator.amplificatorValue.ToString());
    }

    private void Update()
    {
        if (isLinked)
        {
            indexOrder.text = amplificator.moduleIndex.ToString();



            if (isModuleOn.isOn != amplificator.isModuleON)
            {
                isModuleOn.SetIsOnWithoutNotify(amplificator.isModuleON);
            }
        }

    }
    public void plusIndexUI()
    {
        amplificator.PlusIndex();
    }
    public void minusIndexUI()
    {
        amplificator.MinusIndex();
    }
}
