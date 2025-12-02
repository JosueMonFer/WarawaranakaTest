using UnityEngine;
using UnityEngine.SceneManagement;

public class ControladorJuegoMinijuego1 : MonoBehaviour
{
    [Header("Referencias de Jugadores")]
    public GameObject jugador1;
    public GameObject jugador2;

    [Header("Configuración")]
    public float tiempoParaReiniciar = 2f;

    private bool juegoTerminado = false;

    void Start()
    {
        Time.timeScale = 1f;
    }

    public void JugadorChoco(GameObject jugadorQueChoco)
    {
        if (juegoTerminado) return;

        juegoTerminado = true;

        if (jugadorQueChoco == jugador1)
        {
            Debug.Log("¡Jugador 2 Gana!");
        }
        else if (jugadorQueChoco == jugador2)
        {
            Debug.Log("¡Jugador 1 Gana!");
        }

        Invoke("ReiniciarJuego", tiempoParaReiniciar);
    }

    void ReiniciarJuego()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}