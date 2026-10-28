using UnityEngine;

public class TestModule : MonoBehaviour
{
    [SerializeField] Cycle cycle;
    [SerializeField] int affectedZone;
    [SerializeField] float amplitudeAfectorReduction;
    private Tic affectedTic;

    private void Start()
    {
        cycle=GameObject.Find("Cycle").GetComponent<Cycle>();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            AfectorSeek();
        }
    }

    public void AfectorSeek()
    {
        cycle.ResetToDefault();
        foreach (Tic tic in cycle.tics)
        {
            if (tic.ticIndex == affectedZone)
            {
                Debug.Log(tic.name);
                affectedTic=tic;
                DoAfector();
            }
        }
    }

    public void DoAfector()
    {
        
        affectedTic.frequency = affectedTic.frequency+ amplitudeAfectorReduction;
        Debug.Log(affectedTic.frequency);

    }
}
