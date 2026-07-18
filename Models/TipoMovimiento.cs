namespace RetoZiur.Models;

public class TipoMovimiento
{
    public int Codigo { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public bool VActiva { get; set; }
}