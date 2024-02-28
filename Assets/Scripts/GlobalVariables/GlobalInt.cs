using UnityEngine;
using UnityEngine.Events;


[CreateAssetMenu(fileName = "GlobalInt", menuName = "Create global variable/Int")]
public class GlobalInt : ScriptableObject
{
    [SerializeField]
    private int value;
    public int Value
    {
        get => value;
        set
        {
            this.value = value;
            OnValueChanged.Invoke();
        }
    }

    public UnityEvent OnValueChanged { get; private set; } = new UnityEvent();
}
