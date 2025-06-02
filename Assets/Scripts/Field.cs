using UnityEngine;

public class Field : MonoBehaviour
{
    public Particle particle;

    private float previousCharge; //to check the previous charge

    void Start()
    {
        previousCharge = particle.charge;
        UpdateFieldScale();
    }

    void Update()
    {
        if (particle.charge != previousCharge)
        {
            UpdateFieldScale();
            previousCharge = particle.charge;
        }
    }

    void UpdateFieldScale()
    {
        float electricField = Mathf.Abs(particle.charge);
        transform.localScale = Vector3.one + new Vector3(electricField, electricField, electricField);
    }
}
