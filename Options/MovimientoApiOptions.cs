namespace RetoZiur.Options;

public class MovimientoApiOptions
{
    public const string SectionName = "MovimientoApi";

    public string BaseUrl { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
}
