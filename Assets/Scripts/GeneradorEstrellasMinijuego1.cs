using UnityEngine;

public class GeneradorEstrellasMinijuego1 : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject prefabEstrella;
    public GameObject prefabBomba;

    [Header("Configuracion de Generacion")]
    public float tiempoEntreGeneraciones = 1f;
    public float probabilidadBomba = 0.15f; // 15% de probabilidad de generar una bomba

    [Header("Limites de Generacion")]
    public float limiteIzquierdo = -8f;
    public float limiteDerecho = 8f;
    public float alturaGeneracion = 6f;

    private float tiempoSiguienteGeneracion = 0f;
    private bool generando = true;

    void Update()
    {
        if (!generando) return;

        if (Time.time >= tiempoSiguienteGeneracion)
        {
            GenerarObjeto();
            tiempoSiguienteGeneracion = Time.time + tiempoEntreGeneraciones;
        }
    }

    void GenerarObjeto()
    {
        // Decidir si generar estrella o bomba
        GameObject prefabAGenerar;
        if (Random.value < probabilidadBomba && prefabBomba != null)
        {
            prefabAGenerar = prefabBomba;
        }
        else
        {
            prefabAGenerar = prefabEstrella;
        }

        if (prefabAGenerar == null)
        {
            Debug.LogError("No hay prefab asignado!");
            return;
        }

        // Generar en posición aleatoria
        float posX = Random.Range(limiteIzquierdo, limiteDerecho);
        Vector3 posicion = new Vector3(posX, alturaGeneracion, 0);

        Instantiate(prefabAGenerar, posicion, Quaternion.identity);
    }

    public void AumentarDificultad()
    {
        // Reducir el tiempo entre generaciones (más rápido)
        tiempoEntreGeneraciones = Mathf.Max(0.3f, tiempoEntreGeneraciones - 0.05f);
    }

    public void DetenerGeneracion()
    {
        generando = false;
    }

    // Visualizar área de generación en el editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 izquierda = new Vector3(limiteIzquierdo, alturaGeneracion, 0);
        Vector3 derecha = new Vector3(limiteDerecho, alturaGeneracion, 0);
        Gizmos.DrawLine(izquierda, derecha);
    }
}