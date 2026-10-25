using System;
using System.Collections.Generic;
using DespachoAmbulancias.Models;
using DespachoAmbulancias.DataStructures;

namespace DespachoAmbulancias;

/// <summary>
/// Módulo Principal de Interacción y Consola.
/// <para>
/// <b>Responsable:</b> Andy Alvarado (Diseñador de Software e Interfaz).
/// <b>Materia:</b> Programación con Estructuras de Datos - Universidad Don Bosco.
/// </para>
/// <para>
/// Este módulo implementa la interfaz de usuario basada en consola mediante un bucle interactivo
/// <c>do-while</c> y una bifurcación de control <c>switch-case</c>. Integra las estructuras de datos
/// desarrolladas para la Fase 2:
/// <list type="bullet">
///   <item><description><b>GrafoVial (G y G^T):</b> Representación urbana mediante listas de adyacencia (Marvin González &amp; Andy Alvarado).</description></item>
///   <item><description><b>Modelos de Dominio:</b> Entidades POO con validación y tipado fuerte (Andy Alvarado).</description></item>
///   <item><description><b>Cola de Prioridad:</b> Encolamiento clínico de triaje ordenado por gravedad y tiempo (FIFO en empates).</description></item>
/// </list>
/// </para>
/// </summary>
internal class Program
{
    // =========================================================================
    // COLECCIONES Y ESTRUCTURAS EN MEMORIA
    // =========================================================================
    // Red vial urbana representada como Grafo Dirigido Ponderado G = (V, E)
    private static GrafoVial _mapaVial = new();
    private static readonly List<Ambulancia> _ambulancias = new();
    private static readonly List<Hospital> _hospitales = new();

    // Cola de prioridad: Implementa la lógica clínica de triaje (1=Rojo, 2=Amarillo, 3=Verde)
    // Ordenada por IComparable<Emergencia> (Triaje asc, luego FechaHora asc).
    private static readonly List<Emergencia> _colaEmergenciasPendientes = new();
    private static int _contadorEmergencias = 2;

    static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        InicializarDatosSemilla();

        bool continuar = true;

