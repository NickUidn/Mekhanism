using System;
using UnityEngine;

public class Cycle : MonoBehaviour
{
    public Tic tic0;
    public Tic tic1;
    public Tic tic2;
    public Tic tic3;
    public Tic tic4;
    public Tic tic5;
    public Tic tic6;
    public Tic tic7;
    public Tic tic8;
    public Tic tic9;
    public Tic[] tics;
    private void Awake()
    {
        tics = new Tic[] { tic0, tic1, tic2, tic3, tic4, tic5, tic6, tic7, tic8, tic9 };
   
    }
    private void Start()
    {
        ResetToDefault();
    }
    private void Update()
    {
        //For tests
        if (Input.GetKeyDown(KeyCode.F))
        {
         
        }
        //
    }
    public void ResetToDefault()
    {
        foreach (Tic tic in tics)
        {
            tic.amplitude = tic.defaultAmpValue;
            tic.frequency=tic.defaultFreqValue;
        }
    }
}
