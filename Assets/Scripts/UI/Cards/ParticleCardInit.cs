using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ParticleCardInit : MonoBehaviour
{
    public static ParticleCardInit Instance;

    public GameObject cardPrefab;
    public GameObject particlePrefab;

    public Transform cardPanelParent;
    public Transform particleParent;
    public Button addCardButton;

    [HideInInspector] public Particle latestParticle;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        addCardButton.onClick.AddListener(GenerateCardandParticle);
    }

    private void GenerateCardandParticle()
    {
        GameObject particleObj = Instantiate(particlePrefab, Random.insideUnitCircle * 2f, Quaternion.identity, particleParent);
        Particle particle = particleObj.GetComponent<Particle>();
        ParticleManager.Instance.particles.Add(particle);

        latestParticle = particle; 

        GameObject newCard = Instantiate(cardPrefab, cardPanelParent);
        newCard.transform.SetAsLastSibling();
        newCard.transform.localScale = Vector3.one;

        int cardIndex = cardPanelParent.childCount;
        TextMeshProUGUI label = newCard.transform.Find("P#").GetComponent<TextMeshProUGUI>();
        label.text = $"P{cardIndex}";

        ParticleUI particleUI = newCard.GetComponent<ParticleUI>();
        particleUI.Bind(particle);
    }
}
