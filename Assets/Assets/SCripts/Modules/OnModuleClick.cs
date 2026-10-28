using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class OnModuleClick : MonoBehaviour
{
    Collider2D myCollider;
    [SerializeField]ModuleUIManager uiManager;    
    BuildingClass myParentModule;

    private void Awake()
    {
        myCollider = GetComponent<Collider2D>();       
    }
    private void Start()
    {
        myParentModule = GetComponentInParent<BuildingClass>();
         uiManager =FindAnyObjectByType<ModuleUIManager>();
    }
    private bool _pressedOnMe;

    private void OnMouseDown() => _pressedOnMe = true;
    private void OnMouseUp()
    {
        if (!_pressedOnMe) return;
        _pressedOnMe = false;
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;
        CallmanagerOnscreen();
    }
    private void OnMouseExit() => _pressedOnMe = false;

    void CallmanagerOnscreen()
    {
        if (uiManager.currentModule != null)
        {
            uiManager?.DeselectModue();
        }
        myParentModule?.SelectedOnMap();
        uiManager.currentModule = myParentModule;
        uiManager.MatchUiAndModule();
        uiManager.panelMover.MoveOnScreen();        
    }
}
