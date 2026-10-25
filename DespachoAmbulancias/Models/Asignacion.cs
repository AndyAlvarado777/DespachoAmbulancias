namespace DespachoAmbulancias.Models;


public class Asignacion
{
    public Emergencia EmergenciaAtendida { get; set; } = new();


    public Ambulancia AmbulanciaAsignada { get; set; } = new();


    public Hospital HospitalDestino { get; set; } = new();
    public int TiempoLlegadaMinutos { get; set; }
    public int TiempoTrasladoMinutos { get; set; }
    public int TiempoTotalMinutos => TiempoLlegadaMinutos + TiempoTrasladoMinutos;
    public List<string> RutaIda { get; set; } = new();
    public List<string> RutaHospital { get; set; } = new();
    public Asignacion() { }
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
