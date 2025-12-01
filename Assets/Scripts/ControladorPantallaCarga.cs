using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image; // Especificar que usamos Image de Unity

public class ControladorPantallaCarga : MonoBehaviour
{
    [Header("Imagen de Carga")]
    public Image imagenCarga; // La imagen que mostraste

    [Header("Texto de Carga")]
    public TextMeshProUGUI textoCargando;

    [Header("Configuracion")]
    public float tiempoCarga = 3f; // Tiempo que durará la pantalla (en segundos)
    public string escenaDestino = "EscenaPelea";

    [Header("Mensajes de Carga (Aleatorios)")]
    public string[] mensajesCarga = new string[]
    {
        "CARGANDO JUEGO...",
        "PREPARANDO BATALLA...",
        "LISTO PARA PELEAR...",
        "CARGANDO ARENA...",
        "PREPARATE PARA LUCHAR..."
    };

    void Start()
    {
        // Esperar un momento para que el sonido del botón se reproduzca
        StartCoroutine(IniciarCargaConRetraso());
    }

    IEnumerator IniciarCargaConRetraso()
    {
        // Esperar 0.5 segundos para que el sonido del botón se reproduzca completamente
        yield return new WaitForSeconds(4f);

        // Ahora sí, detener toda la música (pero el efecto de sonido ya se reprodujo)
        DetenerTodasLasMusicas();

        // Seleccionar mensaje aleatorio
        if (textoCargando != null && mensajesCarga.Length > 0)
        {
            string mensajeAleatorio = mensajesCarga[Random.Range(0, mensajesCarga.Length)];
            textoCargando.text = mensajeAleatorio;
        }

        // Iniciar la carga
        StartCoroutine(CargarConProgreso());
    }

    void DetenerTodasLasMusicas()
    {
        // Detener música del menú
        ControladorMusica musicaMenu = ControladorMusica.ObtenerInstancia();
        if (musicaMenu != null)
        {
            musicaMenu.DetenerMusica();
            Debug.Log("Música del menú detenida");
        }

        // Detener música de pelea si existe (por si acaso)
        ControladorMusicaPelea musicaPelea = ControladorMusicaPelea.ObtenerInstancia();
        if (musicaPelea != null)
        {
            musicaPelea.DetenerMusica();
            Debug.Log("Música de pelea detenida");
        }

        // Silenciar todo el audio temporalmente
        AudioListener.volume = 0f;

        Debug.Log("Todo el audio ha sido silenciado para la pantalla de carga");
    }

    // Método alternativo: con barra de progreso visual
    IEnumerator CargarConProgreso()
    {
        float tiempoTranscurrido = 0f;

        while (tiempoTranscurrido < tiempoCarga)
        {
            tiempoTranscurrido += Time.deltaTime;
            float progreso = tiempoTranscurrido / tiempoCarga;

            // Actualizar texto con porcentaje
            if (textoCargando != null)
            {
                int porcentaje = Mathf.RoundToInt(progreso * 100);
                textoCargando.text = $"CARGANDO... {porcentaje}%";
            }

            yield return null;
        }

        // Reactivar el audio
        AudioListener.volume = 1f;

        // Cargar escena
        SceneManager.LoadScene(escenaDestino);
    }
}