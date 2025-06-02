using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class MenuIntroBall : MonoBehaviour
{
    [Header("Ball Reference")]
    public Transform positiveChargeBall; 
    public Transform negativeChargeBall; 

    [Header("Animation Settings")]
    public float startDelay = 0.5f;
    public float approachDuration = 1.0f; 
    public float bounceDuration = 0.5f;
    public float floatAmplitude = 0.3f;
    public float floatSpeed = 1f;

    
    private readonly Vector3 leftCollisionPos = new Vector3(3.2f, 2f, 0f);
    private readonly Vector3 rightCollisionPos = new Vector3(5f, 1.5f, 0f);
    private readonly Vector3 leftFinalPos = new Vector3(2f, 2f, 0f);
    private readonly Vector3 rightFinalPos = new Vector3(8f, 1.5f, 0f);
    private readonly Vector3 leftStartPos = new Vector3(-15f, 2f, 0f);
    private readonly Vector3 rightStartPos = new Vector3(15f, 1.5f, 0f);

    private float timer;
    private bool isAnimating;
    private bool hasCollided;
    private bool isBouncing;

    void Start()
    {
        ResetAnimation();
        Invoke(nameof(StartAnimation), startDelay);
        
        AddFloatingEffectComponents();
    }

    void AddFloatingEffectComponents()
    {
        if (!negativeChargeBall.GetComponent<FloatingEffect>()) negativeChargeBall.gameObject.AddComponent<FloatingEffect>();
        if (!positiveChargeBall.GetComponent<FloatingEffect>()) positiveChargeBall.gameObject.AddComponent<FloatingEffect>();
    }

    void ResetAnimation()
    {
        negativeChargeBall.position = leftStartPos;
        positiveChargeBall.position = rightStartPos;
        timer = 0f;
        isAnimating = false;
        hasCollided = false;
        isBouncing = false;
        
        var leftFloat = negativeChargeBall.GetComponent<FloatingEffect>();
        var rightFloat = positiveChargeBall.GetComponent<FloatingEffect>();
        
        if (leftFloat != null)
        {
            leftFloat.enabled = false;
            leftFloat.floatHeight = floatAmplitude;
            leftFloat.floatSpeed = floatSpeed;
        }
        
        if (rightFloat != null)
        {
            rightFloat.enabled = false;
            rightFloat.floatHeight = floatAmplitude;
            rightFloat.floatSpeed = floatSpeed;
        }
    }

    void StartAnimation()
    {
        isAnimating = true;
    }

    void Update()
    {
        if (!isAnimating) return;

        timer += Time.deltaTime;

        if (!hasCollided)
        {
            
            float approachProgress = Mathf.Clamp01(timer / approachDuration);
            negativeChargeBall.position = Vector3.Lerp(leftStartPos, leftCollisionPos, approachProgress);
            positiveChargeBall.position = Vector3.Lerp(rightStartPos, rightCollisionPos, approachProgress);

            if (approachProgress >= 1f)
            {
                OnBallCollide();
                hasCollided = true;
                timer = 0f;
            }
        }
        else if (!isBouncing)
        {
            
            float bounceProgress = Mathf.Clamp01(timer / bounceDuration);
            negativeChargeBall.position = Vector3.Lerp(leftCollisionPos, leftFinalPos, bounceProgress);
            positiveChargeBall.position = Vector3.Lerp(rightCollisionPos, rightFinalPos, bounceProgress);

            if (bounceProgress >= 1f)
            {
                isBouncing = true;
                isAnimating = false;
                EnableFloatingEffect();
            }
        }
    }

    void OnBallCollide()
    {
        Debug.Log("Balls collided!");
        negativeChargeBall.position = leftCollisionPos;
        positiveChargeBall.position = rightCollisionPos;
    }

    void EnableFloatingEffect()
    {
        StartCoroutine(StartFloatingWithDelay(0.1f));
    }

    IEnumerator StartFloatingWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        
        var leftFloat = negativeChargeBall.GetComponent<FloatingEffect>();
        var rightFloat = positiveChargeBall.GetComponent<FloatingEffect>();
        
        if (leftFloat != null)
        {
            leftFloat.enabled = true;
            leftFloat.randomOffset = Random.Range(0, 2f * Mathf.PI);
            leftFloat.startPosition = leftFinalPos; 
        }
        
        if (rightFloat != null)
        {
            rightFloat.enabled = true;
            rightFloat.randomOffset = Random.Range(0, 2f * Mathf.PI);
            rightFloat.startPosition = rightFinalPos;
        }
    }

    void DisableFloatingEffects()
    {
        var leftFloat = negativeChargeBall.GetComponent<FloatingEffect>();
        var rightFloat = positiveChargeBall.GetComponent<FloatingEffect>();
        
        if (leftFloat != null) leftFloat.enabled = false;
        if (rightFloat != null) rightFloat.enabled = false;
    }
}