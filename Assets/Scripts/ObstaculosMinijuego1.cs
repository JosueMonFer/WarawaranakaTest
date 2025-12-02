using UnityEngine;

public class ObstaculosMinijuego1 : MonoBehaviour
{
    [Header("Configuración de Tamaño")]
    public float tamañoMinimo = 0.5f;
    public float tamañoMaximo = 1.5f;

    [Header("Configuración de Velocidad")]
    public float velocidadMinima = 50f;
    public float velocidadMaxima = 150f;
    public float velocidadGiroMaxima = 10f;

    private Rigidbody2D rb;

    void Start()
    {
        float tamañoAleatorio = Random.Range(tamañoMinimo, tamañoMaximo);
        transform.localScale = new Vector3(tamañoAleatorio, tamañoAleatorio, 1);

        rb = GetComponent<Rigidbody2D>();

        float velocidadAleatoria = Random.Range(velocidadMinima, velocidadMaxima) / tamañoAleatorio;
        Vector2 direccionAleatoria = Random.insideUnitCircle.normalized;
        rb.AddForce(direccionAleatoria * velocidadAleatoria);

        float torqueAleatorio = Random.Range(-velocidadGiroMaxima, velocidadGiroMaxima);
        rb.AddTorque(torqueAleatorio);
    }
}