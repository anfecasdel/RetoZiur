# RetoZiur

Aplicación Blazor Server (.NET 7) que consume una API REST externa y muestra una tabla de "Tipos de Movimiento".

## Cómo correrlo

### Requisitos
- .NET SDK 7.0

### 1. Configurar el token (user-secrets, solo desarrollo)

El token de autenticación **no** está en el repositorio, por buenas prácticas de seguridad.
Es el mismo token que Ziur Software proporcionó junto con el endpoint de la API.

El proyecto ya tiene un `UserSecretsId` configurado en el `.csproj` (necesario para que
`dotnet user-secrets` funcione), pero cada persona que clone el repo debe configurar el
valor del token localmente:

```bash
dotnet user-secrets set "MovimientoApi:Token" "<el-token-proporcionado>"
```

Esto guarda el valor en `~/.microsoft/usersecrets/<UserSecretsId>/secrets.json`, fuera de la carpeta del proyecto, por lo que nunca se sube a Git.

### 2. Configurar la URL base (opcional)

La URL base de la API vive en `appsettings.json` bajo `MovimientoApi:BaseUrl`. Si necesitás apuntar a otro ambiente, sobrescribila en `appsettings.Development.json` o con una variable de entorno:

```bash
export MovimientoApi__BaseUrl="https://otro-servidor.com"
```

### 3. Ejecutar

```bash
dotnet restore
dotnet run
```

La app queda disponible en la URL que indique la consola (por defecto algo como `https://localhost:5001`). La tabla de movimientos está en `/movimientos`.

## Configuración en producción

En producción, ni el token ni (normalmente) la URL base deberían vivir en `appsettings.json` en texto plano. Se inyectan vía variables de entorno (`MovimientoApi__Token`, con doble guion bajo) o un almacén de secretos como Azure Key Vault / AWS Secrets Manager — cualquiera de los dos se integra de forma transparente con `IConfiguration` sin cambiar código.

## Decisiones de diseño

- **`IOptions<MovimientoApiOptions>`** para leer `BaseUrl` y `Token` desde configuración, en vez de leer `IConfiguration` a mano o hardcodear valores en el código.
- **`IMovimientoService`** como interfaz mínima sobre `MovimientoService`, registrada vía `AddHttpClient<IMovimientoService, MovimientoService>` (typed client). Permite testear el componente sin llamadas HTTP reales y desacopla la UI del detalle de implementación, sin agregar capas de repositorio ni arquitectura innecesarias para el tamaño del reto.
- **Errores vs. "sin datos":** el servicio loguea (`ILogger<MovimientoService>`) y **lanza** una excepción ante fallas de red o respuestas HTTP no exitosas, en vez de devolver una lista vacía en silencio. Una lista vacía ahora significa exclusivamente "la API respondió bien y no hay datos" — nunca "algo falló".
- **`AuthenticationHeaderValue`** configurado una única vez sobre el `HttpClient` al registrarlo en `Program.cs` (no en cada request), que es el patrón recomendado para typed clients con credenciales estáticas.
- **`CancellationToken`** propagado desde el componente Razor (vía un `CancellationTokenSource` propio, cancelado en `Dispose()`) hasta la llamada HTTP, para abortar trabajo en curso si el usuario navega fuera de la página.
- **UI:** se mantienen los 4 estados explícitos (cargando / error / sin datos / con datos) del componente original, usando clases de Bootstrap ya incluidas en el proyecto en vez de estilos inline.
- **Fuera de alcance deliberadamente:** no se separaron DTOs de dominio, no se agregó una capa de repositorio, ni arquitectura por capas, ni reintentos/Polly — el tamaño del reto no lo justifica y hubiera sido sobre-ingeniería.

## Nota sobre uso de IA

Este refactor se realizó con asistencia de una herramienta de IA (Claude Code) como apoyo para la implementación y la redacción de esta documentación. Las decisiones de diseño y el alcance fueron definidos y revisados por el desarrollador.