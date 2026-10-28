using System.Collections;
using UnityEngine;

public class PanelMover : MonoBehaviour
{
    [Header("References")]
    [SerializeField] RectTransform LinkedPanel;

    [Header("Positions")]
    [SerializeField] Vector2 OnScreenPosition;
    [SerializeField] Vector2 OffScreenPosition;

    [Header("Animation")]
    [Tooltip("How long the transition takes, in seconds.")]
    [SerializeField] float moveDuration = 0.35f;

    [Tooltip("Higher = more pronounced easing. 1 = linear. Typical values: 2–5.")]
    [Range(1f, 8f)]
    [SerializeField] float onScreenEasePower = 3f;   // used for MoveOnScreen  (fast → slow)

    [Range(1f, 8f)]
    [SerializeField] float offScreenEasePower = 3f;  // used for MoveOffScreen (slow → fast)

    [Tooltip("If true, OnScreen uses fast→slow easing and OffScreen uses slow→fast.")]
    [SerializeField] bool useEasing = true;

    private Coroutine activeMove;


    private void Start()
    {
        if (LinkedPanel == null)LinkedPanel = GetComponent<RectTransform>();
       LinkedPanel.anchoredPosition = OffScreenPosition;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            MoveOffScreen();
        }
    }

    public void MoveOnScreen()
    {
        // Starts fast, ends slow → "Easy in" feel
        StartMove(OnScreenPosition, easeOutFeel: true, easePower: onScreenEasePower);
    }

    public void MoveOffScreen()
    {
        // Starts slow, ends fast → "Easy out" feel
        StartMove(OffScreenPosition, easeOutFeel: false, easePower: offScreenEasePower);
    }

    private void StartMove(Vector2 target, bool easeOutFeel, float easePower)
    {
        if (activeMove != null)
            StopCoroutine(activeMove);

        activeMove = StartCoroutine(MoveRoutine(target, easeOutFeel, easePower));
    }

    private IEnumerator MoveRoutine(Vector2 target, bool easeOutFeel, float easePower)
    {
        Vector2 start = LinkedPanel.anchoredPosition;
        float elapsed = 0f;

        while (elapsed < moveDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / moveDuration);

            float eased;
            if (!useEasing)
            {
                eased = t; // linear
            }
            else if (easeOutFeel)
            {
                // Starts fast → ends slow : 1 - (1-t)^power
                eased = 1f - Mathf.Pow(1f - t, easePower);
            }
            else
            {
                // Starts slow → ends fast : t^power
                eased = Mathf.Pow(t, easePower);
            }

            LinkedPanel.anchoredPosition = Vector2.LerpUnclamped(start, target, eased);
            yield return null;
        }

        LinkedPanel.anchoredPosition = target;
        activeMove = null;
    }
}