using UnityEngine;

public class Particle : MonoBehaviour
{
    public Color positiveColor;
    public Color negativeColor;

    public float charge = 1f; // positive or negative
    public float mass = 1f;

    public Vector3 velocity;
    private Vector3 acceleration;

    private Rigidbody2D rb2d;
    private SpriteRenderer spriteRenderer;

    private float previousCharge; // for checking when the charge is changed

    void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();   
        spriteRenderer = GetComponent<SpriteRenderer>(); 
    }

    void Start()
    {
        rb2d = gameObject.GetComponent<Rigidbody2D>();
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        previousCharge = charge;

        if (charge > 0)
            spriteRenderer.color = positiveColor;
        else if (charge < 0)
            spriteRenderer.color = negativeColor;
    }

    public void ApplyForce(Vector3 force)
    {
        acceleration += force / mass;
    }

    public void CustomUpdate(float deltaTime)
    {
        velocity = acceleration * deltaTime;
        rb2d.linearVelocity += new Vector2(velocity.x, velocity.y);
        acceleration = Vector3.zero; // reset for next frame

        if (previousCharge == charge)
            return;

        if (charge > 0)
            spriteRenderer.color = positiveColor;
        else if (charge < 0)
            spriteRenderer.color = negativeColor;
    }
}
