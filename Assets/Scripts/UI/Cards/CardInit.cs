using UnityEngine;
using UnityEngine.UI;

public class CardInit : MonoBehaviour
{
    public GameObject cardPrefab;
    public Transform cardPanelParent; 
    public Button addCardButton;

    void Start()
    {
        addCardButton.onClick.AddListener(GenerateCard);
    }

    private void GenerateCard()
    {
        Debug.Log("Generating card...");

        GameObject newCard = Instantiate(cardPrefab, cardPanelParent);
        newCard.transform.SetAsLastSibling();
        newCard.transform.localScale = Vector3.one;
    }

}
