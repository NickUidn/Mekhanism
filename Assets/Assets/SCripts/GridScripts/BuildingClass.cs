using System;
using UnityEngine;

public class BuildingClass: MonoBehaviour
{
    [SerializeField] GameObject SelectedSpriteGO;

    public static event Action<BuildingClass> NewSelectedOnMap;

    private void OnEnable() => NewSelectedOnMap += HandleSelectionChanged;
    private void OnDisable() => NewSelectedOnMap -= HandleSelectionChanged;

    public void SelectedOnMap()
    {
        NewSelectedOnMap?.Invoke(this);
    }

    public void DeSelectedOnMap()
    {
        SetSelectedVisual(false);
    }

    private void HandleSelectionChanged(BuildingClass selected)
    {
        SetSelectedVisual(selected == this);
    }

    private void SetSelectedVisual(bool selected)
    {
        if (SelectedSpriteGO != null)
            SelectedSpriteGO.SetActive(selected);
    }

private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            DeSelectedOnMap();
        }
    }
}
