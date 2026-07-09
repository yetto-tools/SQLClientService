---
_layout: landing
---

# DBSQLClient

Cliente ligero sobre `Microsoft.Data.SqlClient` para ejecutar queries y procedimientos
almacenados en SQL Server, con mapeo automático a objetos, builder de parámetros (`SqlParams`)
y manejo simplificado de parámetros de salida.

## Empieza aquí

- [Introducción](Docs/introduction.md) — qué resuelve la librería y sus piezas principales.
- [Getting Started](Docs/getting-started.md) — instalación y primeros ejemplos.
- [Referencia API](api/toc.yml) — documentación generada desde el código fuente.

## Instalación rápida

```bash
dotnet add package DBSQLClient
```

```csharp
using DBSQLClient.Conexion;

var service = new SqlClientService(connectionString);
var result = await service.QueryAsync("SELECT * FROM Users WHERE IsActive = 1");
var users = result.ToList<User>();
```
