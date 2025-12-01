using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TarjetaPersonaje : MonoBehaviour
{
    [Header("Sprites de la tarjeta")]
    public Sprite imagenGris;
    public Sprite imagenColor;

    [Header("Referencias")]
    public Image imagenTarjeta;
    public Button botonSeleccionar;
    public TextMeshProUGUI textoBoton;

    [Header("Datos")]
    public string nombrePersonaje;
    public int indicePersonaje; // 0, 1, 2... para identificar qué sonido reproducir

    private ControladorSeleccion controlador;
    private bool estaSeleccionado = false;

    public void InicializarTarjeta(ControladorSeleccion ctrl)
    {
        controlador = ctrl;
        botonSeleccionar.onClick.AddListener(AlClickearSeleccionar);
        DesactivarSeleccion();
    }

    public void AlClickearSeleccionar()
    {
        // Reproducir sonido específico del personaje
        ControladorSonidos.ObtenerInstancia()?.SonidoSeleccionPersonaje(indicePersonaje);
        
        controlador.SeleccionarPersonaje(this);
    }

    public void ActivarSeleccion()
    {
        estaSeleccionado = true;
        imagenTarjeta.sprite = imagenColor;
        textoBoton.text = "Seleccionado";
    }

    public void DesactivarSeleccion()
    {
        estaSeleccionado = false;
        imagenTarjeta.sprite = imagenGris;
        textoBoton.text = "Seleccionar";
    }

    public string ObtenerNombre()
    {
        return nombrePersonaje;
    }
}