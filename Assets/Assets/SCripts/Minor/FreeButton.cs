using UnityEngine;
using UnityEngine.Events;
public class FreeButton : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public UnityEvent OnFreeButtonClick;

    void Awake()
    {
        
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    void OnMouseDown()
    {      
        OnFreeButtonClick.Invoke();
    }
}
