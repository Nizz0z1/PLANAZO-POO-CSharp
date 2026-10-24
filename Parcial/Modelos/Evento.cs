namespace Planazo.Modelos;

public abstract class Evento
{
    protected Evento(int id, string nombre, DateTime fecha, decimal precioBase, int capacidadMaxima)
    {
        if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id), "El identificador debe ser positivo.");
        if (string.IsNullOrWhiteSpace(nombre)) throw new ArgumentException("El nombre es obligatorio.", nameof(nombre));
        if (precioBase < 0) throw new ArgumentOutOfRangeException(nameof(precioBase), "El precio no puede ser negativo.");
        if (capacidadMaxima <= 0) throw new ArgumentOutOfRangeException(nameof(capacidadMaxima), "La capacidad debe ser positiva.");
        Id = id;
        Nombre = nombre.Trim();
        Fecha = fecha;
        PrecioBase = precioBase;
        CapacidadMaxima = capacidadMaxima;
        CuposDisponibles = capacidadMaxima;
    }

    public int Id { get; }
    public string Nombre { get; }
    public DateTime Fecha { get; }
    public decimal PrecioBase { get; }
    public int CapacidadMaxima { get; }
    public int CuposDisponibles { get; private set; }
    public abstract decimal CalcularPrecioFinal();

    internal void ReservarCupos(int cantidad)
    {
        if (cantidad <= 0) throw new ArgumentOutOfRangeException(nameof(cantidad), "La cantidad debe ser mayor que cero.");
        if (cantidad > CuposDisponibles) throw new InvalidOperationException($"Solo quedan {CuposDisponibles} cupos disponibles.");
        CuposDisponibles -= cantidad;
    }

    internal void DevolverCupos(int cantidad)
    {
        if (cantidad <= 0 || CuposDisponibles + cantidad > CapacidadMaxima)
            throw new InvalidOperationException("La devolución excede la capacidad del evento.");
        CuposDisponibles += cantidad;
    }
}
