using UnityEngine;

public class ControladorSonidos : MonoBehaviour
{
    private static ControladorSonidos instancia;

    [Header("Efectos de Sonido - Menu Principal")]
    public AudioClip sonidoBotonComenzar;
    public AudioClip sonidoBotonAjustes;
    public AudioClip sonidoBotonSalir;
    public AudioClip sonidoBotonAtras;

    [Header("Efectos de Sonido - Panel Opciones")]
    public AudioClip sonidoBotonOpciones;

    [Header("Efectos de Sonido - Seleccion Personaje")]
    public AudioClip sonidoSeleccionPersonaje1;
    public AudioClip sonidoSeleccionPersonaje2;
    public AudioClip sonidoSeleccionPersonaje3;
    public AudioClip sonidoSeleccionPersonaje4;
    public AudioClip sonidoBotonListo;

    [Header("Efectos de Sonido - Seleccion Mapa")]
    public AudioClip sonidoSeleccionMapa;
    public AudioClip sonidoComenzarMapa1;
    public AudioClip sonidoComenzarMapa2;
    public AudioClip sonidoComenzarMapa3;
    public AudioClip sonidoComenzarMapa4;

    [Header("Configuracion")]
    [Range(0f, 1f)]
    public float volumenEfectos = 0.7f;

    private AudioSource audioSource;

    void Awake()
    {
        if (instancia == null)
        {
            instancia = this;
            DontDestroyOnLoad(gameObject);

            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.loop = false;
            audioSource.volume = volumenEfectos;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ReproducirSonido(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip, volumenEfectos);
        }
    }

    public void SonidoBotonComenzar()
    {
        ReproducirSonido(sonidoBotonComenzar);
    }

    public void SonidoBotonAjustes()
    {
        ReproducirSonido(sonidoBotonAjustes);
    }

    public void SonidoBotonSalir()
    {
        ReproducirSonido(sonidoBotonSalir);
    }

    public void SonidoBotonAtras()
    {
        ReproducirSonido(sonidoBotonAtras);
    }

    public void SonidoBotonOpciones()
    {
        ReproducirSonido(sonidoBotonOpciones);
    }

    public void SonidoSeleccionPersonaje(int indicePersonaje)
    {
        AudioClip sonido = null;

        if (indicePersonaje == 0)
        {
            sonido = sonidoSeleccionPersonaje1;
        }
        else if (indicePersonaje == 1)
        {
            sonido = sonidoSeleccionPersonaje2;
        }
        else if (indicePersonaje == 2)
        {
            sonido = sonidoSeleccionPersonaje3;
        }
        else if (indicePersonaje == 3)
        {
            sonido = sonidoSeleccionPersonaje4;
        }

        ReproducirSonido(sonido);
    }

    public void SonidoBotonListo()
    {
        ReproducirSonido(sonidoBotonListo);
    }

    public void SonidoSeleccionMapa()
    {
        ReproducirSonido(sonidoSeleccionMapa);
    }

    public void SonidoComenzarMapa(int indiceMapa)
    {
        AudioClip sonido = null;

        if (indiceMapa == 0)
        {
            sonido = sonidoComenzarMapa1;
        }
        else if (indiceMapa == 1)
        {
            sonido = sonidoComenzarMapa2;
        }
        else if (indiceMapa == 2)
        {
            sonido = sonidoComenzarMapa3;
        }
        else if (indiceMapa == 3)
        {
            sonido = sonidoComenzarMapa4;
        }

        ReproducirSonido(sonido);
    }

    public void CambiarVolumenEfectos(float nuevoVolumen)
    {
        volumenEfectos = Mathf.Clamp01(nuevoVolumen);
        if (audioSource != null)
        {
            audioSource.volume = volumenEfectos;
        }
    }

    public static ControladorSonidos ObtenerInstancia()
    {
        return instancia;
    }
}