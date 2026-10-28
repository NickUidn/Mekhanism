using System;
using UnityEngine;

public class BuildUiManager : MonoBehaviour
{
    [SerializeField] PanelMover panelMover;
    public BuildingClass selectedBuildingToBuild;
    public GameObject selectedBuildingPrefab;
    bool isPanelOnscreen=false;
    // Backing field - do NOT expose this publicly
    [SerializeField] private bool _isInBuildingMode;

    // Event that fires only when the value changes
    // Passes the NEW value
    public event Action<bool> OnBuildingModeChanged;

    // Public property wraps the bool
    public bool isInBuildingMode
    {
        get => _isInBuildingMode;
        set
        {
            if (_isInBuildingMode == value) return; // no change, skip
            _isInBuildingMode = value;
            OnBuildingModeChanged?.Invoke(_isInBuildingMode);
         
        }
    }
    private void Start()
    {
        panelMover=GetComponent<PanelMover>();
    }

    public void SelectedBuildingToBuild(BuildingClass building, GameObject prefab)
    {
        selectedBuildingPrefab = prefab;
        isInBuildingMode = true;
        selectedBuildingToBuild=building;
    }

    public void DeselectBuilding()
    {
        isInBuildingMode = false;
        selectedBuildingToBuild =null;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            DeselectBuilding();
            isPanelOnscreen = false;
        }
        if (Input.GetMouseButtonDown(1))
        {       
            DeselectBuilding();           
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            if(isPanelOnscreen)
            {
                panelMover.MoveOffScreen();
                isPanelOnscreen=false;
            }
            else 
            {
                panelMover.MoveOnScreen();
                isPanelOnscreen=true;
            }
            
        }
    }
}
