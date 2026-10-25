using System;
using System.Collections.Generic;
using DespachoAmbulancias.Models;

namespace DespachoAmbulancias.DataStructures;


public class GrafoVial
{
    private readonly Dictionary<string, NodoVial> _nodos;
    private readonly Dictionary<string, List<Tramo>> _listaAdyacencia;

  
    public GrafoVial()
    {
        _nodos = new Dictionary<string, NodoVial>(StringComparer.OrdinalIgnoreCase);
        _listaAdyacencia = new Dictionary<string, List<Tramo>>(StringComparer.OrdinalIgnoreCase);
    }

  
    public int CantidadNodos => _nodos.Count;

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

  
    public void AgregarNodo(NodoVial nodo)
    {
        if (nodo == null) throw new ArgumentNullException(nameof(nodo));

        if (!_nodos.ContainsKey(nodo.Id))
        {
            _nodos.Add(nodo.Id, nodo);
            _listaAdyacencia.Add(nodo.Id, new List<Tramo>());
        }
    }

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

  
    public void AgregarTramoDobleSentido(string nodoA, string nodoB, int tiempoMinutos)
    {
        AgregarTramo(nodoA, nodoB, tiempoMinutos);
        AgregarTramo(nodoB, nodoA, tiempoMinutos);
    }

  
    public List<Tramo> ObtenerVecinos(string idNodo)
    {
        if (!string.IsNullOrWhiteSpace(idNodo) && _listaAdyacencia.TryGetValue(idNodo.Trim(), out var vecinos))
        {
            return vecinos;
        }
        return new List<Tramo>();
    }

  
    public Dictionary<string, NodoVial> GetNodos() => _nodos;

    
    public bool ContieneNodo(string idNodo) => !string.IsNullOrWhiteSpace(idNodo) && _nodos.ContainsKey(idNodo.Trim());

    
    public NodoVial? ObtenerNodo(string idNodo)
    {
        if (!string.IsNullOrWhiteSpace(idNodo) && _nodos.TryGetValue(idNodo.Trim(), out var nodo))
        {
            return nodo;
        }
        return null;
    }

   
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
