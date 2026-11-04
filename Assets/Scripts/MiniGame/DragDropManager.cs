
using System.Collections.Generic;
using UnityEngine;

public class DragDropManager : MonoBehaviour
{
    [Header("Piezas del rompecabezas")]
    [SerializeField] private UIDraggable[] piezas;

    [Header("Panel de victoria")]
    [SerializeField] private GameObject panelVictoria;

    private HashSet<UIDraggable> piezasRegistradas =
        new HashSet<UIDraggable>();

    private HashSet<UIDraggable> piezasCompletadas =
        new HashSet<UIDraggable>();

    private bool juegoCompletado;

    private void Start()
    {
        // Ocultar panel de victoria al comenzar
        if (panelVictoria != null)
            panelVictoria.SetActive(false);

        // Registrar las piezas del minijuego
        foreach (UIDraggable pieza in piezas)
        {
            if (pieza == null)
            {
                Debug.LogWarning("Hay una pieza sin asignar.");
                continue;
            }

            // Evitar registrar dos veces la misma pieza
            if (!piezasRegistradas.Add(pieza))
                continue;

            // Asignar administrador y bloquear botón
            pieza.Configurar(this);
        }

        if (piezasRegistradas.Count == 0)
        {
            Debug.LogError(
                "DragDropManager: No hay piezas asignadas."
            );
        }
        else
        {
            Debug.Log(
                "Minijuego iniciado. Total de piezas: " +
                piezasRegistradas.Count
            );
        }
    }

    public void RegistrarPiezaColocada(UIDraggable pieza)
    {
        if (juegoCompletado || pieza == null)
            return;

        // Comprobar que la pieza pertenece al minijuego
        if (!piezasRegistradas.Contains(pieza))
            return;

        // Evitar contar dos veces una misma pieza
        if (!piezasCompletadas.Add(pieza))
            return;

        Debug.Log(
            "Piezas colocadas: " +
            piezasCompletadas.Count +
            " / " +
            piezasRegistradas.Count
        );

        // Comprobar condición de victoria
        if (piezasCompletadas.Count == piezasRegistradas.Count)
        {
            Ganar();
        }
    }

    public void Ganar()
    {
        if (juegoCompletado)
            return;

        // No permitir ganar si faltan piezas
        if (piezasRegistradas.Count == 0 ||
            piezasCompletadas.Count != piezasRegistradas.Count)
            return;

        juegoCompletado = true;

        // Activar panel de victoria
        if (panelVictoria != null)
            panelVictoria.SetActive(true);

        // Habilitar los botones de todas las piezas
        foreach (UIDraggable pieza in piezasRegistradas)
        {
            pieza.DesbloquearBoton();
        }

        Debug.Log("¡MINIJUEGO COMPLETADO!");
    }
}
