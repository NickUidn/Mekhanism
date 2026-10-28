using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderUiPanel : MonoBehaviour
{
    [SerializeField] ModuleCycleManager manager;
    public List<GameObject> orderObjects = new List<GameObject>();
    [SerializeField] GameObject content;

    private Coroutine reorderRoutine;
    private System.Collections.IEnumerator Start()
    {
        yield return null; 
        ReorderOrdeUI();
    }

    private void OnEnable()
    {
        reorderRoutine = StartCoroutine(ReorderLoop());
    }

    private void OnDisable()
    {
        if (reorderRoutine != null)
        {
            StopCoroutine(reorderRoutine);
            reorderRoutine = null;
        }
    }

    private IEnumerator ReorderLoop()
    {
        var wait = new WaitForSeconds(1f);
        while (true)
        {
            yield return wait;
            ReorderOrdeUI();
        }
    }

    public void ReorderOrdeUI()
    {
        if (content == null || manager == null) return;

        // 1. Collect live children
        orderObjects.Clear();
        foreach (Transform child in content.transform)
        {
            if (child == null) continue;
            orderObjects.Add(child.gameObject);
        }

        // 2. Build a list of (targetIndex, transform), skipping invalid entries
        var pairs = new List<(int target, Transform t)>();
        foreach (var go in orderObjects)
        {
            if (go == null) continue;
            var ui = go.GetComponent<ModuleOrderUi>();
            if (ui == null || ui.module == null) continue;

            int index = manager.modules.IndexOf(ui.module);
            if (index < 0) continue;

            pairs.Add((index, go.transform));
        }

        // 3. Apply in target-index order (0, 1, 2, ...)
        pairs.Sort((a, b) => a.target.CompareTo(b.target));
        foreach (var (target, t) in pairs)
        {
            if (t == null) continue;
            t.SetSiblingIndex(target);
        }
    }
}