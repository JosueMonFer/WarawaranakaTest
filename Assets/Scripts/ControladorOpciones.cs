using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ControladorOpciones : MonoBehaviour
{
    [Header("Panel de Opciones")]
    public GameObject panelOpciones;

    [Header("Controles de Audio")]
    public Slider sliderVolumenMusica;
    public TextMeshProUGUI textoVolumenMusica;

    public Slider sliderVolumenEfectos;
    public TextMeshProUGUI textoVolumenEfectos;

    public Slider sliderVolumenMaestro;
    public TextMeshProUGUI textoVolumenMaestro;

    [Header("Botones")]
    public Button botonAplicar;
    public Button botonRestablecer;
    public Button botonCerrar;

    void Start()
    {
        if (panelOpciones != null)
        {
            panelOpciones.SetActive(false);
        }

        if (sliderVolumenMusica != null)
            sliderVolumenMusica.onValueChanged.AddListener(CambiarVolumenMusica);

        if (sliderVolumenEfectos != null)
            sliderVolumenEfectos.onValueChanged.AddListener(CambiarVolumenEfectos);

        if (sliderVolumenMaestro != null)
            sliderVolumenMaestro.onValueChanged.AddListener(CambiarVolumenMaestro);

        if (botonAplicar != null)
            botonAplicar.onClick.AddListener(AplicarCambios);

        if (botonRestablecer != null)
            botonRestablecer.onClick.AddListener(RestablecerValoresPorDefecto);

        if (botonCerrar != null)
            botonCerrar.onClick.AddListener(CerrarOpciones);

        CargarConfiguracion();
    }

    public void AbrirOpciones()
    {
        if (panelOpciones != null)
        {
            panelOpciones.SetActive(true);
            CargarConfiguracion();
        }
    }

    public void CerrarOpciones()
    {
        ControladorSonidos.ObtenerInstancia()?.SonidoBotonOpciones();

        if (panelOpciones != null)
        {
            panelOpciones.SetActive(false);
        }
    }

    void CambiarVolumenMusica(float valor)
    {
        if (textoVolumenMusica != null)
            textoVolumenMusica.text = Mathf.RoundToInt(valor * 100) + "%";

        // Actualizar música del menú
        ControladorMusica musicaMenu = ControladorMusica.ObtenerInstancia();
        if (musicaMenu != null)
        {
            float volumenMaestro = sliderVolumenMaestro != null ? sliderVolumenMaestro.value : 1f;
            musicaMenu.CambiarVolumen(valor * volumenMaestro);
        }

        // Actualizar música de pelea (si existe)
        ControladorMusicaPelea musicaPelea = ControladorMusicaPelea.ObtenerInstancia();
        if (musicaPelea != null)
        {
            float volumenMaestro = sliderVolumenMaestro != null ? sliderVolumenMaestro.value : 1f;
            musicaPelea.CambiarVolumen(valor);
            musicaPelea.ActualizarConVolumenMaestro(volumenMaestro);
        }
    }

    void CambiarVolumenEfectos(float valor)
    {
        if (textoVolumenEfectos != null)
            textoVolumenEfectos.text = Mathf.RoundToInt(valor * 100) + "%";

        ControladorSonidos soundManager = ControladorSonidos.ObtenerInstancia();
        if (soundManager != null)
        {
            float volumenMaestro = sliderVolumenMaestro != null ? sliderVolumenMaestro.value : 1f;
            soundManager.CambiarVolumenEfectos(valor * volumenMaestro);
        }
    }

    void CambiarVolumenMaestro(float valor)
    {
        if (textoVolumenMaestro != null)
            textoVolumenMaestro.text = Mathf.RoundToInt(valor * 100) + "%";

        AudioListener.volume = valor;

        // Actualizar todos los volúmenes con el nuevo volumen maestro
        if (sliderVolumenMusica != null)
            CambiarVolumenMusica(sliderVolumenMusica.value);
        if (sliderVolumenEfectos != null)
            CambiarVolumenEfectos(sliderVolumenEfectos.value);
    }

    public void AplicarCambios()
    {
        ControladorSonidos.ObtenerInstancia()?.SonidoBotonOpciones();

        if (sliderVolumenMusica != null)
            PlayerPrefs.SetFloat("VolumenMusica", sliderVolumenMusica.value);

        if (sliderVolumenEfectos != null)
            PlayerPrefs.SetFloat("VolumenEfectos", sliderVolumenEfectos.value);

        if (sliderVolumenMaestro != null)
            PlayerPrefs.SetFloat("VolumenMaestro", sliderVolumenMaestro.value);

        PlayerPrefs.Save();

        Debug.Log("Configuracion de audio guardada");
    }

    void CargarConfiguracion()
    {
        float volumenMusica = PlayerPrefs.GetFloat("VolumenMusica", 0.5f);
        float volumenEfectos = PlayerPrefs.GetFloat("VolumenEfectos", 0.7f);
        float volumenMaestro = PlayerPrefs.GetFloat("VolumenMaestro", 1f);

        if (sliderVolumenMusica != null)
        {
            sliderVolumenMusica.value = volumenMusica;
        }

        if (sliderVolumenEfectos != null)
        {
            sliderVolumenEfectos.value = volumenEfectos;
        }

        if (sliderVolumenMaestro != null)
        {
            sliderVolumenMaestro.value = volumenMaestro;
        }
    }

    public void RestablecerValoresPorDefecto()
    {
        ControladorSonidos.ObtenerInstancia()?.SonidoBotonOpciones();

        if (sliderVolumenMusica != null)
            sliderVolumenMusica.value = 0.5f;

        if (sliderVolumenEfectos != null)
            sliderVolumenEfectos.value = 0.7f;

        if (sliderVolumenMaestro != null)
            sliderVolumenMaestro.value = 1f;

        Debug.Log("Valores de audio restablecidos");
    }
}