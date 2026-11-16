using System.Collections.Generic;
using UnityEngine;

public class ContractSegment : MonoBehaviour
{
    public int positionInSequance;
    public ContractClass contract;
    public GameObject body;

    [Tooltip("Optional transform to use as the snap point. Falls back to this GameObject.")]
    public Transform snapReference;

    [Tooltip("Radius used when searching for a tick collider on release.")]
    public float releaseSearchRadius = 0.15f;

    [Tooltip("Bounds are shrunk by this much before the overlap test, so segments that only touch edges don't count as overlapping.")]
    public float overlapShrink = 0.02f;

    private Camera cam;
    private Canvas canvas;
    private Vector3 originalBodyPosition;
    private bool isDragging;
    private Vector3 dragOffset;
    public int dropedOnThisSegment;

    // Registry of every live segment, so we can test against other contracts' segments
    private static readonly HashSet<ContractSegment> AllSegments = new HashSet<ContractSegment>();

    private SpriteRenderer _spriteRenderer;
    private SpriteRenderer spriteRenderer
    {
        get
        {
            if (_spriteRenderer == null)
                _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            return _spriteRenderer;
        }
    }

    void OnEnable() { AllSegments.Add(this); }
    void OnDisable() { AllSegments.Remove(this); }

    void Start()
    {
        canvas = GetComponentInParent<Canvas>();
        cam = (canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay && canvas.worldCamera != null)
            ? canvas.worldCamera
            : Camera.main;

        originalBodyPosition = body.transform.position;

        Physics2D.queriesHitTriggers = true;
    }

    void Update()
    {
        if (!isDragging)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 p = ScreenToWorld(Input.mousePosition);

                // Explicit pick: accept any collider under the cursor that belongs to THIS segment.
                foreach (var h in Physics2D.OverlapPointAll(p))
                {
                    if (h == null) continue;
                    if (h.transform == transform || h.transform.IsChildOf(transform))
                    {
                        dragOffset = body.transform.position - p;
                        isDragging = true;
                        break;
                    }
                }
            }
            return;
        }

        // Drag: move body, as before.
        Vector3 pointerWorld = ScreenToWorld(Input.mousePosition);
        body.transform.position = pointerWorld + dragOffset;

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
            HandleRelease(pointerWorld);
        }
    }

    private void HandleRelease(Vector3 pointerWorld)
    {
        Physics2D.queriesHitTriggers = true;
        Collider2D[] hits = Physics2D.OverlapCircleAll(pointerWorld, releaseSearchRadius);

        Collider2D targetCol = null;
        int droppedTic = -1;
        foreach (var h in hits)
        {
            if (h == null) continue;
            if (h.transform == transform || h.transform.IsChildOf(transform)) continue;
            if (IsTickTag(h.gameObject.tag, out droppedTic)) { targetCol = h; break; }
        }

        if (targetCol != null)
        {
            Vector3 targetWorld = targetCol.transform.position;
            Vector3 snapWorldNow = snapReference != null ? snapReference.position : transform.position;
            Vector3 delta = targetWorld - snapWorldNow;

            body.transform.position += delta;

            // Dropped on top of another contract -> don't snap, go back to spawn
            if (OverlapsOtherContract())
            {
                ReturnToSpawn();
                return;
            }

            // Propagate drop info to this segment and all following segments.
            ApplyDropToNeighbors(droppedTic);

            contract.isContractActive = true;
        }
        else
        {
            ReturnToSpawn();
        }
    }

    /// <summary>
    /// Clears drop data, deactivates the contract and moves it back to where it spawned.
    /// </summary>
    private void ReturnToSpawn()
    {
        ClearDropFromNeighbors();
        contract.isContractActive = false;
        body.transform.position = originalBodyPosition;
    }

    /// <summary>
    /// True if any segment of THIS contract overlaps any segment of a DIFFERENT contract.
    /// Uses sprite bounds, which update immediately when the body is moved.
    /// </summary>
    private bool OverlapsOtherContract()
    {
        if (contract == null) return false;

        foreach (var mine in contract.segments)
        {
            if (mine == null || mine.spriteRenderer == null) continue;

            Bounds myBounds = mine.spriteRenderer.bounds;
            myBounds.Expand(-overlapShrink);

            foreach (var other in AllSegments)
            {
                if (other == null || other == mine) continue;
                if (other.contract == contract) continue;      // same contract, ignore
                if (other.spriteRenderer == null) continue;

                Bounds otherBounds = other.spriteRenderer.bounds;
                otherBounds.Expand(-overlapShrink);

                // 2D game: ignore Z by putting both bounds on the same Z plane
                Bounds flatMine = myBounds;
                flatMine.center = new Vector3(myBounds.center.x, myBounds.center.y, otherBounds.center.z);

                if (flatMine.Intersects(otherBounds))
                    return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Writes the tic index into contractBody for this segment and every segment
    /// after it (by positionInSequance). The tic index advances by 1 per segment.
    /// </summary>
    private void ApplyDropToNeighbors(int droppedTic)
    {
        if (contract == null) return;

        foreach (var seg in contract.segments)
        {
            if (seg == null) continue;

            int offset = seg.positionInSequance - this.positionInSequance;
            int newTic = droppedTic + offset;
            int bodyIndex = seg.positionInSequance - 1;

            if (bodyIndex < 0 || bodyIndex >= contract.contractBody.Count) continue;

            if (newTic < 1 || newTic > 10)
            {
                seg.dropedOnThisSegment = 0;
                contract.contractBody[bodyIndex].targetTicIndex = -1;
                continue;
            }

            seg.dropedOnThisSegment = newTic;
            contract.contractBody[bodyIndex].targetTicIndex = newTic - 1;
        }
    }

    /// <summary>
    /// Clears drop info on this segment and every following segment.
    /// </summary>
    private void ClearDropFromNeighbors()
    {
        if (contract == null) return;

        foreach (var seg in contract.segments)
        {
            if (seg == null) continue;

            seg.dropedOnThisSegment = 0;

            int bodyIndex = seg.positionInSequance - 1;
            if (bodyIndex >= 0 && bodyIndex < contract.contractBody.Count)
                contract.contractBody[bodyIndex].targetTicIndex = -1;
        }
    }

    Vector3 ScreenToWorld(Vector3 screenPos)
    {
        if (cam == null) cam = Camera.main;
        float z = cam != null ? cam.nearClipPlane : 0f;
        Vector3 sp = screenPos;
        sp.z = z;
        Vector3 w = cam.ScreenToWorldPoint(sp);
        w.z = body.transform.position.z;
        return w;
    }

    bool IsTickTag(string tag, out int segment)
    {
        segment = -1;

        if (string.IsNullOrEmpty(tag) || tag.Length < 4) return false;
        if (!tag.StartsWith("tic")) return false;

        char last = tag[tag.Length - 1];
        if (last < '0' || last > '9') return false;

        switch (last)
        {
            case '0': segment = 1; break;
            case '1': segment = 2; break;
            case '2': segment = 3; break;
            case '3': segment = 4; break;
            case '4': segment = 5; break;
            case '5': segment = 6; break;
            case '6': segment = 7; break;
            case '7': segment = 8; break;
            case '8': segment = 9; break;
            case '9': segment = 10; break;
            default: return false;
        }

        if (segment + (contract.contractLength - positionInSequance) > 10) { segment = -1; return false; }
        if (segment - positionInSequance < 0) { segment = -1; return false; }

        return true;
    }
}