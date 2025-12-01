using UnityEngine;

public class ControladorMusica : MonoBehaviour
{
    private static ControladorMusica instancia;

    [Header("Audio")]
    public AudioClip musicaMenu;

    private AudioSource audioSource;

    void Awake()
    {
        // Patrón Singleton: solo puede existir una instancia
        if (instancia == null)
        {
            instancia = this;
            DontDestroyOnLoad(gameObject); // No destruir al cambiar de escena

            // Configurar AudioSource
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = musicaMenu;
            audioSource.loop = true;
            audioSource.playOnAwake = false;
            audioSource.volume = 0.5f; // Volumen al 50%

            // Iniciar música
            audioSource.Play();
        }
        else
        {
            // Si ya existe una instancia, destruir este objeto duplicado
            Destroy(gameObject);
        }
    }

    // Método para cambiar el volumen (útil para opciones)
    public void CambiarVolumen(float nuevoVolumen)
    {
        if (audioSource != null)
        {
            audioSource.volume = Mathf.Clamp01(nuevoVolumen);
        }
    }

    // Método para detener la música (cuando inicies combate)
    public void DetenerMusica()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    // Método para reanudar la música
    public void ReanudarMusica()
    {
        if (audioSource != null && !audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

    // Método para obtener la instancia desde otros scripts
    public static ControladorMusica ObtenerInstancia()
    {
        return instancia;
    }
}