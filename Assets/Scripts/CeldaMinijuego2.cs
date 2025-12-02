using UnityEngine;
using UnityEngine.UI;

public class CeldaMinijuego2 : MonoBehaviour
{
    public int cellIndex;
    private Button button;
    private Text symbolText;
    private bool isOccupied = false;

    void Start()
    {
        button = GetComponent<Button>();

        // Crear el texto para mostrar X u O
        GameObject textObj = new GameObject("SymbolText");
        textObj.transform.SetParent(transform);
        symbolText = textObj.AddComponent<Text>();
        symbolText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        symbolText.fontSize = 80;
        symbolText.alignment = TextAnchor.MiddleCenter;
        symbolText.color = Color.black;
        symbolText.text = "";

        RectTransform rectTransform = symbolText.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = Vector2.zero;
        rectTransform.sizeDelta = new Vector2(180, 180);

        button.onClick.AddListener(OnCellClicked);
    }

    void OnCellClicked()
    {
        if (!isOccupied && ControladorJuegoMinijuego2.instance != null) // CAMBIADO
        {
            ControladorJuegoMinijuego2.instance.MakeMove(cellIndex); // CAMBIADO
        }
    }

    public void SetSymbol(string symbol)
    {
        symbolText.text = symbol;
        isOccupied = true;
        button.interactable = false;
    }

    public void Reset()
    {
        symbolText.text = "";
        isOccupied = false;
        button.interactable = true;
    }

    public bool IsOccupied()
    {
        return isOccupied;
    }
}