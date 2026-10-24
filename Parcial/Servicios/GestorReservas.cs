using Planazo.Modelos;

namespace Planazo.Servicios;

public sealed record ResumenReservas(int ReservasActivas, int EntradasReservadas, decimal ValorTotal);

public sealed class GestorReservas
{
    private readonly List<Evento> _eventos = [];
    private readonly List<Usuario> _usuarios = [];
    private readonly List<Reserva> _reservas = [];
    private int _siguienteReservaId = 1;
    private int _siguienteUsuarioId = 1;

    public IReadOnlyList<Evento> Eventos => _eventos.AsReadOnly();
    public IReadOnlyList<Reserva> Reservas => _reservas.AsReadOnly();

    public void RegistrarEvento(Evento evento)
    {
        ArgumentNullException.ThrowIfNull(evento);
        if (_eventos.Any(e => e.Id == evento.Id)) throw new InvalidOperationException($"Ya existe un evento con ID {evento.Id}.");
        _eventos.Add(evento);
    }

    public Evento? BuscarEvento(int id) => _eventos.FirstOrDefault(e => e.Id == id);

    public Reserva CrearReserva(string nombreUsuario, int eventoId, int cantidad)
    {
        if (string.IsNullOrWhiteSpace(nombreUsuario)) throw new ArgumentException("El nombre de la persona es obligatorio.", nameof(nombreUsuario));
        if (cantidad <= 0) throw new ArgumentOutOfRangeException(nameof(cantidad), "La cantidad de entradas debe ser mayor que cero.");
        Evento evento = BuscarEvento(eventoId) ?? throw new ArgumentException($"No existe un evento con ID {eventoId}.", nameof(eventoId));
        Usuario? usuario = _usuarios.FirstOrDefault(u => string.Equals(u.Nombre, nombreUsuario.Trim(), StringComparison.OrdinalIgnoreCase));
        if (usuario is null)
        {
            usuario = new Usuario(_siguienteUsuarioId++, nombreUsuario.Trim());
            _usuarios.Add(usuario);
        }

        evento.ReservarCupos(cantidad);
        var reserva = new Reserva(_siguienteReservaId++, usuario, evento, cantidad);
        _reservas.Add(reserva);
        return reserva;
    }

    public void CancelarReserva(int reservaId)
    {
        Reserva reserva = _reservas.FirstOrDefault(r => r.Id == reservaId)
            ?? throw new ArgumentException($"No existe una reserva con ID {reservaId}.", nameof(reservaId));
        if (reserva.Estado == EstadoReserva.Cancelada) throw new InvalidOperationException("La reserva ya fue cancelada.");
        reserva.Evento.DevolverCupos(reserva.CantidadEntradas);
        reserva.MarcarCancelada();
    }

    public IReadOnlyList<Reserva> BuscarReservasPorUsuario(string nombre) => _reservas
        .Where(r => string.Equals(r.Usuario.Nombre, nombre.Trim(), StringComparison.OrdinalIgnoreCase))
        .ToList()
        .AsReadOnly();

    public ResumenReservas ObtenerResumen()
    {
        List<Reserva> activas = _reservas.Where(r => r.Estado == EstadoReserva.Activa).ToList();
        return new ResumenReservas(activas.Count, activas.Sum(r => r.CantidadEntradas), activas.Sum(r => r.ValorTotal));
    }
}
