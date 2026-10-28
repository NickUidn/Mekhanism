
using UnityEngine;
using CodeMonkey.Utils;
using UnityEngine.Diagnostics;

public class GridMain : MonoBehaviour
{
   public GridClass<GridCellData> grid; 
    [SerializeField]BuildUiManager buildUiManager;
    private void Start()
    {
       
        grid = new GridClass<GridCellData>(100, 100, 5f, new Vector3(-250, -250), CreateGridCellData);
      
    }
    public GridCellData CreateGridCellData ()
    {
        GridCellData celldata = new GridCellData();
        return  celldata;
    }
    public class GridCellData 
    {
      public bool isBlocking=false;
        public bool isUnitOn = false;
        public bool isBuildingOn = false;
        public BuildingClass building;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (buildUiManager.isInBuildingMode)
            {
                
                Vector3 mousePos = UtilsClass.GetMouseWorldPosition();
                grid.GetXY(mousePos, out int x, out int y);

                Vector3 cellCenter = grid.GetWolrdPosition(x, y) + new Vector3(
                    grid.GetCellSize()*0.5f, 
                    grid.GetCellSize() *0.5f,
                    0f
                );
                GridCellData cell = grid.GetValue(x, y);
                if (cell.isBuildingOn == false)
                {
                    Instantiate(
                        buildUiManager.selectedBuildingPrefab,
                        cellCenter,
                        Quaternion.identity
                    );
                    buildUiManager.DeselectBuilding();
                    // Optionally update grid data              
                    cell.isBuildingOn = true;
                    cell.building = buildUiManager.selectedBuildingPrefab.GetComponentInChildren<BuildingClass>();
                    Debug.Log(cell.building);
                    grid.SetValue(x, y, cell);
                }
            }
        }

       
    }
        
}
