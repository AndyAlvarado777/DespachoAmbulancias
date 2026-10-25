using System;
using System.Collections.Generic;

// ============================================================
// CASOS DE PRUEBA de BuscadorRutas (Etapa 2 - PED941)
// Cada prueba indica ENTRADA -> SALIDA ESPERADA.
// Se ejecuta llamando PruebasBuscadorRutas.Ejecutar() desde Main.
// ============================================================
public static class PruebasBuscadorRutas
{
    private static int pasadas = 0;
    private static int falladas = 0;

    private static void Verificar(string nombre, bool condicion, string detalle = "")
    {
        if (condicion) { pasadas++; Console.WriteLine($"[OK]    {nombre}"); }
        else { falladas++; Console.WriteLine($"[FALLO] {nombre} {detalle}"); }
    }

    private static bool Igual(double a, double b) => Math.Abs(a - b) < 1e-9;

    // Construye un grafo a partir de ids y calles (doble sentido o sentido único)
    private static GrafoVial Crear(string[] ids, params (string a, string b, double min, bool dobleSentido)[] calles)
    {
        var g = new GrafoVial();
        foreach (var id in ids) g.AgregarNodo(new Nodo(id, id));
        foreach (var c in calles)
        {
            g.AgregarTramo(c.a, c.b, c.min);
            if (c.dobleSentido) g.AgregarTramo(c.b, c.a, c.min);
        }
        return g;
    }

