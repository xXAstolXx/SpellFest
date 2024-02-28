using UnityEngine;
using UnityEngine.Events;


[CreateAssetMenu(fileName = "GlobalFloat", menuName = "Create global variable/Float")]
public class GlobalFloat : ScriptableObject
{
    [SerializeField]
    private float value;
    public float Value
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
