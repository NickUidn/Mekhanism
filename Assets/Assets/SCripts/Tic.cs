using UnityEngine;

public class Tic : MonoBehaviour
{
    public int ticIndex;
    [SerializeField] StaticWave staticWave;
    public Tic previousTic;
    public Tic nextTic;

    private float _amplitude;
    private float _frequency;

    public float defaultFreqValue;
    public float defaultAmpValue;

    // Symmetric amplitude range: Amp_MIN_VALUE is always -Amp_MAX_VALUE.
   static float Amp_MAX_VALUE = 6f;
    static float Amp_MIN_VALUE => -Amp_MAX_VALUE;

  static float Freq_MIN_VALUE = 0f;
  static float Freq_MAX_VALUE = 3f;

    // External input range (e.g. slider)
    public const int INPUT_MIN = 1;
    public const int INPUT_MAX = 100;

    [Header("Smooth Transition")]
    private static float smoothDuration = 0.5f;
    [SerializeField] private AnimationCurve smoothCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    private Coroutine _ampRoutine;
    private Coroutine _freqRoutine;

    public float amplitude
    {
        get => _amplitude;
        set
        {
            float clampedValue = Mathf.Clamp(value, Amp_MIN_VALUE, Amp_MAX_VALUE);

            // Enforce the sign rule based on tic parity
            if (ticIndex % 2 != 0) // odd
                _amplitude = -Mathf.Abs(clampedValue);
            else
                _amplitude = Mathf.Abs(clampedValue);

            if (staticWave != null)
                staticWave.amplitude = _amplitude;
        }
    }

    public float frequency
    {
        get => _frequency;
        set
        {
            _frequency = Mathf.Clamp(value, Freq_MIN_VALUE, Freq_MAX_VALUE);
            if (staticWave != null)
                staticWave.frequency = _frequency;
        }
    }

    private void Awake()
    {
        staticWave = GetComponent<StaticWave>();
    }

    private void Start()
    {
        float initialAmp = staticWave.amplitude;
        amplitude = initialAmp;
        frequency = staticWave.frequency;
    }

    // ---------------- Mapping helpers ----------------

    private static float NormalizeInput(int value)
    {
        int v = Mathf.Clamp(value, INPUT_MIN, INPUT_MAX);
        return (v - INPUT_MIN) / (float)(INPUT_MAX - INPUT_MIN); // 0..1
    }

    // Because Amp_MIN_VALUE == -Amp_MAX_VALUE, the max magnitude is just Amp_MAX_VALUE.
    private static float MaxAmplitudeMagnitude => Mathf.Abs(Amp_MAX_VALUE);

    private static float MapAmplitude(int value)
    {
        // Magnitude only (0..Amp_MAX_VALUE). Sign is applied by the setter via ticIndex parity.
        return Mathf.Lerp(0f, MaxAmplitudeMagnitude, NormalizeInput(value));
    }

    private static float MapFrequency(int value)
    {
        return Mathf.Lerp(Freq_MIN_VALUE, Freq_MAX_VALUE, NormalizeInput(value));
    }

    // ---------------- Public API (input: int 1..100) ----------------

    public void SetAmplitude(int amplitudeValue)
    {
        float mapped = MapAmplitude(amplitudeValue);

        if (!Application.isPlaying || smoothDuration <= 0f)
        {
            amplitude = mapped;
            return;
        }

        if (_ampRoutine != null) StopCoroutine(_ampRoutine);
        _ampRoutine = StartCoroutine(SmoothAmplitudeRoutine(mapped));
    }

    public void SetFrequency(int frequencyValue)
    {
        float mapped = MapFrequency(frequencyValue);

        if (!Application.isPlaying || smoothDuration <= 0f)
        {
            frequency = mapped;
            return;
        }

        if (_freqRoutine != null) StopCoroutine(_freqRoutine);
        _freqRoutine = StartCoroutine(SmoothFrequencyRoutine(mapped));
    }

    // ---------------- Public readback (returns int 1..100) ----------------

    public int GetNormalizedAmplitude()
    {
        if (Mathf.Approximately(Amp_MAX_VALUE, 0f)) return INPUT_MIN; // guard divide-by-zero

        float mag = Mathf.Abs(_amplitude); // 0 .. Amp_MAX_VALUE
        float t01 = Mathf.Clamp01(mag / Amp_MAX_VALUE); // 0..1
        return Mathf.RoundToInt(t01 * (INPUT_MAX - INPUT_MIN) + INPUT_MIN);
    }

    public int GetNormalizedFrequency()
    {
        float denom = Freq_MAX_VALUE - Freq_MIN_VALUE;
        if (Mathf.Approximately(denom, 0f)) return INPUT_MIN; // guard divide-by-zero

        float t01 = Mathf.InverseLerp(Freq_MIN_VALUE, Freq_MAX_VALUE, _frequency); // 0..1, auto-clamped
        return Mathf.RoundToInt(t01 * (INPUT_MAX - INPUT_MIN) + INPUT_MIN);
    }

    // ---------------- Coroutines ----------------

    private System.Collections.IEnumerator SmoothAmplitudeRoutine(float target)
    {
        float start = _amplitude; // signed value
        float clamped = Mathf.Clamp(target, Amp_MIN_VALUE, Amp_MAX_VALUE);
        float end = (ticIndex % 2 != 0) ? -Mathf.Abs(clamped) : clamped;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / smoothDuration;
            float eased = smoothCurve.Evaluate(Mathf.Clamp01(t));
            float v = Mathf.LerpUnclamped(start, end, eased);
            _amplitude = v;
            if (staticWave != null) staticWave.amplitude = _amplitude;
            yield return null;
        }

        // Snap to final via the property setter so invariants run once at the end
        amplitude = target;
        _ampRoutine = null;
    }

    private System.Collections.IEnumerator SmoothFrequencyRoutine(float target)
    {
        float start = _frequency;
        float end = Mathf.Clamp(target, Freq_MIN_VALUE, Freq_MAX_VALUE);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / smoothDuration;
            float eased = smoothCurve.Evaluate(Mathf.Clamp01(t));
            _frequency = Mathf.LerpUnclamped(start, end, eased);
            if (staticWave != null) staticWave.frequency = _frequency;
            yield return null;
        }

        frequency = target;
        _freqRoutine = null;
    }

    private void OnDisable()
    {
        if (_ampRoutine != null) { StopCoroutine(_ampRoutine); _ampRoutine = null; }
        if (_freqRoutine != null) { StopCoroutine(_freqRoutine); _freqRoutine = null; }
    }
}