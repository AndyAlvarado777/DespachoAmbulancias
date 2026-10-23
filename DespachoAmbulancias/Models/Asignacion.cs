namespace DespachoAmbulancias.Models;

/// <summary>
/// Representa el resultado coordinado del motor de despacho (Algoritmo de Dijkstra).
/// <para>
/// <b>Diagrama UML Fase 1:</b>
/// Modela la asociación entre la <see cref="Emergencia"/> atendida, la <see cref="Ambulancia"/> asignada
/// y el <see cref="Hospital"/> de destino, incluyendo los tiempos de respuesta calculados y las rutas ordenadas.
/// </para>
/// </summary>
public class Asignacion
{
    /// <summary>
    /// Emergencia médica atendida en este despacho.
    /// </summary>
    public Emergencia EmergenciaAtendida { get; set; } = new();

    /// <summary>
    /// Ambulancia asignada para el auxilio.
    /// </summary>
    public Ambulancia AmbulanciaAsignada { get; set; } = new();

    /// <summary>
    /// Hospital de destino seleccionado con camas disponibles.
    /// </summary>
    public Hospital HospitalDestino { get; set; } = new();

    /// <summary>
    /// Tiempo estimado en minutos desde la ubicación de la ambulancia hasta el punto del incidente.
    /// </summary>
    public int TiempoLlegadaMinutos { get; set; }

    /// <summary>
    /// Tiempo estimado en minutos desde el punto del incidente hasta el hospital de destino.
    /// </summary>
    public int TiempoTrasladoMinutos { get; set; }

    /// <summary>
    /// Tiempo total del ciclo de despacho (Llegada + Traslado).
    /// </summary>
    public int TiempoTotalMinutos => TiempoLlegadaMinutos + TiempoTrasladoMinutos;

    /// <summary>
    /// Secuencia ordenada de identificadores de nodos que forman la ruta desde la ambulancia hacia el incidente.
    /// Obtenida mediante el seguimiento de predecesores sobre el Grafo Transpuesto (\( G^T \)).
    /// </summary>
    public List<string> RutaIda { get; set; } = new();

    /// <summary>
    /// Secuencia ordenada de identificadores de nodos que forman la ruta desde el incidente hacia el hospital.
    /// Reconstruida utilizando una Pila (Stack LIFO) sobre el Grafo Original (\( G \)).
    /// </summary>
    public List<string> RutaHospital { get; set; } = new();

    /// <summary>
    /// Constructor por defecto.
    /// </summary>
    public Asignacion() { }

    /// <summary>
    /// Inicializa una nueva asignación completa de servicio de emergencia.
    /// </summary>
    public Asignacion(
        Emergencia emergencia,
        Ambulancia ambulancia,
        List<string> rutaIda,
        int tiempoLlegada,
        Hospital hospital,
        List<string> rutaHospital,
        int tiempoTraslado)
    {
        EmergenciaAtendida = emergencia ?? throw new ArgumentNullException(nameof(emergencia));
        AmbulanciaAsignada = ambulancia ?? throw new ArgumentNullException(nameof(ambulancia));
        RutaIda = rutaIda ?? new List<string>();
        TiempoLlegadaMinutos = tiempoLlegada;
        HospitalDestino = hospital ?? throw new ArgumentNullException(nameof(hospital));
        RutaHospital = rutaHospital ?? new List<string>();
        TiempoTrasladoMinutos = tiempoTraslado;
    }

    /// <summary>
    /// Muestra un reporte estructurado y visual en consola con el detalle del despacho.
    /// </summary>
    public void ImprimirResumenDespacho()
    {
        Console.WriteLine("\n╔════════════════════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║                        RESUMEN OFICIAL DE DESPACHO                        ║");
        Console.WriteLine("╚════════════════════════════════════════════════════════════════════════════╝");
        Console.WriteLine($" ► Emergencia       : {EmergenciaAtendida.Id} (Paciente: {EmergenciaAtendida.Paciente})");
        Console.WriteLine($" ► Triaje Clínico   : {EmergenciaAtendida.NivelTriaje} (Ubicación: {EmergenciaAtendida.UbicacionNodoId})");
        Console.WriteLine($" ► Unidad Asignada  : {AmbulanciaAsignada.Id} (Placa: {AmbulanciaAsignada.Placa})");
        Console.WriteLine($" ► Tiempo de Arribo : {TiempoLlegadaMinutos} minutos");
        Console.WriteLine($" ► Ruta hacia Escena: {string.Join(" ──> ", RutaIda)}");
        Console.WriteLine("──────────────────────────────────────────────────────────────────────────────");
        Console.WriteLine($" ► Hospital Receptor: {HospitalDestino.Nombre} (Nodo: {HospitalDestino.UbicacionNodoId})");
        Console.WriteLine($" ► Camas Restantes  : {HospitalDestino.CamasDisponibles}");
        Console.WriteLine($" ► Tiempo Traslado  : {TiempoTrasladoMinutos} minutos");
        Console.WriteLine($" ► Ruta al Hospital : {string.Join(" ──> ", RutaHospital)}");
        Console.WriteLine("──────────────────────────────────────────────────────────────────────────────");
        Console.WriteLine($" ★ TIEMPO TOTAL ESTIMADO DE OPERACIÓN: {TiempoTotalMinutos} minutos");
        Console.WriteLine("══════════════════════════════════════════════════════════════════════════════\n");
    }
}
