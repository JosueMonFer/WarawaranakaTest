using UnityEngine;

public class EstrellaMinijuego1 : MonoBehaviour
{
    [Header("Configuración de Caída")]
    public float velocidadCaida = 3f;
    public float velocidadRotacion = 100f;

    void Update()
    {
        // Mover la estrella hacia abajo
        transform.Translate(Vector3.down * velocidadCaida * Time.deltaTime);

        // Rotar la estrella para efecto visual
        transform.Rotate(Vector3.forward * velocidadRotacion * Time.deltaTime);

        // Destruir si sale de la pantalla (por debajo)
        if (transform.position.y < -10f)
        {
            Destroy(gameObject);
        }
    }
}