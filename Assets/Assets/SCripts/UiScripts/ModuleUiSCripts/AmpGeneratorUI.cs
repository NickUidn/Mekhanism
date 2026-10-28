using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static ModuleUIManager;

public class AmpGeneratorUI : ModuleUiClass
{
    [SerializeField] ModuleUIManager UIManager;
    [SerializeField] GeneratorAMP ampGenerator;
    [SerializeField] TMP_InputField ampField;
    [SerializeField] TMP_InputField affectedModuleField;
    [SerializeField] Button plusIndexButton;
    [SerializeField] Button minusIdexButton;
    [SerializeField]TMP_Text indexOrder;
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

    public void LinkControllsAndFields(GeneratorAMP Source)
    {
        // Always clear ALL listeners first
        plusIndexButton.onClick.RemoveAllListeners();
        minusIdexButton.onClick.RemoveAllListeners();
        ampField.onValueChanged.RemoveAllListeners();
        affectedModuleField.onValueChanged.RemoveAllListeners();
        isModuleOn.onValueChanged.RemoveAllListeners();

        ampGenerator = Source;
        newAffModule = ampGenerator.affectedModule;
        newAmpValue = ampGenerator.ampGenerationValue;

        ampField.SetTextWithoutNotify(newAmpValue.ToString());
        affectedModuleField.SetTextWithoutNotify(newAffModule.ToString());
        isModuleOn.SetIsOnWithoutNotify(ampGenerator.isModuleON);
        moduleName.SetTextWithoutNotify(ampGenerator.moduleName);

        plusIndexButton.onClick.AddListener(plusIndexUI);
        minusIdexButton.onClick.AddListener(minusIndexUI);
        ampField.onEndEdit.AddListener(NewAmpGeneration);
        affectedModuleField.onEndEdit.AddListener(NewAffectedModule);
        isModuleOn.onValueChanged.AddListener(state => ampGenerator.isModuleON = state);
        moduleName.onEndEdit.AddListener(NewName);
      

        isLinked = true;
    }

    public void NewName(string name)
    {
        ampGenerator.moduleName=name;
    }

    public void NewAffectedModule(string newAffModuleString)
    {
        newAffModule = int.TryParse(newAffModuleString, out int parsed) ? parsed : 0;

        ampGenerator.affectedModule = newAffModule;
        affectedModuleField.SetTextWithoutNotify(ampGenerator.affectedModule.ToString());
    }

    public void NewAmpGeneration(string newAmpGenString)
    {
        newAmpValue = int.TryParse(newAmpGenString, out int parsed) ? parsed : 1;

        ampGenerator.ampGenerationValue = newAmpValue;
        ampField.SetTextWithoutNotify(ampGenerator.ampGenerationValue.ToString());
    }

    private void Update()
    {
        if (isLinked)
        {
            indexOrder.text = ampGenerator.moduleIndex.ToString();
          


            if (isModuleOn.isOn != ampGenerator.isModuleON)
            {
                isModuleOn.SetIsOnWithoutNotify(ampGenerator.isModuleON);
            }
        }

    }
    public void plusIndexUI()
    {
        ampGenerator.PlusIndex();
    }
    public void minusIndexUI()
    {
        ampGenerator.MinusIndex();
    }
}
