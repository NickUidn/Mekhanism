using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ContractClass : MonoBehaviour
{
    private ModuleCycleManager cycleManager;
    private Cycle cycle;

    [System.Serializable]
    public class ContractDataForTic
    {
        public int contractPosition;
        public int targetTicIndex;

        private int _margins;
        public int margins
        {
            get => _margins;
            set => _margins = Mathf.Clamp(value, 1, 100);
        }

        private int _contractAmp;
        public int contractAmp
        {
            get => _contractAmp;
            set => _contractAmp = Mathf.Clamp(value, 1, 100);
        }

        private int _contractFreq;
        public int contractFreq
        {
            get => _contractFreq;
            set => _contractFreq = Mathf.Clamp(value, 1, 100);
        }

        public ContractDataForTic(int targetTicIndex, int margins, int contractAmp, int contractFreq)
        {
            this.targetTicIndex = targetTicIndex;
            this.margins = margins;
            this.contractAmp = contractAmp;
            this.contractFreq = contractFreq;
        }

        public ContractDataForTic() { }
    }

    [SerializeField] public List<ContractDataForTic> contractBody = new List<ContractDataForTic>();

    // NEW: runtime list of spawned segment components, in spawn order
   public List<ContractSegment> segments = new List<ContractSegment>();

    [SerializeField] GameObject segmentPrefab;
    public int reward;
    public bool isOdd;
    public int currentIndex;
    public float contractTime;
    private int _contractLength = 0;

    public int contractLength
    {
        get => _contractLength;
        set => _contractLength = Mathf.Clamp(value, 0, 5);
    }

    public bool isContractActive;

    private void Start()
    {
        currentIndex = 0;
        cycle = FindAnyObjectByType<Cycle>();
        cycleManager = FindAnyObjectByType<ModuleCycleManager>();
        cycleManager.AfterSuccessfulCycleActions += CheckContract;

        AddContractSegment(0, 5, 20, 50);
        AddContractSegment(1, 5, 40, 30);
        AddContractSegment(3, 5, 10, 20);
        BuildContractBody();
    }

    private void OnDisable()
    {
        cycleManager.AfterSuccessfulCycleActions -= CheckContract;
    }

  

    public void AddContractSegment(int targetTicIndex, int margins, int contractAmp, int contractFreq)
    {
        contractLength++;
        contractBody.Add(new ContractDataForTic(targetTicIndex, margins, contractAmp, contractFreq));
    }

    public void SnapedToPosition()
    {
    }

    public void MatchKeyToSegmentIndex()
    {
        for (int i = 0; i < contractBody.Count; i++)
        {
            // If you need to do anything per segment, use contractBody[i].targetTicIndex here.
        }
    }

    public void BuildContractBody()
    {
        // Clear any previously-built segments so we don't duplicate on rebuild
        ClearSegments();

        if (segmentPrefab == null)
        {
            Debug.LogError($"[{name}] segmentPrefab is NOT assigned.", this);
            return;
        }

        float offset = 0f;
        int positionInSequance = 0;

        foreach (ContractDataForTic data in contractBody)
        {
            Vector3 spawnPos = transform.position + transform.right * offset;

            GameObject contractVis = Instantiate(
                segmentPrefab,
                spawnPos,
                transform.rotation,
                transform);

            ContractVis vis = contractVis.GetComponentInChildren<ContractVis>();
            SpriteRenderer visSprite = contractVis.GetComponentInChildren<SpriteRenderer>();
            ContractSegment contractSegment = contractVis.GetComponent<ContractSegment>();

            vis.contract = this;
            positionInSequance++;
            contractSegment.positionInSequance = positionInSequance;
            contractSegment.contract = this;
            contractSegment.body = gameObject.transform.parent.gameObject;

            offset += visSprite.bounds.size.x;

            if (vis != null)
            {
                vis.SetAmplitude(data.contractAmp);
                vis.SetFrequency(data.contractFreq);
                vis.length = 5f;
            }
            vis.DrawStaticWave();

            // NEW: track the spawned segment for later use
            segments.Add(contractSegment);
        }
    }

    // NEW: iterate over the built segments
    public void ProcessSegments()
    {
        foreach (ContractSegment seg in segments)
        {
            if (seg == null) continue;
            // e.g. seg.DoSomething();
        }
    }

    // NEW: destroy and clear built segments
    public void ClearSegments()
    {
        foreach (ContractSegment seg in segments)
        {
            if (seg != null)
            {
                Destroy(seg.gameObject);
            }
        }
        segments.Clear();
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

        foreach (ContractDataForTic data in contractBody)
        {
            int targetTicIndex = data.targetTicIndex;

            if (cycle == null || cycle.tics == null || targetTicIndex < 0 || targetTicIndex >= cycle.tics.Length)
            {
                Debug.Log($"Contract failed: no matching tic at index {targetTicIndex}.");
                return;
            }

            var target = cycle.tics[targetTicIndex];

            float amp = target.GetNormalizedAmplitude();
            float freq = target.GetNormalizedFrequency();

            bool amplitudeMatches = amp >= (data.contractAmp - data.margins)
                                 && amp <= (data.contractAmp + data.margins);

            bool frequencyMatches = freq >= (data.contractFreq - data.margins)
                                 && freq <= (data.contractFreq + data.margins);

            if (!amplitudeMatches || !frequencyMatches)
            {
                Debug.Log($"Contract at tic {targetTicIndex} failed. " +
                          $"Amp: {amp} vs [{data.contractAmp - data.margins}, {data.contractAmp + data.margins}], " +
                          $"Freq: {freq} vs [{data.contractFreq - data.margins}, {data.contractFreq + data.margins}]");
                return;
            }
        }

        Debug.Log("Yep! " + reward.ToString());
    }
}