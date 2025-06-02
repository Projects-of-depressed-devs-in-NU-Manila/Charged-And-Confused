using UnityEngine;
using UnityEngine.UI;

public class FieldToggle : MonoBehaviour
{
    public Button toggleFieldButtonOff;
    public Button toggleFieldButtonOn;

    private void Start()
    {
        toggleFieldButtonOff.onClick.AddListener(() => SetAllFieldsActive(false));
        toggleFieldButtonOn.onClick.AddListener(() => SetAllFieldsActive(true));
    }

    private void SetAllFieldsActive(bool isActive)
    {
        foreach (Particle particle in ParticleManager.Instance.particles)
        {
            if (particle != null)
            {
                Field field = particle.GetComponentInChildren<Field>(true);
                if (field != null)
                {
                    field.gameObject.SetActive(isActive);
                }
            }
        }
        Debug.Log($"All fields turned {(isActive ? "on" : "off")}.");
    }
}
