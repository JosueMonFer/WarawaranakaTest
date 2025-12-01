using UnityEngine;

public class ControladorMusicaPelea : MonoBehaviour
{
    private static ControladorMusicaPelea instancia;

    [Header("Musica de Mapas")]
    public AudioClip musicaMapa0;
    public AudioClip musicaMapa1;
    public AudioClip musicaMapa2;
    public AudioClip musicaMapa3;

    [Header("Configuracion")]
    [Range(0f, 1f)]
    public float volumenMusica = 0.6f;

    private AudioSource audioSource;
    private float volumenBase = 0.6f; // Volumen inicial

    void Awake()
    {
        // Patrón Singleton
        if (instancia == null)
        {
            instancia = this;
            DontDestroyOnLoad(gameObject);

            // Configurar AudioSource
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.loop = true;
            audioSource.playOnAwake = false;

            // Cargar el volumen guardado de las opciones
            CargarVolumenDesdeOpciones();

            Debug.Log("ControladorMusicaPelea inicializado");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void CargarVolumenDesdeOpciones()
    {
        // Cargar los valores guardados en PlayerPrefs (del menú de opciones)
        float volumenMusica = PlayerPrefs.GetFloat("VolumenMusica", 0.5f);
        float volumenMaestro = PlayerPrefs.GetFloat("VolumenMaestro", 1f);

        // Aplicar el volumen combinado
        volumenBase = volumenMusica;
        audioSource.volume = volumenMusica * volumenMaestro;

        Debug.Log($"Volumen de música de pelea cargado: Música={volumenMusica}, Maestro={volumenMaestro}, Final={audioSource.volume}");
    }

    void Start()
    {
        ReproducirMusicaDelMapa();
    }

    void ReproducirMusicaDelMapa()
    {
        int indiceMapa = DatosJuego.indiceMapaSeleccionado;
        AudioClip musicaSeleccionada = null;

        Debug.Log($"Reproduciendo música para mapa índice: {indiceMapa}");

        switch (indiceMapa)
        {
            case 0:
                musicaSeleccionada = musicaMapa0;
                break;
            case 1:
                musicaSeleccionada = musicaMapa1;
                break;
            case 2:
                musicaSeleccionada = musicaMapa2;
                break;
            case 3:
                musicaSeleccionada = musicaMapa3;
                break;
            default:
                Debug.LogWarning($"Índice de mapa no válido: {indiceMapa}");
                break;
        }

        if (musicaSeleccionada != null)
        {
            audioSource.clip = musicaSeleccionada;
            audioSource.Play();
            Debug.Log($"Reproduciendo: {musicaSeleccionada.name}");
        }
        else
        {
            Debug.LogWarning($"No hay música asignada para el mapa {indiceMapa}");
        }
    }

    // Método para detener la música (cuando termine la pelea)
    public void DetenerMusica()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
            Debug.Log("Música de pelea detenida");
        }
    }

    // Método para pausar la música
    public void PausarMusica()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Pause();
        }
    }

    // Método para reanudar la música
    public void ReanudarMusica()
    {
        if (audioSource != null && !audioSource.isPlaying)
        {
            audioSource.UnPause();
        }
    }

    // Método para cambiar el volumen
    public void CambiarVolumen(float nuevoVolumen)
    {
        if (audioSource != null)
        {
            volumenBase = Mathf.Clamp01(nuevoVolumen);

            // Aplicar también el volumen maestro
            float volumenMaestro = PlayerPrefs.GetFloat("VolumenMaestro", 1f);
            audioSource.volume = volumenBase * volumenMaestro;

            Debug.Log($"Volumen de música de pelea actualizado: {audioSource.volume}");
        }
    }

    // Método para actualizar con el volumen maestro
    public void ActualizarConVolumenMaestro(float volumenMaestro)
    {
        if (audioSource != null)
        {
            audioSource.volume = volumenBase * volumenMaestro;
            Debug.Log($"Volumen maestro aplicado a música de pelea: {audioSource.volume}");
        }
    }

    // Método para obtener la instancia
    public static ControladorMusicaPelea ObtenerInstancia()
    {
        return instancia;
    }

    // Método para destruir la instancia (cuando termine la pelea)
    public void DestruirInstancia()
    {
        if (audioSource != null)
        {
            audioSource.Stop();
        }

        if (instancia == this)
        {
            instancia = null;
        }

        Destroy(gameObject);
    }
}