using RetoZiur.Models;

namespace RetoZiur.Services;

public class MovimientoService : IMovimientoService
{
    private const string Endpoint = "/Ziur.API/basedatos_01/ZiurServiceRest.svc/api/DocumentosFillsCombos";

    private readonly HttpClient _http;
    private readonly ILogger<MovimientoService> _logger;

    public MovimientoService(HttpClient http, ILogger<MovimientoService> logger)
    {
        _http = http;
        _logger = logger;
    }

    public async Task<List<TipoMovimiento>> ObtenerMovimientosAsync(CancellationToken cancellationToken = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, Endpoint);

        HttpResponseMessage response;
        try
        {
            response = await _http.SendAsync(request, cancellationToken);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Fallo de red al consultar {Endpoint}", Endpoint);
            throw;
        }

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError(
                "La API de movimientos respondió {StatusCode} ({ReasonPhrase}) para {Endpoint}",
                (int)response.StatusCode, response.ReasonPhrase, Endpoint);

            throw new HttpRequestException(
                $"La API de movimientos respondió {(int)response.StatusCode} ({response.ReasonPhrase}).");
        }

        var datos = await response.Content.ReadFromJsonAsync<List<TipoMovimiento>>(cancellationToken: cancellationToken);
        return datos ?? new List<TipoMovimiento>();
    }
}