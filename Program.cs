using System;

namespace ProyectoFase2
{
    class Program
    {
        static void Main(string[] args)
        {
            // Cargamos el mapa
            GrafoVial mapa = GrafoVial.CrearMapaEjemplo();

            Console.WriteLine("=== PROBANDO MIS CLASES (FASE 2) ===");

            foreach (var nodo in mapa.GetNodos().Values)
            {
                Console.WriteLine($"\nNodo: {nodo.IdNodo} - {nodo.Nombre}");
                var vecinos = mapa.ObtenerVecinos(nodo.IdNodo);

                foreach (var tramo in vecinos)
                {
                    Console.WriteLine($"   --> Conecta con {tramo.Destino} en {tramo.Tiempo} minutos");
                }
            }

            Console.WriteLine("\n¡Todo organizado y funcionando perfectamente!");
            Console.ReadKey();
        }
    }
}
