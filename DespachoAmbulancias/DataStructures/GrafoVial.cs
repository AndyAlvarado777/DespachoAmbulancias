using System;
using System.Collections.Generic;
using DespachoAmbulancias.Models;

namespace DespachoAmbulancias.DataStructures;

/// <summary>
/// Representa la red vial urbana mediante un Grafo Dirigido y Ponderado \( G = (V, E) \).
/// <para>
/// <b>Justificación Teórica (Materia PED):</b>
/// Implementado mediante <b>Listas de Adyacencia</b> sobre tablas hash (<c>Dictionary&lt;string, List&lt;Tramo&gt;&gt;</c>).
/// Es la representación óptima para redes de transporte urbano (grafos dispersos), requiriendo un
/// costo espacial de \( \mathcal{O}(V + E) \) frente a los \( \mathcal{O}(V^2) \) de una matriz de adyacencia.
/// </para>
/// <para>
/// <b>Estructura a cargo de:</b> Marvin González (adaptada e integrada al modelo de dominio por Andy Alvarado).
/// </para>
/// </summary>
public class GrafoVial
{
    private readonly Dictionary<string, NodoVial> _nodos;
    private readonly Dictionary<string, List<Tramo>> _listaAdyacencia;

    /// <summary>
    /// Inicializa un grafo vial vacío con diccionarios de acceso \( \mathcal{O}(1) \).
    /// </summary>
    public GrafoVial()
    {
        _nodos = new Dictionary<string, NodoVial>(StringComparer.OrdinalIgnoreCase);
        _listaAdyacencia = new Dictionary<string, List<Tramo>>(StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Total de vértices o intersecciones registradas en la red vial (\( |V| \)).
    /// </summary>
    public int CantidadNodos => _nodos.Count;

    /// <summary>
    /// Total de aristas o tramos dirigidos registrados en la red vial (\( |E| \)).
    /// </summary>
    public int CantidadTramos
    {
        get
        {
            int total = 0;
            foreach (var lista in _listaAdyacencia.Values)
            {
                total += lista.Count;
            }
            return total;
        }
    }

    /// <summary>
    /// Agrega un nuevo vértice al mapa vial si no existe previamente.
    /// Complejidad: \( \mathcal{O}(1) \) promedio.
    /// </summary>
    /// <param name="nodo">Instancia del nodo vial a registrar.</param>
    public void AgregarNodo(NodoVial nodo)
    {
        if (nodo == null) throw new ArgumentNullException(nameof(nodo));

        if (!_nodos.ContainsKey(nodo.Id))
        {
            _nodos.Add(nodo.Id, nodo);
            _listaAdyacencia.Add(nodo.Id, new List<Tramo>());
        }
    }

    /// <summary>
    /// Registra un tramo vial dirigido con su tiempo de viaje asociado.
    /// Complejidad: \( \mathcal{O}(1) \) promedio.
    /// </summary>
    /// <param name="origenId">Id del nodo de partida.</param>
    /// <param name="destinoId">Id del nodo de llegada.</param>
    /// <param name="tiempoMinutos">Tiempo estimado de recorrido en minutos (peso \( w \ge 0 \)).</param>
    public void AgregarTramo(string origenId, string destinoId, int tiempoMinutos)
    {
        if (string.IsNullOrWhiteSpace(origenId)) throw new ArgumentException("Origen no válido.", nameof(origenId));
        if (string.IsNullOrWhiteSpace(destinoId)) throw new ArgumentException("Destino no válido.", nameof(destinoId));

        origenId = origenId.Trim().ToUpperInvariant();
        destinoId = destinoId.Trim().ToUpperInvariant();

        // Validar que ambos vértices existan en el grafo
        if (!_nodos.ContainsKey(origenId))
        {
            AgregarNodo(new NodoVial(origenId, $"Intersección {origenId}", TipoNodo.Interseccion));
        }

        if (!_nodos.ContainsKey(destinoId))
        {
            AgregarNodo(new NodoVial(destinoId, $"Intersección {destinoId}", TipoNodo.Interseccion));
        }

        Tramo nuevoTramo = new(origenId, destinoId, tiempoMinutos);
        _listaAdyacencia[origenId].Add(nuevoTramo);
    }

    /// <summary>
    /// Registra una calle de doble sentido agregando dos aristas dirigidas opuestas.
    /// </summary>
    public void AgregarTramoDobleSentido(string nodoA, string nodoB, int tiempoMinutos)
    {
        AgregarTramo(nodoA, nodoB, tiempoMinutos);
        AgregarTramo(nodoB, nodoA, tiempoMinutos);
    }

    /// <summary>
    /// Obtiene las aristas o calles que salen directamente de un nodo.
    /// Complejidad: \( \mathcal{O}(1) \) para acceder a la lista.
    /// </summary>
    /// <param name="idNodo">Identificador del nodo consultado.</param>
    /// <returns>Lista de tramos adyacentes salientes.</returns>
    public List<Tramo> ObtenerVecinos(string idNodo)
    {
        if (!string.IsNullOrWhiteSpace(idNodo) && _listaAdyacencia.TryGetValue(idNodo.Trim(), out var vecinos))
        {
            return vecinos;
        }
        return new List<Tramo>();
    }

    /// <summary>
    /// Retorna la colección completa de nodos registrados.
    /// </summary>
    public Dictionary<string, NodoVial> GetNodos() => _nodos;

    /// <summary>
    /// Verifica si un nodo existe en el grafo.
    /// </summary>
    public bool ContieneNodo(string idNodo) => !string.IsNullOrWhiteSpace(idNodo) && _nodos.ContainsKey(idNodo.Trim());

    /// <summary>
    /// Obtiene un nodo por su identificador.
    /// </summary>
    public NodoVial? ObtenerNodo(string idNodo)
    {
        if (!string.IsNullOrWhiteSpace(idNodo) && _nodos.TryGetValue(idNodo.Trim(), out var nodo))
        {
            return nodo;
        }
        return null;
    }

    /// <summary>
    /// Construye y retorna el <b>Grafo Transpuesto (\( G^T \))</b>, invirtiendo la dirección
    /// de todas las aristas dirigidas conservando los mismos pesos.
    /// <para>
    /// <b>Aplicación en el Despacho:</b>
    /// Permite ejecutar el algoritmo de Dijkstra desde el lugar del incidente hacia atrás,
    /// calculando en una sola ejecución el tiempo real que le tomaría a cada ambulancia
    /// disponible llegar al paciente respetando el sentido de las vías.
    /// </para>
    /// Complejidad: \( \mathcal{O}(V + E) \) tiempo y espacio.
    /// </summary>
    /// <returns>Nueva instancia de <see cref="GrafoVial"/> con aristas invertidas.</returns>
    public GrafoVial ObtenerGrafoTranspuesto()
    {
        GrafoVial grafoT = new();

        // 1. Clonar todos los vértices
        foreach (var nodo in _nodos.Values)
        {
            grafoT.AgregarNodo(new NodoVial(nodo.Id, nodo.Nombre, nodo.Tipo));
        }

        // 2. Invertir cada arista (u -> v pasa a ser v -> u)
        foreach (var origen in _listaAdyacencia.Keys)
        {
            foreach (var tramo in _listaAdyacencia[origen])
            {
                grafoT.AgregarTramo(tramo.DestinoId, tramo.OrigenId, tramo.TiempoMinutos);
            }
        }

        return grafoT;
    }

    /// <summary>
    /// Genera la red vial inicial documentada en la Fase 1 del proyecto (San Salvador).
    /// </summary>
    public static GrafoVial CrearMapaEjemplo()
    {
        GrafoVial grafo = new();

        // Vértices clave
        grafo.AgregarNodo(new NodoVial("BASE-A1", "Base Cruz Roja San Salvador", TipoNodo.BaseAmbulancia));
        grafo.AgregarNodo(new NodoVial("BASE-A2", "Base Comandos de Salvamento", TipoNodo.BaseAmbulancia));
        grafo.AgregarNodo(new NodoVial("HOSP-01", "Hospital Nacional Rosales", TipoNodo.Hospital));
        grafo.AgregarNodo(new NodoVial("HOSP-02", "Hospital Médico Quirúrgico ISSS", TipoNodo.Hospital));
        grafo.AgregarNodo(new NodoVial("X1", "Alameda Roosevelt y 49 Av. Sur", TipoNodo.Interseccion));
        grafo.AgregarNodo(new NodoVial("X2", "Bulevar Los Héroes y Calle Gabriela Mistral", TipoNodo.Interseccion));
        grafo.AgregarNodo(new NodoVial("X3", "Paseo General Escalón y Fuentes Beethoven", TipoNodo.Interseccion));
        grafo.AgregarNodo(new NodoVial("INC-01", "Monumento al Salvador del Mundo", TipoNodo.PuntoInteres));

        // Tramos viales con tiempos en minutos
        grafo.AgregarTramoDobleSentido("BASE-A1", "X1", 4);
        grafo.AgregarTramoDobleSentido("BASE-A2", "X3", 2);
        grafo.AgregarTramoDobleSentido("X1", "INC-01", 3);
        grafo.AgregarTramoDobleSentido("X1", "X2", 5);
        grafo.AgregarTramoDobleSentido("X2", "HOSP-01", 4);
        grafo.AgregarTramoDobleSentido("X3", "INC-01", 6);
        grafo.AgregarTramoDobleSentido("X3", "HOSP-02", 5);

        return grafo;
    }
}
