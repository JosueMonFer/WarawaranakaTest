using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TarjetaMapaSimple : MonoBehaviour
{
    [Header("Sprites del mapa")]
    public Sprite imagenGris;
    public Sprite imagenColor;

    [Header("Referencias")]
    public Image imagenMapa;
    public TextMeshProUGUI textoNombre;
    public Button botonSeleccionar;
    public TextMeshProUGUI textoBoton;

    [Header("Datos")]
    public string nombreMapa;
    public int indiceMapa;

    private ControladorSeleccionMapa controlador;
    private bool estaSeleccionado = false;

    public void InicializarTarjeta(ControladorSeleccionMapa ctrl)
    {
        controlador = ctrl;

        if (botonSeleccionar != null)
        {
            botonSeleccionar.onClick.RemoveAllListeners();
            botonSeleccionar.onClick.AddListener(AlClickearSeleccionar);
        }
        else
        {
            Debug.LogError("El boton 'botonSeleccionar' no esta asignado en la tarjeta: " + nombreMapa);
        }

        DesactivarSeleccion();
    }

    public void AlClickearSeleccionar()
    {
        ControladorSonidos.ObtenerInstancia()?.SonidoSeleccionMapa();
        
        if (controlador != null)
        {
            controlador.SeleccionarMapa(this);
        }
    }

    public void ActivarSeleccion()
    {
        estaSeleccionado = true;

        if (imagenMapa != null && imagenColor != null)
            imagenMapa.sprite = imagenColor;

        if (textoNombre != null)
            textoNombre.color = Color.white;

        if (textoBoton != null)
            textoBoton.text = "Seleccionado";

        if (botonSeleccionar != null)
            botonSeleccionar.GetComponent<Image>().color = new Color(0.3f, 0.7f, 0.3f, 1f);
    }

    public void DesactivarSeleccion()
    {
        estaSeleccionado = false;

        if (imagenMapa != null && imagenGris != null)
            imagenMapa.sprite = imagenGris;

        if (textoNombre != null)
            textoNombre.color = new Color(0.7f, 0.7f, 0.7f, 1f);

        if (textoBoton != null)
            textoBoton.text = "Seleccionar";

        if (botonSeleccionar != null)
            botonSeleccionar.GetComponent<Image>().color = new Color(0.3f, 0.3f, 0.3f, 1f);
    }

    public string ObtenerNombre()
    {
        return nombreMapa;
    }

    public int ObtenerIndiceMapa()
    {
        return indiceMapa;
    }
}