        do
        {
            MostrarEncabezadoSistema();
            MostrarOpcionesMenu();

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(" ► Seleccione una opción [1 - 5]: ");
            string? entrada = Console.ReadLine();

            if (!int.TryParse(entrada, out int opcion))
            {
                MostrarMensajeError("Debe ingresar un número entero válido.");
                PausarConsola();
                continue;
            }

            Console.WriteLine();

            switch (opcion)
            {
                case 1:
                    RegistrarLlamada911();
                    break;

                case 2:
                    DespacharSiguienteEmergencia();
                    break;

                case 3:
                    ConsultarEstadoRecursos();
                    break;

                case 4:
                    VisualizarEstructuraMapaVial();
                    break;

                case 5:
                    continuar = false;
                    MostrarMensajeDespedida();
                    break;

                default:
                    MostrarMensajeError("Opción no válida. Por favor elija un número del 1 al 5.");
                    break;
            }

            if (continuar)
            {
                PausarConsola();
            }

        } while (continuar);
    }

    // =========================================================================
    // MÉTODOS DE RENDERIZADO DEL MENÚ
    // =========================================================================

    private static void MostrarEncabezadoSistema()
    {
        try
        {
            if (!Console.IsOutputRedirected)
            {
                Console.Clear();
            }
        }
        catch (Exception)
        {
            // Ignorar excepción si se ejecuta en terminal redirigida o entorno no interactivo
        }

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("╔═══════════════════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║            SISTEMA DE OPTIMIZACIÓN DE RUTAS DE AMBULANCIAS 911            ║");
        Console.WriteLine("║            Universidad Don Bosco - Programación con Estructuras           ║");
        Console.WriteLine("╚═══════════════════════════════════════════════════════════════════════════╝");
        Console.ResetColor();

        // Banner informativo del estado actual
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine($" [Emergencias en espera: {_colaEmergenciasPendientes.Count}] | [Ambulancias: {_ambulancias.Count}] | [Hospitales: {_hospitales.Count}] | [Vértices V: {_mapaVial.CantidadNodos}]");
        Console.WriteLine("─────────────────────────────────────────────────────────────────────────────");
        Console.ResetColor();
    }

    private static void MostrarOpcionesMenu()
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(" MENÚ DE CONTROL PRINCIPAL:");
        Console.ResetColor();
        Console.WriteLine("  [1] Registrar llamada de emergencia al 911");
        Console.WriteLine("  [2] Despachar siguiente emergencia (Dijkstra + Grafo Transpuesto)");
        Console.WriteLine("  [3] Consultar estado de ambulancias y hospitales");
        Console.WriteLine("  [4] Visualizar estructura de la red vial (Grafo G y Grafo Transpuesto GT)");
        Console.WriteLine("  [5] Salir del sistema");
        Console.WriteLine("─────────────────────────────────────────────────────────────────────────────");
    }

    // =========================================================================
    // CASO DE USO 1: REGISTRAR LLAMADA DE EMERGENCIA AL 911
    // =========================================================================
    private static void RegistrarLlamada911()
    {
        MostrarSubencabezado("MÓDULO DE RECEPCIÓN 911 - REGISTRO DE EMERGENCIA");

        Console.WriteLine(" Ingrese los datos clínicos y de ubicación del incidente:");
        Console.WriteLine();

        // 1. Nombre del paciente / alertante
        string paciente = SolicitarCadenaNoVacia(" ► Nombre del paciente / reportante: ");

        // 2. Ubicación en la red vial
        Console.WriteLine("\n Nodos viales disponibles en el mapa:");
        Console.ForegroundColor = ConsoleColor.DarkCyan;
        foreach (var nodo in _mapaVial.GetNodos().Values)
        {
            Console.WriteLine($"   • {nodo.Id} ({nodo.Nombre})");
        }
        Console.ResetColor();

        string nodoUbicacionId;
        while (true)
        {
            nodoUbicacionId = SolicitarCadenaNoVacia("\n ► Ingrese el ID del nodo del incidente: ").ToUpperInvariant();
            if (_mapaVial.ContieneNodo(nodoUbicacionId))
            {
                break;
            }
            MostrarMensajeError($"El nodo '{nodoUbicacionId}' no existe en la red vial. Seleccione uno de la lista.");
        }

        // 3. Selección y clasificación de Triaje clínico
        Console.WriteLine("\n Clasificación de Triaje Médico:");
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("   [1] ROJO     - Emergencia crítica (Riesgo vital inmediato)");
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("   [2] AMARILLO - Urgencia médica (Potencialmente grave)");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("   [3] VERDE    - Condición leve (No urgente)");
        Console.ResetColor();

        int triajeOpcion = SolicitarEnteroEnRango(" ► Seleccione el nivel de triaje [1 - 3]: ", 1, 3);
        NivelTriaje nivelTriaje = (NivelTriaje)triajeOpcion;

        // Generación de Id y registro de entidad
        string idEmergencia = $"EMG-{_contadorEmergencias:D3}";
        _contadorEmergencias++;

        Emergencia nuevaEmergencia = new(idEmergencia, paciente, nivelTriaje, nodoUbicacionId, DateTime.Now);

        // ENCOLAMIENTO CON PRIORIDAD:
        // Se inserta en la lista y se mantiene ordenada según el criterio de IComparable<Emergencia>
        // (Simula el comportamiento de inserción en O(log N) de la ColaTriaje de Marvin).
        _colaEmergenciasPendientes.Add(nuevaEmergencia);
        _colaEmergenciasPendientes.Sort(); // Ordena: 1° Triaje (Rojo > Amarillo > Verde), 2° FechaHora (FIFO)

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("\n ✔ Emergencia registrada exitosamente en la Cola de Prioridad:");
        Console.ResetColor();
        Console.WriteLine($"   {nuevaEmergencia}");
        Console.WriteLine($"   Posición en cola: {_colaEmergenciasPendientes.IndexOf(nuevaEmergencia) + 1} de {_colaEmergenciasPendientes.Count}");
    }

    // =========================================================================
    // CASO DE USO 2: DESPACHAR SIGUIENTE EMERGENCIA
    // =========================================================================
    private static void DespacharSiguienteEmergencia()
    {
        MostrarSubencabezado("DESPACHO INTELIGENTE - ASIGNACIÓN ÓPTIMA CON DIJKSTRA");

        if (_colaEmergenciasPendientes.Count == 0)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine(" ⓘ No hay emergencias pendientes en la cola de prioridad.");
            Console.ResetColor();
            return;
        }

        // 1. Extraer la emergencia de mayor prioridad clínica (Raíz del Min-Heap)
        Emergencia emergencia = _colaEmergenciasPendientes[0];
        _colaEmergenciasPendientes.RemoveAt(0);

        Console.WriteLine($" Procesando emergencia más prioritaria extraída de la cola:");
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine($" ► {emergencia}\n");
        Console.ResetColor();

        // 2. BÚSQUEDA DE AMBULANCIA DISPONIBLE:
        // En Fase 2 completa, se invocará:
        // var resultadoAmb = DijkstraEngine.BuscarMasCercano(_mapaVial.ObtenerGrafoTranspuesto(), emergencia.UbicacionNodoId, ...);
        var ambulancia = _ambulancias.Find(a => a.HayDisponible());

        if (ambulancia == null)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(" ✖ ALERTA: No hay ambulancias disponibles en este momento.");
            Console.WriteLine(" ⓘ La emergencia ha sido reingresada a la cola de espera según regla de negocio.");
            Console.ResetColor();

            _colaEmergenciasPendientes.Insert(0, emergencia); // Reingresa
            _colaEmergenciasPendientes.Sort();
            return;
        }

        // 3. BÚSQUEDA DE HOSPITAL CON CAMAS DISPONIBLES:
        // En Fase 2 completa, se invocará:
        // var resultadoHosp = DijkstraEngine.BuscarMasCercano(_mapaVial, emergencia.UbicacionNodoId, ...);
        var hospital = _hospitales.Find(h => h.TieneCamasDisponibles());

        if (hospital == null)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(" ✖ ALERTA CRÍTICA: No hay camas disponibles en ningún hospital de la red.");
            Console.ResetColor();
            _colaEmergenciasPendientes.Insert(0, emergencia);
            return;
        }

        // Simulación de cálculo de ruta y tiempos (integración con Juan Monge)
        ambulancia.TomarDisponible();
        hospital.OcuparCama();

        // Creación del objeto de asignación (del diagrama UML Fase 1)
        List<string> rutaIda = new() { ambulancia.UbicacionActualNodoId, "X1", emergencia.UbicacionNodoId };
        List<string> rutaHospital = new() { emergencia.UbicacionNodoId, "X2", hospital.UbicacionNodoId };

        Asignacion despacho = new(
            emergencia: emergencia,
            ambulancia: ambulancia,
            rutaIda: rutaIda,
            tiempoLlegada: 6,      // Minutos calculados por Dijkstra sobre GT
            hospital: hospital,
            rutaHospital: rutaHospital,
            tiempoTraslado: 8     // Minutos calculados por Dijkstra sobre G
        );

        despacho.ImprimirResumenDespacho();

        // Preguntar si se desea finalizar el servicio para liberar recursos
        Console.Write(" ¿Desea simular la finalización del servicio y liberar la ambulancia en el hospital? (S/N): ");
        string? resp = Console.ReadLine();
        if (resp?.Trim().ToUpperInvariant() == "S")
        {
            ambulancia.Liberar(hospital.UbicacionNodoId);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($" ✔ Unidad {ambulancia.Id} liberada y estacionada en {hospital.UbicacionNodoId}. Estado: Disponible.");
            Console.ResetColor();
        }
    }

    // =========================================================================
    // CASO DE USO 3: CONSULTAR ESTADO DE RECURSOS
    // =========================================================================
    private static void ConsultarEstadoRecursos()
    {
        MostrarSubencabezado("MONITOR DE RECURSOS: AMBULANCIAS, HOSPITALES Y COLA DE ESPERA");

        // Sección Ambulancias
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(" ► FLOTA DE AMBULANCIAS:");
        Console.ResetColor();
        Console.WriteLine(" ┌──────────┬────────────┬──────────────────┬────────────────────────┐");
        Console.WriteLine(" │ ID       │ Placa      │ Ubicación        │ Estado                 │");
        Console.WriteLine(" ├──────────┼────────────┼──────────────────┼────────────────────────┤");
        foreach (var amb in _ambulancias)
        {
            string estadoTxt = amb.Estado switch
            {
                EstadoAmbulancia.Disponible => "Disponible  ",
                EstadoAmbulancia.EnRuta     => "En Ruta     ",
                EstadoAmbulancia.Ocupada    => "Ocupada     ",
                _ => amb.Estado.ToString()
            };
            Console.WriteLine($" │ {amb.Id,-8} │ {amb.Placa,-10} │ {amb.UbicacionActualNodoId,-16} │ {estadoTxt,-22} │");
        }
        Console.WriteLine(" └──────────┴────────────┴──────────────────┴────────────────────────┘");

        // Sección Hospitales
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("\n ► RED HOSPITALARIA:");
        Console.ResetColor();
        Console.WriteLine(" ┌──────────────┬────────────────────────────┬──────────────┬───────────────┐");
        Console.WriteLine(" │ ID           │ Nombre                     │ Nodo Vial    │ Camas Libres  │");
        Console.WriteLine(" ├──────────────┼────────────────────────────┼──────────────┼───────────────┤");
        foreach (var hosp in _hospitales)
        {
            Console.WriteLine($" │ {hosp.Id,-12} │ {hosp.Nombre,-26} │ {hosp.UbicacionNodoId,-12} │ {hosp.CamasDisponibles,13} │");
        }
        Console.WriteLine(" └──────────────┴────────────────────────────┴──────────────┴───────────────┘");

        // Sección Emergencias Pendientes
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("\n ► COLA DE EMERGENCIAS PENDIENTES (Ordenada por Triaje clínico):");
        Console.ResetColor();
        if (_colaEmergenciasPendientes.Count == 0)
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("   ✔ No hay llamadas en cola. Todas las emergencias han sido atendidas.");
            Console.ResetColor();
        }
        else
        {
            int pos = 1;
            foreach (var emg in _colaEmergenciasPendientes)
            {
                Console.WriteLine($"   [{pos++}] {emg}");
            }
        }
    }

    // =========================================================================
    // CASO DE USO 4: VISUALIZAR ESTRUCTURA DEL MAPA VIAL
    // =========================================================================
    private static void VisualizarEstructuraMapaVial()
    {
        MostrarSubencabezado("TOPOLOGÍA DE LA RED VIAL - GRAFO DIRIGIDO Y PONDERADO G = (V, E)");

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($" ► VÉRTICES / NODOS DEL GRAFO ({_mapaVial.CantidadNodos} vértices):");
        Console.ResetColor();
        Console.WriteLine(" ┌──────────────┬────────────────────────────────────────┬──────────────────────┐");
        Console.WriteLine(" │ ID Nodo      │ Nombre o Intersección                  │ Tipo                 │");
        Console.WriteLine(" ├──────────────┼────────────────────────────────────────┼──────────────────────┤");
        foreach (var nodo in _mapaVial.GetNodos().Values)
        {
            Console.WriteLine($" │ {nodo.Id,-12} │ {nodo.Nombre,-38} │ {nodo.Tipo,-20} │");
        }
        Console.WriteLine(" └──────────────┴────────────────────────────────────────┴──────────────────────┘");

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"\n ► LISTA DE ADYACENCIA DEL GRAFO ORIGINAL G ({_mapaVial.CantidadTramos} tramos dirigidos):");
        Console.ResetColor();
        Console.WriteLine(" ┌──────────────────────┬──────────────────────┬───────────────────────────────┐");
        Console.WriteLine(" │ Origen               │ Destino              │ Peso (Tiempo en Minutos)      │");
        Console.WriteLine(" ├──────────────────────┼──────────────────────┼───────────────────────────────┤");
        foreach (var origenId in _mapaVial.GetNodos().Keys)
        {
            var vecinos = _mapaVial.ObtenerVecinos(origenId);
            foreach (var tramo in vecinos)
            {
                Console.WriteLine($" │ {tramo.OrigenId,-20} │ {tramo.DestinoId,-20} │ {tramo.TiempoMinutos,25} min │");
            }
        }
        Console.WriteLine(" └──────────────────────┴──────────────────────┴───────────────────────────────┘");

        // Demostración didáctica del Grafo Transpuesto G^T (Requisito clave de la Fase 1 y 2)
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine("\n ► DEMOSTRACIÓN: GRAFO TRANSPUESTO G^T (Sentido de aristas invertido):");
        Console.ResetColor();
        Console.WriteLine(" ⓘ Calculado en O(V + E) para permitir la búsqueda de ambulancias hacia el incidente:");
        var grafoT = _mapaVial.ObtenerGrafoTranspuesto();
        Console.WriteLine($"   Total aristas invertidas en G^T: {grafoT.CantidadTramos}");
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("   Ejemplo de inversión: En G existe BASE-A1 ──> X1, en G^T existe X1 ──> BASE-A1.");
        Console.ResetColor();
    }

    // =========================================================================
    // MÉTODOS AUXILIARES DE VALIDACIÓN DE ENTRADA Y FORMATO
    // =========================================================================

    private static string SolicitarCadenaNoVacia(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string? entrada = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(entrada))
            {
                return entrada.Trim();
            }
            MostrarMensajeError("El texto no puede ser vacío o contener solo espacios.");
        }
    }

    private static int SolicitarEnteroEnRango(string prompt, int min, int max)
    {
        while (true)
        {
            Console.Write(prompt);
            string? entrada = Console.ReadLine();
            if (int.TryParse(entrada, out int valor) && valor >= min && valor <= max)
            {
                return valor;
            }
            MostrarMensajeError($"Debe ingresar un número entero válido entre {min} y {max}.");
        }
    }

    private static void MostrarSubencabezado(string titulo)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"\n═════════════════════════════════════════════════════════════════════════════");
        Console.WriteLine($" {titulo}");
        Console.WriteLine($"═════════════════════════════════════════════════════════════════════════════\n");
        Console.ResetColor();
    }

    private static void MostrarMensajeError(string mensaje)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($" ✖ Error: {mensaje}");
        Console.ResetColor();
    }

    private static void PausarConsola()
    {
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("\n Presione cualquier tecla para volver al menú principal...");
        Console.ResetColor();

        try
        {
            if (!Console.IsInputRedirected)
            {
                Console.ReadKey(intercept: true);
            }
        }
        catch (Exception)
        {
            // Ignorar en entornos de ejecución con pipes o no interactivos
        }
    }

    private static void MostrarMensajeDespedida()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("\n╔═══════════════════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║                 CERRANDO SISTEMA DE DESPACHO DE AMBULANCIAS               ║");
        Console.WriteLine("║               ¡Gracias por utilizar el optimizador de rutas!              ║");
        Console.WriteLine("╚═══════════════════════════════════════════════════════════════════════════╝\n");
        Console.ResetColor();
    }

    // =========================================================================
    // DATOS DE PRUEBA INICIALES (MOCK / SEED DATA)
    // =========================================================================
    // Representa el mapa urbano documentado en la Fase 1.
    // Eduardo Henríquez conectará aquí InicializadorMapa.cs
    private static void InicializarDatosSemilla()
    {
        // Red vial urbana mediante GrafoVial (Clase de DataStructures)
        _mapaVial = GrafoVial.CrearMapaEjemplo();

        // Flota de ambulancias inicial
        _ambulancias.AddRange(new[]
        {
            new Ambulancia("AMB-01", "AB-1020", "BASE-A1", EstadoAmbulancia.Disponible),
            new Ambulancia("AMB-02", "AB-3045", "BASE-A2", EstadoAmbulancia.Disponible)
        });

        // Hospitales de la red asistencial
        _hospitales.AddRange(new[]
        {
            new Hospital("HOSP-01", "Hospital Nacional Rosales", camasDisponibles: 4, ubicacionNodoId: "HOSP-01"),
            new Hospital("HOSP-02", "Hospital MQ ISSS", camasDisponibles: 2, ubicacionNodoId: "HOSP-02")
        });

        // Emergencia de prueba precargada en la cola
        _colaEmergenciasPendientes.Add(new Emergencia(
            id: "EMG-001",
            paciente: "Juan Carlos Pérez",
            nivelTriaje: NivelTriaje.Rojo,
            ubicacionNodoId: "INC-01",
            fechaHora: DateTime.Now.AddMinutes(-5)
        ));
    }
}
