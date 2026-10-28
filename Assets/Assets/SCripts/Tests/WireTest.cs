using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(LineRenderer))]
public class WireTest : MonoBehaviour
{
    [Header("Endpoints")]
    [Tooltip("Anchors the wire passes through, in order. Needs at least 2.")]
    [SerializeField] private List<Transform> points = new List<Transform>();

    [Header("Shape")]
    [Tooltip("How many segments per span. More = smoother, slower.")]
    [Range(2, 100)]
    [SerializeField] private int segmentsPerSpan = 10;

    [Tooltip("How far the middle of each span dips below the straight line.")]
    [SerializeField] private float sag = 1.5f;

    [Tooltip("Higher = tighter rope (less sag relative to length).")]
    [SerializeField] private float tension = 1f;

    [Tooltip("Uses the true catenary formula instead of a quadratic approximation.")]
    [SerializeField] private bool useTrueCatenary = false;

    [Tooltip("Scale sag by span length so short spans dip less. Recommended for uneven spacing.")]
    [SerializeField] private bool scaleSagWithLength = true;

    [Tooltip("Span length that produces the full 'sag' value. Shorter spans dip less, longer spans dip more (clamped).")]
    [SerializeField] private float referenceSpanLength = 10f;

    [Header("Dynamic")]
    [Tooltip("Update the rope every frame. Disable if anchors never move.")]
    [SerializeField] private bool updateEveryFrame = true;

    private LineRenderer _lr;

    private void Awake()
    {
        _lr = GetComponent<LineRenderer>();
    }

    private void Start()
    {
        Redraw();
    }

    private void Update()
    {
        if (updateEveryFrame)
            Redraw();
    }

    // -------------------------------------------------------------------
    //  Public API
    // -------------------------------------------------------------------

    /// <summary>Rebuilds the whole wire from the current anchor positions.</summary>
    public void Redraw()
    {
        if (points == null || points.Count < 2)
        {
            _lr.positionCount = 0;
            return;
        }

        // Count valid anchors (skip nulls)
        int validCount = 0;
        for (int i = 0; i < points.Count; i++)
            if (points[i] != null) validCount++;

        if (validCount < 2)
        {
            _lr.positionCount = 0;
            return;
        }

        int spanCount = validCount - 1;
        int totalPoints = spanCount * segmentsPerSpan + 1;
        _lr.positionCount = totalPoints;

        int writeIndex = 0;

        for (int s = 0; s < spanCount; s++)
        {
            Transform a = GetValidPoint(s);
            Transform b = GetValidPoint(s + 1);

            Vector3 p0 = a.position;
            Vector3 p1 = b.position;

            float spanSag = GetSagForSpan(p0, p1);

            // For every segment in this span, write one point.
            // The last point of each span (t = 1) is skipped because it equals
            // the first point of the next span — except for the very last span,
            // where we DO write t = 1.
            int steps = segmentsPerSpan;
            for (int i = 0; i < steps; i++)
            {
                float t = i / (float)steps;
                Vector3 pos = useTrueCatenary
                    ? CatenaryPoint(p0, p1, t, spanSag)
                    : QuadraticPoint(p0, p1, t, spanSag);

                _lr.SetPosition(writeIndex++, pos);
            }

            // If this is the last span, also write its endpoint.
            if (s == spanCount - 1)
            {
                Vector3 endPos = useTrueCatenary
                    ? CatenaryPoint(p0, p1, 1f, spanSag)
                    : QuadraticPoint(p0, p1, 1f, spanSag);

                _lr.SetPosition(writeIndex++, endPos);
            }
        }
    }

    /// <summary>
    /// Returns the world position along the entire multi-span wire at
    /// normalized parameter t (0 = first anchor, 1 = last anchor).
    /// Handy for moving objects along the wire.
    /// </summary>
    public Vector3 GetPointAt(float t)
    {
        if (points == null || points.Count < 2) return Vector3.zero;
        t = Mathf.Clamp01(t);

        int validCount = 0;
        for (int i = 0; i < points.Count; i++)
            if (points[i] != null) validCount++;

        if (validCount < 2) return Vector3.zero;

        int spanCount = validCount - 1;

        // Which span does t fall into? (spans are treated as equal length in t-space)
        float scaled = t * spanCount;
        int spanIndex = Mathf.Min(Mathf.FloorToInt(scaled), spanCount - 1);
        float localT = scaled - spanIndex;

        Transform a = GetValidPoint(spanIndex);
        Transform b = GetValidPoint(spanIndex + 1);

        Vector3 p0 = a.position;
        Vector3 p1 = b.position;
        float spanSag = GetSagForSpan(p0, p1);

        return useTrueCatenary
            ? CatenaryPoint(p0, p1, localT, spanSag)
            : QuadraticPoint(p0, p1, localT, spanSag);
    }

    public void SetPoints(List<Transform> newPoints)
    {
        points = newPoints ?? new List<Transform>();
        Redraw();
    }

    // -------------------------------------------------------------------
    //  Internals
    // -------------------------------------------------------------------

    private Transform GetValidPoint(int index)
    {
        int count = 0;
        for (int i = 0; i < points.Count; i++)
        {
            if (points[i] == null) continue;
            if (count == index) return points[i];
            count++;
        }
        return null;
    }

    private float GetSagForSpan(Vector3 p0, Vector3 p1)
    {
        if (!scaleSagWithLength) return sag;

        float dist = Vector3.Distance(p0, p1);
        float factor = referenceSpanLength > 0.0001f
            ? dist / referenceSpanLength
            : 1f;

        // Clamp so very long spans don't sag absurdly
        factor = Mathf.Clamp(factor, 0.1f, 3f);
        return sag * factor;
    }

    private Vector3 QuadraticPoint(Vector3 p0, Vector3 p1, float t, float spanSag)
    {
        Vector3 line = Vector3.Lerp(p0, p1, t);
        float dip = 4f * t * (1f - t) * spanSag;
        return line - Vector3.up * dip;
    }

    private Vector3 CatenaryPoint(Vector3 p0, Vector3 p1, float t, float spanSag)
    {
        float a = Mathf.Max(0.001f, tension);
        float x = (t - 0.5f) * 2f * (Vector3.Distance(p0, p1) * 0.5f);
        float coshX = (Mathf.Exp(x / a) + Mathf.Exp(-x / a)) * 0.5f;
        float dip = (coshX - 1f) * a;

        // Scale the dip so 'sag' still controls the visible depth
        // (catenary naturally produces different depths for different spans)
        float norm = Mathf.Max(0.0001f, (float)Math.Cosh(Vector3.Distance(p0, p1) / (2f * a)) - 1f);
        dip = dip / norm * spanSag;

        Vector3 line = Vector3.Lerp(p0, p1, t);
        return line - Vector3.up * dip;
    }
}