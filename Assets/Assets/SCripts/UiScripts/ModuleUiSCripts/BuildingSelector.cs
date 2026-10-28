using UnityEditor;
using UnityEngine;

public class BuildingSelector : MonoBehaviour
{
BuildUiManager buildUiManager;
BuildingClass Building;
[SerializeField] GameObject buildingPrefab;

    private void Start()
    {
        buildUiManager=GetComponentInParent<BuildUiManager>();
    }

    public void Selected()
    {
        buildUiManager.SelectedBuildingToBuild(Building, buildingPrefab);
    }
}
