using System;
using UnityEngine;

public class DeactivatedTargetImages : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Lista de imágenes/objetos que se pueden desactivar.")]
    private GameObject[] imageListTracking;

    /// <summary>
    /// Desactiva un objeto específico de la lista según su índice.
    /// </summary>
    /// <param name="index">El número de posición en el arreglo (empezando desde 0).</param>
    public void DeactivateTrackingImage(int index)
    {
        try
        {
            // 1. Verificación preventiva (Defensive Programming)
            // Corregido: Usamos 'imageListTracking' que es el nombre real de tu variable
            if (imageListTracking == null)
            {
                Debug.LogError("El arreglo 'imageListTracking' es nulo. Asígnalo en el Inspector.");
                return;
            }

            // 2. Verificación de rango
            if (index >= 0 && index < imageListTracking.Length)
            {
                // 3. Verificación de que el objeto individual no sea nulo
                if (imageListTracking[index] != null)
                {
                    imageListTracking[index].SetActive(false);
                    Debug.Log($"Objeto en el índice {index} desactivado con éxito.");
                }
                else
                {
                    Debug.LogWarning(
                        $"El objeto en el índice {index} está vacío (null) en el Inspector."
                    );
                }
            }
            else
            {
                // Si el índice no es válido, lanzamos una excepción manualmente para capturarla abajo
                throw new IndexOutOfRangeException(
                    $"El índice {index} está fuera de los límites del arreglo (Tamaño: {imageListTracking.Length})."
                );
            }
        }
        catch (IndexOutOfRangeException ex)
        {
            // Captura errores de índice específicamente
            Debug.LogError($"[Error de Índice]: {ex.Message}");
        }
        catch (Exception ex)
        {
            // Captura cualquier otro error inesperado (memoria, sistema, etc.)
            Debug.LogError($"[Error Inesperado]: {ex.GetType()} - {ex.Message}");
        }
    }
}
