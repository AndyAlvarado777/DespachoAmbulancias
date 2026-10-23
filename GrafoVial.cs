using System;
using System.Collections.Generic;

public class GrafoVial
{
    private Dictionary<string, Nodo> nodos;
    private Dictionary<string, List<Tramo>> listaAdyacencia;

    public GrafoVial()
    {
        nodos = new Dictionary<string, Nodo>();
        listaAdyacencia = new Dictionary<string, List<Tramo>>();
    }

    public void AgregarNodo(Nodo nodo)
    {
        if (!nodos.ContainsKey(nodo.IdNodo))
        {
            nodos.Add(nodo.IdNodo, nodo);
            listaAdyacencia.Add(nodo.IdNodo, new List<Tramo>());
        }
    }

    public void AgregarTramo(string origen, string destino, double tiempo)
    {
        if (listaAdyacencia.ContainsKey(origen))
        {
            listaAdyacencia[origen].Add(new Tramo(destino, tiempo));
        }
    }

    public List<Tramo> ObtenerVecinos(string idNodo)
    {
        if (listaAdyacencia.ContainsKey(idNodo))
            return listaAdyacencia[idNodo];
        return new List<Tramo>();
    }

    public Dictionary<string, Nodo> GetNodos()
    {
        return nodos;
    }

    // Grafo Transpuesto (GT)
    public GrafoVial ObtenerGrafoTranspuesto()
    {
        GrafoVial grafoT = new GrafoVial();

        foreach (var nodo in nodos.Values)
        {
            grafoT.AgregarNodo(new Nodo(nodo.IdNodo, nodo.Nombre));
        }

        foreach (var origen in listaAdyacencia.Keys)
        {
            foreach (var tramo in listaAdyacencia[origen])
            {
                grafoT.AgregarTramo(tramo.Destino, origen, tramo.Tiempo);
            }
        }

        return grafoT;
    }

    // Mapa de Ejemplo
    public static GrafoVial CrearMapaEjemplo()
    {
        GrafoVial grafo = new GrafoVial();

        grafo.AgregarNodo(new Nodo("N1", "Base Ambulancia Central"));
        grafo.AgregarNodo(new Nodo("N2", "Intersección A"));
        grafo.AgregarNodo(new Nodo("N3", "Hospital General"));
        grafo.AgregarNodo(new Nodo("N4", "Zona de Incidente"));

        grafo.AgregarTramo("N1", "N2", 4.0);
        grafo.AgregarTramo("N2", "N3", 3.5);
        grafo.AgregarTramo("N2", "N4", 5.0);
        grafo.AgregarTramo("N4", "N3", 2.0);

        return grafo;
    }
}
