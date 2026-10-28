using System;
using System.Collections.Generic;
using UnityEngine;

public class BuildingGhost : MonoBehaviour
{
    [SerializeField] private BuildUiManager manager;
    [SerializeField] private GridMain gridMain;                 // your grid reference
    [SerializeField] private Dictionary<GameObject, GameObject> ghosts = new();

    [SerializeField] private float followSpeed = 15f;       // how fast the lerp is

    private GameObject _activeGhost;                        // currently visible ghost

    private void OnEnable()
    {
        if (manager != null)
            manager.OnBuildingModeChanged += HandleBuildingModeChanged;
    }

    private void OnDisable()
    {
        if (manager != null)
            manager.OnBuildingModeChanged -= HandleBuildingModeChanged;
    }

    private void Start()
    {
        foreach (var ghost in ghosts.Values)
            if (ghost != null)
                ghost.SetActive(false);

        _activeGhost = null;
    }

    private void HandleBuildingModeChanged(bool isOn)
    {
        if (isOn)
        {
            _activeGhost = null;

            foreach (var kvp in ghosts)
            {
                GameObject source = kvp.Key;
                GameObject ghost = kvp.Value;

                if (ghost == null) continue;

                bool matches = source == manager.selectedBuildingPrefab;
                ghost.SetActive(matches);

                if (matches)
                    _activeGhost = ghost;
            }
        }
        else
        {
            foreach (var ghost in ghosts.Values)
                if (ghost != null)
                    ghost.SetActive(false);

            _activeGhost = null;
        }
    }

    private void Update()
    {
        if (_activeGhost == null || gridMain == null) return;

        // Mouse world position (z = 0 for 2D / top-down)
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0f;

        // Convert to grid cell coordinates
        gridMain.grid.GetXY(mouseWorld, out int x, out int y);

        // Snap to the CENTER of that cell
        Vector3 cellCenter = gridMain.grid.GetWolrdPosition(x, y);
        cellCenter.x += gridMain.grid.GetCellSize() * 0.5f;
        cellCenter.y += gridMain.grid.GetCellSize() * 0.5f;
        cellCenter.z = 0f;

        // Smoothly move the active ghost toward the cell center
        _activeGhost.transform.position = Vector3.Lerp(
            _activeGhost.transform.position,
            cellCenter,
            Time.deltaTime * followSpeed
        );
    }
}