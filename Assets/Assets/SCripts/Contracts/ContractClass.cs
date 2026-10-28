using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class ContractClass : MonoBehaviour
{
    private ModuleCycleManager cycleManager;
    private Cycle cycle;   
    [System.Serializable]public struct ContractDataForTic
    {
        private int _margins;
        public int margins
        {
            get => _margins;
            set
            {
                _margins = Math.Clamp(value, 1, 100);
            }
        }
        int _contractAmp;
        public int contractAmp
        {
            get => _contractAmp;
            set
            {
                _contractAmp = Math.Clamp(value, 1, 100);

            }
        }
        int _contractFreq;
        public int contractFreq
        {
            get => _contractFreq;
            set
            {
                _contractFreq = Math.Clamp(value, 1, 100);

            }
        }
        public ContractDataForTic(int margins, int contractAmp,int contractFreq) : this()
        {
            this.margins = margins;
            this.contractAmp = contractAmp;
            this.contractFreq = contractFreq;
        }
    }
    [SerializeField] public  Dictionary<int, ContractDataForTic> contractBody = new Dictionary<int, ContractDataForTic>();
    [SerializeField] GameObject segmentPrefab;
    public int reward;
    public bool isOdd;
    public float contractTime;
    private int _contractLength=0;
   
    
    public int contractLength      
    {
        get => _contractLength;
        set => _contractLength = Mathf.Clamp(value, 0, 5);
    }

    public bool isContractActive;

    private void Start()
    {
       cycle=FindAnyObjectByType<Cycle>();
        cycleManager =FindAnyObjectByType<ModuleCycleManager>();
        cycleManager.AfterSuccessfulCycleActions += CheckContract;
        AddContractSegment(0, 20, 20, 50);
        AddContractSegment(1, 20, 40, 30);
        AddContractSegment(3, 20, 10, 20);
    }
   
   
    private void OnDisable()
    {
        cycleManager.AfterSuccessfulCycleActions-= CheckContract;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            BuildContractBody();
        }
       
    }
    public void AddContractSegment(int key, int margins, int contractAmp, int contractFreq)
    {
        contractLength++;
        contractBody[key] = new ContractDataForTic(margins, contractAmp, contractFreq);
    }
    public void BuildContractBody()
    {
        if (segmentPrefab == null)
        {
            Debug.LogError($"[{name}] segmentPrefab is NOT assigned.", this);
            return;
        }

        Vector3 parentLossy = transform.lossyScale;
        Vector3 childLocalScale = new Vector3(
            parentLossy.x != 0f ? 1f / parentLossy.x : 1f,
            parentLossy.y != 0f ? 1f / parentLossy.y : 1f,
            parentLossy.z != 0f ? 1f / parentLossy.z : 1f);

        float offset = 0f;
        foreach (KeyValuePair<int, ContractDataForTic> entry in contractBody)
        {
            ContractDataForTic data = entry.Value;

            Vector3 spawnPos = transform.position + transform.right * offset;

            GameObject contractVis = Instantiate(
                segmentPrefab,
                spawnPos,
                transform.rotation,
                transform);

           contractVis.transform.localScale = childLocalScale;

            ContractVis vis = contractVis.GetComponentInChildren<ContractVis>();
            if (vis != null)
            {
                vis.SetAmplitude(data.contractAmp);
                vis.SetFrequency( data.contractFreq);
                vis.length = 5f;
            }

            offset += 5f;
        }   

}
    public void CheckContract()
    {
        if (!isContractActive)
            {
            return;   
            }
        if (contractBody.Count == 0)
        {
            Debug.Log("No contracts to check.");
            return;
        }

        foreach (var kvp in contractBody)
        {
            int key = kvp.Key;
            ContractDataForTic data = kvp.Value;

            // Bounds check — the tics list must have this index
            if (cycle == null || cycle.tics == null || key < 0 || key >= cycle.tics.Length)
            {
                Debug.Log($"Contract {key} failed: no matching tic at index {key}.");
                return;
            }

            var target = cycle.tics[key];

            float amp = target.GetNormalizedAmplitude();   // roughly 1–100
            float freq = target.GetNormalizedFrequency();   // roughly 1–100

            bool amplitudeMatches = amp >= (data.contractAmp - data.margins)
                                 && amp <= (data.contractAmp + data.margins);

            bool frequencyMatches = freq >= (data.contractFreq - data.margins)
                                 && freq <= (data.contractFreq + data.margins);

            if (!amplitudeMatches || !frequencyMatches)
            {
                Debug.Log($"Contract {key} failed. " +
                          $"Amp: {amp} vs [{data.contractAmp - data.margins}, {data.contractAmp + data.margins}], " +
                          $"Freq: {freq} vs [{data.contractFreq - data.margins}, {data.contractFreq + data.margins}]");
                return;
            }
        }

        Debug.Log("Yep! " + reward.ToString());
    }
}
