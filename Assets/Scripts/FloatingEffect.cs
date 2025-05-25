using UnityEngine;
using Random = UnityEngine.Random;

public class FloatingEffect : MonoBehaviour
{
    [Header("Floating Settings")]
    public float floatSpeed = 1f;
    public float floatHeight = 1f;
    public bool randomizeEffect = true;

    [HideInInspector] public Vector3 startPosition;
    [HideInInspector] public float randomOffset;

    void OnEnable()
    {
        startPosition = transform.position;
        randomOffset = randomizeEffect ? Random.Range(0f, 2f * Mathf.PI) : 0f;
    }

    void Update()
    {
        float newY = startPosition.y + Mathf.Sin(Time.time * floatSpeed + randomOffset) * floatHeight;
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);
        transform.Rotate(0f, 0f, 10f * Time.deltaTime);
    }

    
}

