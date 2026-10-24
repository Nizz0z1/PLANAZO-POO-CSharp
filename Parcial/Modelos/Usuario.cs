namespace Planazo.Modelos;

public sealed class Usuario
{
    public Usuario(int id, string nombre)
    {
        if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id));
        if (string.IsNullOrWhiteSpace(nombre)) throw new ArgumentException("El nombre es obligatorio.", nameof(nombre));
        Id = id;
        Nombre = nombre.Trim();
    }

    public int Id { get; }
    public string Nombre { get; }
}
