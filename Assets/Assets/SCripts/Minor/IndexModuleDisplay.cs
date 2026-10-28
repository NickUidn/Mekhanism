using UnityEngine;
using TMPro;

public class IndexModuleDisplay : MonoBehaviour
{
   [SerializeField] TMP_Text indexNumber;
   [SerializeField]ModuleClassCycle module;
   
    void Update()
    {
        indexNumber.text = module.moduleIndex.ToString();
    }
}
