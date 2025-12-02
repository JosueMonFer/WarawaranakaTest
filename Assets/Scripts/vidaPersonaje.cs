using UnityEngine;
using UnityEngine.UI;

public class vidaPersonaje : MonoBehaviour
{

    [Header("TEXTO")]
    public Text texto;
    public ControlPlayer scriptPlaye;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        texto = GetComponent<Text>();
        scriptPlaye = GetComponent<ControlPlayer>();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log($"VIDA {scriptPlaye.vidaActual}");
        texto.text = scriptPlaye.vidaActual.ToString() + " / 100";
    }
}
