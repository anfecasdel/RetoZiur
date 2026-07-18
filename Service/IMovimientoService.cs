using RetoZiur.Models;

namespace RetoZiur.Services;

public interface IMovimientoService
{
    Task<List<TipoMovimiento>> ObtenerMovimientosAsync(CancellationToken cancellationToken = default);
}
