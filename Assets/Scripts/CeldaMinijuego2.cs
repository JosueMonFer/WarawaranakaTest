using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CeldaMinijuego2 : MonoBehaviour
{
    public int cellIndex;
    private Image symbolImage;
    public Sprite xSprite;
    public Sprite oSprite;

    void Awake()
    {
        // Obtener la imagen del símbolo (el hijo de la celda)
        symbolImage = transform.GetChild(0).GetComponent<Image>();
        if (symbolImage != null)
        {
            symbolImage.gameObject.SetActive(false);
        }
    }

    public void SetSymbol(string player)
    {
        if (symbolImage != null)
        {
            symbolImage.gameObject.SetActive(true);

            if (player == "X")
            {
                symbolImage.sprite = xSprite;
                symbolImage.color = Color.red;
            }
            else
            {
                symbolImage.sprite = oSprite;
                symbolImage.color = Color.blue;
            }
        }
    }

    public void ClearSymbol()
    {
        if (symbolImage != null)
        {
            symbolImage.gameObject.SetActive(false);
            symbolImage.sprite = null;
        }
    }
}