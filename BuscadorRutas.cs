using System;
using System.Collections.Generic;

// ============================================================
// RESULTADO de BuscarMasCercano
// Equivale a la tupla (nodoEncontrado, tiempoTotal, previo)
// del pseudocódigo de la Fase 1.
// ============================================================
public class ResultadoBusqueda
{
    public string NodoEncontrado { get; }
    public double TiempoTotal { get; }                      // minutos
    public Dictionary<string, string> Previo { get; }       // idNodo -> nodo anterior en la ruta

    public ResultadoBusqueda(string nodoEncontrado, double tiempoTotal,
                             Dictionary<string, string> previo)
    {
        NodoEncontrado = nodoEncontrado;
        TiempoTotal = tiempoTotal;
        Previo = previo;
    }
}

// ============================================================
// BUSCADOR DE RUTAS
// Dijkstra con terminación temprana + reconstrucción de rutas.
// Usa el GrafoVial del equipo (ObtenerVecinos / Tramo.Destino / Tramo.Tiempo).
//
// Uso en el despachador:
//   // 1) Ambulancia más cercana: se busca sobre el grafo TRANSPUESTO
//   GrafoVial gt = grafo.ObtenerGrafoTranspuesto();
//   var amb = BuscadorRutas.BuscarMasCercano(gt, idIncidente, n => hayAmbulanciaEn(n));
//   var rutaAmbulancia = BuscadorRutas.SeguirPrevio(amb.Previo, amb.NodoEncontrado);
//
//   // 2) Hospital más cercano: se busca sobre el grafo ORIGINAL
//   var hosp = BuscadorRutas.BuscarMasCercano(grafo, idIncidente, n => hayHospitalEn(n));
//   var rutaHospital = BuscadorRutas.ReconstruirRuta(hosp.Previo, hosp.NodoEncontrado);
// ============================================================
public static class BuscadorRutas
{
    // --------------------------------------------------------
    // ALGORITMO BuscarMasCercano(G, origen, EsObjetivo)
    // Devuelve null si no hay ningún objetivo alcanzable.
    // --------------------------------------------------------
    public static ResultadoBusqueda BuscarMasCercano(
        GrafoVial grafo,
        string origen,
        Func<string, bool> esObjetivo)
    {
        if (grafo == null) throw new ArgumentNullException(nameof(grafo));
        if (esObjetivo == null) throw new ArgumentNullException(nameof(esObjetivo));
        if (string.IsNullOrEmpty(origen))
            throw new ArgumentException("El nodo origen no puede estar vacío.", nameof(origen));
        if (!grafo.GetNodos().ContainsKey(origen))
            throw new ArgumentException($"El nodo origen '{origen}' no existe en el grafo.", nameof(origen));

        var dist = new Dictionary<string, double>();      // idNodo -> tiempo mínimo conocido
        var previo = new Dictionary<string, string>();    // idNodo -> nodo anterior en la ruta
        var visitados = new HashSet<string>();            // nodos con distancia ya definitiva
        var cp = new PriorityQueue<string, double>();     // montículo de mínimos (tiempo)

        dist[origen] = 0;
        cp.Enqueue(origen, 0);

        while (cp.TryDequeue(out string u, out double d))
        {
            // Entrada obsoleta (inserción perezosa): ya se procesó con menor tiempo
            if (visitados.Contains(u) || d > dist[u])
                continue;

            visitados.Add(u);   // dist[u] ya es definitiva

            // Terminación temprana: u es el objetivo más cercano
            if (esObjetivo(u))
                return new ResultadoBusqueda(u, d, previo);

            // Relajar las aristas que salen de u
            foreach (Tramo tramo in grafo.ObtenerVecinos(u))
            {
                if (tramo.Tiempo < 0)
                    throw new ArgumentException(
                        $"El tramo {u} -> {tramo.Destino} tiene tiempo negativo ({tramo.Tiempo}). " +
                        "Dijkstra solo funciona con tiempos no negativos.");

                string v = tramo.Destino;
                double nuevo = d + tramo.Tiempo;

                if (!visitados.Contains(v) &&
                    (!dist.ContainsKey(v) || nuevo < dist[v]))
                {
                    dist[v] = nuevo;
                    previo[v] = u;
                    cp.Enqueue(v, nuevo);
                }
            }
        }

        return null;   // ningún objetivo alcanzable
    }

    // --------------------------------------------------------
    // ALGORITMO SeguirPrevio(previo, inicio)
    // Para la búsqueda de AMBULANCIA (sobre el grafo transpuesto):
    // seguir previo desde la ambulancia ya da la ruta en orden de viaje
    // (ambulancia -> ... -> incidente).
    // --------------------------------------------------------
    public static List<string> SeguirPrevio(Dictionary<string, string> previo, string inicio)
    {
        var ruta = new List<string>();
        string actual = inicio;

        while (actual != null)
        {
            ruta.Add(actual);
            // El origen de la búsqueda no está en previo -> actual queda null
            actual = previo.TryGetValue(actual, out string anterior) ? anterior : null;
        }

        return ruta;
    }

    // --------------------------------------------------------
    // ALGORITMO ReconstruirRuta(previo, destino)
    // Para la búsqueda de HOSPITAL (sobre el grafo original):
    // la cadena queda al revés (hospital -> ... -> incidente),
    // así que se usa una PILA (LIFO) para invertirla.
    // --------------------------------------------------------
    public static List<string> ReconstruirRuta(Dictionary<string, string> previo, string destino)
    {
        var pila = new Stack<string>();
        string actual = destino;

        // Se apila de destino hacia origen
        while (actual != null)
        {
            pila.Push(actual);
            actual = previo.TryGetValue(actual, out string anterior) ? anterior : null;
        }

        // Se desapila: sale de origen hacia destino
        var ruta = new List<string>();
        while (pila.Count > 0)
            ruta.Add(pila.Pop());

        return ruta;
    }
}
