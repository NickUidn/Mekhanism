using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static ModuleUIManager;

public class FreqwenserUI : MonoBehaviour
{
    [SerializeField] ModuleUIManager UIManager;
    [SerializeField] Freqwenser freqwenser;
    [SerializeField] TMP_InputField freqField;
    [SerializeField] TMP_InputField affectedModuleField;
    [SerializeField] Button plusIndexButton;
    [SerializeField] Button minusIdexButton;
    [SerializeField] TMP_Text indexOrder;
    [SerializeField] TMP_InputField moduleName;
    [SerializeField] Toggle isModuleOn;
    public bool isGoingBackwards;
    int _newAffModule;
    public int newAffModule
    {
        get => _newAffModule;
        set
        {
            _newAffModule = Mathf.Clamp(value, 0, 9);
        }
    }
    int _newFreqValue;
    public int newFreqValue
    {
        get => _newFreqValue;
        set
        {
            _newFreqValue = Math.Clamp(value, 1, 100);

        }
    }
    bool isLinked;

    public void LinkControllsAndFields(Freqwenser Source)
    {
        // Always clear ALL listeners first
        plusIndexButton.onClick.RemoveAllListeners();
        minusIdexButton.onClick.RemoveAllListeners();
        freqField.onValueChanged.RemoveAllListeners();
        affectedModuleField.onValueChanged.RemoveAllListeners();
        isModuleOn.onValueChanged.RemoveAllListeners();

        freqwenser = Source;
        newAffModule = freqwenser.affectedModule;
       

        freqField.SetTextWithoutNotify(newFreqValue.ToString());
        affectedModuleField.SetTextWithoutNotify(newAffModule.ToString());
        isModuleOn.SetIsOnWithoutNotify(freqwenser.isModuleON);
        moduleName.SetTextWithoutNotify(freqwenser.moduleName);

        plusIndexButton.onClick.AddListener(plusIndexUI);
        minusIdexButton.onClick.AddListener(minusIndexUI);
        freqField.onEndEdit.AddListener(NewValue);
        affectedModuleField.onEndEdit.AddListener(NewAffectedModule);
        isModuleOn.onValueChanged.AddListener(state => freqwenser.isModuleON = state);
        moduleName.onEndEdit.AddListener(NewName);


        isLinked = true;
    }

    public void NewName(string name)
    {
        freqwenser.moduleName = name;
    }

    public void NewAffectedModule(string newAffModuleString)
    {
        newAffModule = int.TryParse(newAffModuleString, out int parsed) ? parsed : 0;

        freqwenser.affectedModule = newAffModule;
        affectedModuleField.SetTextWithoutNotify(freqwenser.affectedModule.ToString());
    }

    public void NewValue(string value)
    {
        newFreqValue = int.TryParse(value, out int parsed) ? parsed : 0;
        freqwenser.FreqwenserValue = newFreqValue;
        freqField.SetTextWithoutNotify(freqwenser.FreqwenserValue.ToString());

    }

    private void Update()
    {
        if (isLinked)
        {
            indexOrder.text = freqwenser.moduleIndex.ToString();



            if (isModuleOn.isOn != freqwenser.isModuleON)
            {
                isModuleOn.SetIsOnWithoutNotify(freqwenser.isModuleON);
            }
        }

    }
    public void plusIndexUI()
    {
        freqwenser.PlusIndex();
    }
    public void minusIndexUI()
    {
        freqwenser.MinusIndex();
    }
}