    // Devuelve 0 si todas pasan, 1 si alguna falla
    public static int Ejecutar()
    {
        pasadas = 0; falladas = 0;

        // Red de la traza de ejemplo de la Fase 1 (calles de doble sentido)
        var red = Crear(new[] { "I", "X1", "X2", "X3", "A1", "A2" },
            ("I", "X1", 4, true), ("I", "X2", 2, true), ("X2", "X1", 1, true),
            ("X1", "X3", 5, true), ("X2", "X3", 8, true), ("X1", "A2", 10, true),
            ("X3", "A2", 2, true), ("X3", "A1", 3, true));
        Func<string, bool> hayAmbulancia = n => n == "A1" || n == "A2";

        // CP-01: traza de la Fase 1 -> A2, 10 min
        var r1 = BuscadorRutas.BuscarMasCercano(red, "I", hayAmbulancia);
        Verificar("CP-01 nodo encontrado es A2 (unidad U-02)", r1 != null && r1.NodoEncontrado == "A2");
        Verificar("CP-01 tiempo total es 10 min", r1 != null && Igual(r1.TiempoTotal, 10),
                  r1 == null ? "(null)" : $"(dio {r1.TiempoTotal})");

        // CP-02: ruta ambulancia -> incidente con SeguirPrevio
        var rutaIda = BuscadorRutas.SeguirPrevio(r1.Previo, r1.NodoEncontrado);
        Verificar("CP-02 SeguirPrevio da A2,X3,X1,X2,I",
                  string.Join(",", rutaIda) == "A2,X3,X1,X2,I", $"(dio {string.Join(",", rutaIda)})");

        // CP-03: la calle directa I-X1 (4) pierde contra I-X2-X1 (3)
        Verificar("CP-03 previo[X1] es X2", r1.Previo["X1"] == "X2");

        // CP-04: A2 por X1 costaría 13, por X3 son 10
        Verificar("CP-04 previo[A2] es X3 (no X1)", r1.Previo["A2"] == "X3");

        // CP-05: terminación temprana, A1 (11 min) no se elige
        Verificar("CP-05 no se elige A1 (11 min) porque A2 (10 min) es más cercana", r1.NodoEncontrado != "A1");

        // CP-06: ReconstruirRuta con pila (incidente -> hospital)
        var r2 = BuscadorRutas.BuscarMasCercano(red, "I", n => n == "A1");
        var rutaTraslado = BuscadorRutas.ReconstruirRuta(r2.Previo, r2.NodoEncontrado);
        Verificar("CP-06 ReconstruirRuta da I,X2,X1,X3,A1 (orden de viaje)",
                  string.Join(",", rutaTraslado) == "I,X2,X1,X3,A1", $"(dio {string.Join(",", rutaTraslado)})");
        Verificar("CP-06 tiempo de traslado es 11 min", Igual(r2.TiempoTotal, 11));

        // CP-07 (borde): el origen ya es el objetivo -> 0 min
        var r3 = BuscadorRutas.BuscarMasCercano(red, "A1", hayAmbulancia);
        Verificar("CP-07 origen ya es objetivo: 0 min", r3 != null && Igual(r3.TiempoTotal, 0) && r3.NodoEncontrado == "A1");
        Verificar("CP-07 ruta de un solo nodo",
                  string.Join(",", BuscadorRutas.ReconstruirRuta(r3.Previo, "A1")) == "A1");

        // CP-08 (borde): ningún objetivo alcanzable -> null
        Verificar("CP-08 sin objetivo alcanzable devuelve null",
                  BuscadorRutas.BuscarMasCercano(red, "I", n => false) == null);

        // CP-09 (borde): nodo aislado
        var aislado = Crear(new[] { "Z", "A1" });
        Verificar("CP-09 incidente en nodo aislado devuelve null",
                  BuscadorRutas.BuscarMasCercano(aislado, "Z", hayAmbulancia) == null);

        // CP-10 (borde): calle de un solo sentido
        var dirigido = Crear(new[] { "A", "B" }, ("A", "B", 3, false));
        Verificar("CP-10 A->B sí se alcanza",
                  Igual(BuscadorRutas.BuscarMasCercano(dirigido, "A", n => n == "B")?.TiempoTotal ?? -1, 3));
        Verificar("CP-10 B->A no se alcanza (un solo sentido)",
                  BuscadorRutas.BuscarMasCercano(dirigido, "B", n => n == "A") == null);

        // CP-11 (borde): tiempo negativo -> ArgumentException
        var negativo = Crear(new[] { "A", "B" }, ("A", "B", -2, false));
        bool lanzo = false;
        try { BuscadorRutas.BuscarMasCercano(negativo, "A", n => n == "B"); }
        catch (ArgumentException) { lanzo = true; }
        Verificar("CP-11 tiempo negativo lanza ArgumentException", lanzo);

        // CP-12 (borde): origen vacío o inexistente -> ArgumentException
        bool e1 = false, e2 = false;
        try { BuscadorRutas.BuscarMasCercano(red, "", hayAmbulancia); } catch (ArgumentException) { e1 = true; }
        try { BuscadorRutas.BuscarMasCercano(red, "NO_EXISTE", hayAmbulancia); } catch (ArgumentException) { e2 = true; }
        Verificar("CP-12 origen vacío lanza ArgumentException", e1);
        Verificar("CP-12 origen inexistente lanza ArgumentException", e2);

        // CP-13: empate de tiempos -> devuelve una de las dos a 5 min
        var empate = Crear(new[] { "I", "A1", "A2" }, ("I", "A1", 5, true), ("I", "A2", 5, true));
        var r6 = BuscadorRutas.BuscarMasCercano(empate, "I", hayAmbulancia);
        Verificar("CP-13 empate: devuelve una unidad a 5 min",
                  r6 != null && Igual(r6.TiempoTotal, 5) && (r6.NodoEncontrado == "A1" || r6.NodoEncontrado == "A2"));

        // CP-14: rodeo de 2 min gana al directo de 10 min
        var rodeo = Crear(new[] { "I", "X1", "H" }, ("I", "H", 10, false), ("I", "X1", 1, false), ("X1", "H", 1, false));
        var r7 = BuscadorRutas.BuscarMasCercano(rodeo, "I", n => n == "H");
        Verificar("CP-14 elige el rodeo de 2 min sobre el directo de 10 min", r7 != null && Igual(r7.TiempoTotal, 2));
        Verificar("CP-14 ruta I,X1,H", string.Join(",", BuscadorRutas.ReconstruirRuta(r7.Previo, "H")) == "I,X1,H");

        // CP-15: tiempos decimales (el mapa de ejemplo del equipo usa 3.5)
        // Mapa: N1->N2 (4), N2->N3 (3.5), N2->N4 (5), N4->N3 (2). Incidente en N4.
        var mapa = GrafoVial.CrearMapaEjemplo();
        var hosp = BuscadorRutas.BuscarMasCercano(mapa, "N4", n => n == "N3");
        Verificar("CP-15 hospital más cercano al incidente N4 es N3 en 2 min",
                  hosp != null && hosp.NodoEncontrado == "N3" && Igual(hosp.TiempoTotal, 2));
        Verificar("CP-15 ruta de traslado N4,N3",
                  string.Join(",", BuscadorRutas.ReconstruirRuta(hosp.Previo, "N3")) == "N4,N3");

        // CP-16: grafo TRANSPUESTO en el mapa del equipo.
        // Ambulancia en N1, incidente en N4. Sobre GT se busca desde N4 hasta N1.
        var gt = mapa.ObtenerGrafoTranspuesto();
        var amb = BuscadorRutas.BuscarMasCercano(gt, "N4", n => n == "N1");
        Verificar("CP-16 ambulancia en N1 llega al incidente N4 en 9 min (4 + 5)",
                  amb != null && Igual(amb.TiempoTotal, 9), amb == null ? "(null)" : $"(dio {amb.TiempoTotal})");
        Verificar("CP-16 ruta de la ambulancia N1,N2,N4",
                  string.Join(",", BuscadorRutas.SeguirPrevio(amb.Previo, amb.NodoEncontrado)) == "N1,N2,N4");

        // CP-17: sin GT la ambulancia no encontraría camino (calles de un solo sentido)
        Verificar("CP-17 sobre el grafo original, desde N4 no se llega a N1 (sentidos de las calles)",
                  BuscadorRutas.BuscarMasCercano(mapa, "N4", n => n == "N1") == null);

        Console.WriteLine();
        Console.WriteLine($"Resultado: {pasadas} pasadas, {falladas} falladas.");
        return falladas == 0 ? 0 : 1;
    }
}
