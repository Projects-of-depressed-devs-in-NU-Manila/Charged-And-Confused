using UnityEngine;

public class Field : MonoBehaviour
{
    public Particle particle; // Assign this in the Inspector
    
    void Start()
    {
        {
            float absCharge = Mathf.Abs(particle.charge);
            transform.localScale += new Vector3 (absCharge, absCharge, absCharge);
        }
    }
}