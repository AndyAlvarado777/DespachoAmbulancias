namespace DespachoAmbulancias.Models;


public enum NivelTriaje
{
    Rojo = 1,
    Amarillo = 2,
    Verde = 3
}

public enum EstadoAmbulancia
{
    Disponible,
    EnRuta,
    Ocupada
}
public enum TipoNodo
{
    Interseccion,
    Hospital,
    BaseAmbulancia,
    PuntoInteres
}
