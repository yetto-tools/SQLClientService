# ⚡ Referencia Rápida - SqlParams

## 🎯 Cheat Sheet

```csharp
using DBSQLClient.Servicio.Parameter;
```

---

## 📝 **Sintaxis Rápida**

### Crear Parámetros

| Código | Descripción |
|--------|-------------|
| `SqlParams.Param("Name", value)` | Parámetro simple |
| `SqlParams.AddParams(("N1", v1), ("N2", v2))` | Múltiples parámetros |
| `SqlParams.Int("Id", 123)` | Entero |
| `SqlParams.String("Name", "Juan", 100)` | String con tamaño |
| `SqlParams.Decimal("Price", 99.99m, 18, 2)` | Decimal con precisión |
| `SqlParams.DateTime("Date", DateTime.Now)` | Fecha/hora |
| `SqlParams.Bool("IsActive", true)` | Booleano |
| `SqlParams.Guid("UserId", guid)` | GUID |
| `SqlParams.OutParam("Result", SqlDbType.Int)` | Parámetro de salida |

---

## 🔥 **Ejemplos de 1 Línea**

```csharp
// Procedimiento simple
await service.ExecuteAsync("sp_GetUser", SqlParams.AddParams(("Id", 123)));

// Consulta con filtro
await service.QueryAsync("SELECT * FROM Users WHERE Id = @Id", 
    SqlParams.AddParams(("Id", userId)));

// Con múltiples parámetros
await service.ExecuteAsync("sp_UpdateUser", SqlParams.AddParams(
    ("Id", 123), ("Name", "Juan"), ("Email", "juan@email.com")
));
```

---

## 📊 **Métodos Disponibles**

### Creación Básica
```csharp
SqlParams.Param(name, value)
SqlParams.Param(name, value, type)
SqlParams.Param(name, value, type, size)
```

### Por Tipo
```csharp
SqlParams.Int(name, value)
SqlParams.String(name, value, size = SqlParams.Max)
SqlParams.Decimal(name, value, precision = 18, scale = 2)
SqlParams.DateTime(name, value)
SqlParams.DateTime2(name, value)
SqlParams.Date(name, value)
SqlParams.Bool(name, value)
SqlParams.Guid(name, value)
SqlParams.Binary(name, value)
```

### Salida
```csharp
SqlParams.OutParam(name, type)
SqlParams.InOutParam(name, value, type)
SqlParams.ReturnParam()
```

### Nulos explícitos
```csharp
SqlParams.DbNull(name, type)
SqlParams.DbNull(name, type, size)
```

### Múltiples
```csharp
SqlParams.AddParams((name, value), ...)
SqlParams.FromDictionary(dict)
SqlParams.FromObject(obj)
new SqlParameterBuilder().AddRange(parametrosYaConstruidos)
```

### Obtener Valores
```csharp
SqlParams.GetOutputValue<T>(parameter)
```

---

## 🏗️ **Builder Pattern**

```csharp
var parametros = new SqlParameterBuilder()
    .AddInt("Id", 123)
    .AddString("Name", "Juan")
    .AddDecimal("Price", 99.99m)
    .AddOutput("Result", SqlDbType.Int)
    .AddRange(SqlParams.AddParams(("Extra", "valor")))  // combina parametros ya construidos
    .Build();
```

---

## 🔧 **Extension Methods**

```csharp
param.AsOutput()
param.AsInputOutput()
param.WithSize(100)
param.WithPrecision(18, 2)
param.WithValue(value)
```

---

## 💡 **Patrones Comunes**

### Parámetro Opcional
```csharp
SqlParams.Param("Optional", value ?? null)
```

### String Largo
```csharp
SqlParams.String("Text", longText, SqlParams.Max)  // NVARCHAR(MAX)
```

### Fecha sin Hora
```csharp
SqlParams.Date("BirthDate", DateTime.Today)
```

### Valor de Salida
```csharp
var output = SqlParams.OutParam("Result", SqlDbType.Int);
await service.ExecuteAsync("sp_Proc", new[] { output });
int result = SqlParams.GetOutputValue<int>(output) ?? 0;
```

---

## 🚫 **Valores Nulos y Valores por Defecto**

Todos los métodos tipados (`Int`, `String`, `Decimal`, `DateTime`, `DateTime2`, `Date`, `Bool`,
`Guid`, `Binary`) ya convierten `null` a `DBNull.Value` automáticamente — nunca revientan por
un valor nulo. Además, aceptan un `defaultValue` opcional: si `value` es nulo, se usa
`defaultValue` en su lugar; si ambos son nulos, se envía `DBNull.Value` como antes (sin romper
el comportamiento previo).

```csharp
// Sin default: value nulo -> DBNull
SqlParams.Int("Count", null);                          // DBNull

// Con default: value nulo -> usa el default en vez de DBNull
SqlParams.Int("Count", null, defaultValue: 0);          // 0
SqlParams.String("Status", null, defaultValue: "Pendiente"); // "Pendiente"
SqlParams.Bool("Active", null, defaultValue: true);     // true
```

Cuando quieres dejar explícito en el código que un valor es intencionalmente nulo (en vez de que
`value` "termine siendo" nulo), usa `SqlParams.DbNull`:

```csharp
SqlParams.DbNull("MiddleName", SqlDbType.NVarChar);            // NULL explícito
SqlParams.DbNull("Notes", SqlDbType.NVarChar, 200);            // NULL explícito, con tamaño
new SqlParameterBuilder().AddDbNull("Optional", SqlDbType.Int).Build();
```

---

## ⚠️ **Errores Comunes**

| ❌ Incorrecto | ✅ Correcto |
|--------------|-------------|
| `Param("Name", null)` — tipo ambiguo, ADO.NET no puede inferirlo de un valor nulo | `DbNull("Name", SqlDbType.NVarChar)` ✓ |
| `value == null ? 0 : value` repetido en cada parámetro | `Int("Count", value, defaultValue: 0)` ✓ |
| `String("Text", text)` sin tamaño | `String("Text", text, SqlParams.Max)` ✓ |
| No verificar null en output | `GetOutputValue<int>(p) ?? 0` ✓ |

---

## 🚀 **Inicio Rápido - 3 Pasos**

### 1. Copia SqlParams.cs a tu proyecto

### 2. Agrega using
```csharp
using DBSQLClient.Servicio.Parameter;
```

### 3. Usa en tu código
```csharp
var parametros = SqlParams.AddParams(
    ("UserId", 123),
    ("Status", "Active")
);

var result = await service.ExecuteAsync("sp_GetUsers", parametros);
```

---

## 📚 **Más Información**

Ver ejemplos completos en: `Ejemplos de Uso - SqlParams`


--- 

# 📘 Guía Completa - Uso de SqlParameter

## ✅ **Código Actualizado**

El servicio ahora usa `SqlParameter` de Microsoft directamente. Más simple y estándar.

---

## 🎯 **Formas de Crear Parámetros (de mejor a menos recomendada)**

### **Opción 1: Constructor Simple (RECOMENDADA)** ⭐⭐⭐⭐⭐

```csharp
var parametros = new[]
{
    new SqlParameter("@pN1", 1),
    new SqlParameter("@pN2", 0),
    new SqlParameter("@pN3", 0)
};

var result = await service.ExecuteAsync("sp_MiProcedimiento", parametros);
```

**✅ Ventajas:**
- Más simple y directo
- Menos líneas de código
- Tipo se infiere automáticamente

---

### **Opción 2: Constructor con Tipo Explícito** ⭐⭐⭐⭐

```csharp
var parametros = new[]
{
    new SqlParameter("@pN1", SqlDbType.Int) { Value = 1 },
    new SqlParameter("@pN2", SqlDbType.Int) { Value = 0 },
    new SqlParameter("@pN3", SqlDbType.Int) { Value = 0 }
};
```

**Cuándo usar:**
- Tipos complejos (DateTime, Decimal, etc.)
- Para evitar ambigüedades
- Para mayor control sobre el tipo SQL

---

### **Opción 3: Object Initializer** ⭐⭐⭐

```csharp
var parametros = new[]
{
    new SqlParameter { ParameterName = "@pN1", Value = 1, SqlDbType = SqlDbType.Int },
    new SqlParameter { ParameterName = "@pN2", Value = 0, SqlDbType = SqlDbType.Int },
    new SqlParameter { ParameterName = "@pN3", Value = 0, SqlDbType = SqlDbType.Int }
};
```

---

### **Opción 4: Inline (Para casos simples)** ⭐⭐⭐⭐

```csharp
var result = await service.ExecuteAsync("sp_MiProcedimiento", new[]
{
    new SqlParameter("@pN1", 1),
    new SqlParameter("@pN2", 0),
    new SqlParameter("@pN3", 0)
});
```

---

## 📊 **Ejemplos por Tipo de Dato**

### **Enteros**
```csharp
new SqlParameter("@Id", 123)
new SqlParameter("@Count", SqlDbType.Int) { Value = 100 }
new SqlParameter("@BigNumber", SqlDbType.BigInt) { Value = 999999999L }
```

### **Cadenas**
```csharp
new SqlParameter("@Name", "Juan Pérez")
new SqlParameter("@Email", SqlDbType.NVarChar, 100) { Value = "juan@email.com" }
new SqlParameter("@Description", SqlDbType.NVarChar, SqlParams.Max) { Value = longText } // SqlParams.Max = MAX
```

### **Fechas**
```csharp
new SqlParameter("@CreatedDate", SqlDbType.DateTime) { Value = DateTime.Now }
new SqlParameter("@UpdatedDate", SqlDbType.DateTime2) { Value = DateTime.UtcNow }
new SqlParameter("@OnlyDate", SqlDbType.Date) { Value = DateTime.Today }
new SqlParameter("@OnlyTime", SqlDbType.Time) { Value = DateTime.Now.TimeOfDay }
```

### **Decimales/Dinero**
```csharp
new SqlParameter("@Price", SqlDbType.Decimal) { Value = 99.99m, Precision = 18, Scale = 2 }
new SqlParameter("@Amount", SqlDbType.Money) { Value = 1500.50m }
```

### **Booleanos**
```csharp
new SqlParameter("@IsActive", SqlDbType.Bit) { Value = true }
new SqlParameter("@HasPermission", true) // Se convierte a bit automáticamente
```

### **Valores NULL**
```csharp
new SqlParameter("@OptionalField", SqlDbType.Int) { Value = DBNull.Value }
new SqlParameter("@NullableDate", SqlDbType.DateTime) { Value = (object?)null ?? DBNull.Value }
```

### **Binarios**
```csharp
byte[] fileData = File.ReadAllBytes("image.jpg");
new SqlParameter("@FileData", SqlDbType.VarBinary, SqlParams.Max) { Value = fileData }
```

### **Uniqueidentifier (GUID)**
```csharp
new SqlParameter("@UserId", SqlDbType.UniqueIdentifier) { Value = Guid.NewGuid() }
```

### **XML**
```csharp
string xmlData = "<root><item>value</item></root>";
new SqlParameter("@XmlData", SqlDbType.Xml) { Value = xmlData }
```

---

## 🔧 **Parámetros de Salida (Output)**

```csharp
var parametros = new[]
{
    new SqlParameter("@InputId", 123),
    new SqlParameter("@OutputValue", SqlDbType.Int) { Direction = ParameterDirection.Output },
    new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.ReturnValue }
};

var result = await service.ExecuteAsync("sp_GetData", parametros);

// Leer valores de salida
int outputValue = (int)parametros[1].Value;
int returnValue = (int)parametros[2].Value;
```

---

## 🔄 **Parámetros Input/Output**

```csharp
var param = new SqlParameter("@Counter", SqlDbType.Int)
{
    Direction = ParameterDirection.InputOutput,
    Value = 10
};

var result = await service.ExecuteAsync("sp_IncrementCounter", new[] { param });

int newValue = (int)param.Value; // Valor actualizado por el SP
```

---

## 💡 **Métodos Helper Recomendados**

### **Helper Básico**

```csharp
public static class SqlParams
{
    public static SqlParameter Param(string name, object value)
    {
        return new SqlParameter(name, value ?? DBNull.Value);
    }

    public static SqlParameter Param(string name, object value, SqlDbType type)
    {
        return new SqlParameter(name, type) { Value = value ?? DBNull.Value };
    }

    public static SqlParameter OutParam(string name, SqlDbType type)
    {
        return new SqlParameter(name, type) { Direction = ParameterDirection.Output };
    }
}

// USO
var parametros = new[]
{
    SqlParams.Param("@Id", 123),
    SqlParams.Param("@Name", "Juan"),
    SqlParams.OutParam("@Result", SqlDbType.Int)
};
```

### **Helper con Fluent API**

```csharp
public static class SqlParameterExtensions
{
    public static SqlParameter AsOutput(this SqlParameter param)
    {
        param.Direction = ParameterDirection.Output;
        return param;
    }

    public static SqlParameter WithSize(this SqlParameter param, int size)
    {
        param.Size = size;
        return param;
    }

    public static SqlParameter WithPrecision(this SqlParameter param, byte precision, byte scale)
    {
        param.Precision = precision;
        param.Scale = scale;
        return param;
    }
}

// USO
var parametros = new[]
{
    new SqlParameter("@Name", SqlDbType.NVarChar).WithSize(100).WithValue("Juan"),
    new SqlParameter("@Price", SqlDbType.Decimal).WithPrecision(18, 2).WithValue(99.99m),
    new SqlParameter("@Result", SqlDbType.Int).AsOutput()
};
```

> **Más simple:** para este caso concreto no hace falta ningún `new SqlParameter` — el
> `SqlParameterBuilder` ya incluido cubre size, precisión/escala y output con sus propios métodos:
> ```csharp
> var parametros = new SqlParameterBuilder()
>     .AddString("Name", "Juan", 100)
>     .AddDecimal("Price", 99.99m)     // precision=18, scale=2 por defecto
>     .AddOutput("Result", SqlDbType.Int)
>     .Build();
> ```
> Y si ya tienes parámetros construidos por otro lado (ej: `SqlParams.AddParams(...)`), `AddRange`
> los combina en la misma cadena sin volver a escribirlos:
> ```csharp
> var parametros = new SqlParameterBuilder()
>     .AddRange(SqlParams.AddParams(("Id", 1), ("Name", "Juan")))
>     .AddOutput("Result", SqlDbType.Int)
>     .Build();
> ```

---

## 📝 **Ejemplos Completos de Uso**

### **Ejemplo 1: Consulta Simple**

```csharp
var service = new SqlClientService(connectionString);

var parametros = new[]
{
    new SqlParameter("@UserId", 123)
};

var result = await service.QueryAsync(
    "SELECT * FROM Users WHERE Id = @UserId",
    parametros
);

var users = result.ToList<User>();
```

### **Ejemplo 2: Procedimiento con Múltiples Parámetros**

```csharp
var parametros = new[]
{
    new SqlParameter("@StartDate", SqlDbType.DateTime) { Value = DateTime.Now.AddDays(-30) },
    new SqlParameter("@EndDate", SqlDbType.DateTime) { Value = DateTime.Now },
    new SqlParameter("@Status", SqlDbType.NVarChar, 50) { Value = "Active" },
    new SqlParameter("@MinAmount", SqlDbType.Decimal) { Value = 100m }
};

var result = await service.ExecuteAsync("sp_GetSalesReport", parametros);
```

### **Ejemplo 3: Con Parámetros de Salida**

```csharp
var parametros = new[]
{
    new SqlParameter("@UserId", 123),
    new SqlParameter("@TotalOrders", SqlDbType.Int) { Direction = ParameterDirection.Output },
    new SqlParameter("@TotalAmount", SqlDbType.Decimal) { Direction = ParameterDirection.Output, Precision = 18, Scale = 2 }
};

var result = await service.ExecuteAsync("sp_GetUserStatistics", parametros);

// Leer valores de salida
int totalOrders = (int)parametros[1].Value;
decimal totalAmount = (decimal)parametros[2].Value;

Console.WriteLine($"Órdenes: {totalOrders}, Total: ${totalAmount}");
```

### **Ejemplo 4: Con Valores Nullables**

```csharp
int? optionalId = null;
string? optionalName = null;

var parametros = new[]
{
    new SqlParameter("@Id", (object?)optionalId ?? DBNull.Value),
    new SqlParameter("@Name", (object?)optionalName ?? DBNull.Value)
};

var result = await service.QueryAsync("SELECT * FROM Users WHERE (@Id IS NULL OR Id = @Id)", parametros);
```

### **Ejemplo 5: Inline para Casos Simples**

```csharp
// Sin variable intermedia
var result = await service.ExecuteAsync("sp_DeleteUser", new[]
{
    new SqlParameter("@UserId", 123)
});

if (result.HasRows)
{
    Console.WriteLine("Usuario eliminado");
}
```

---

## ⚠️ **Errores Comunes y Soluciones**

### **Error 1: Tipo incorrecto**
```csharp
// ❌ INCORRECTO
new SqlParameter("@Price", DbType.Decimal) // DbType en lugar de SqlDbType

// ✅ CORRECTO
new SqlParameter("@Price", SqlDbType.Decimal)
```

### **Error 2: Tamaño de string**
```csharp
// ❌ PUEDE TRUNCAR
new SqlParameter("@LongText", longString) // Sin especificar tamaño

// ✅ CORRECTO
new SqlParameter("@LongText", SqlDbType.NVarChar, SqlParams.Max) { Value = longString } // SqlParams.Max = MAX
```

### **Error 3: Valores NULL**
```csharp
// ❌ INCORRECTO
new SqlParameter("@OptionalField", null) // Puede causar error

// ✅ CORRECTO
new SqlParameter("@OptionalField", (object?)value ?? DBNull.Value)
```

### **Error 4: Olvidar @ en el nombre**
```csharp
// ✅ AMBOS SON VÁLIDOS con este servicio
new SqlParameter("@UserId", 123)  // Con @
new SqlParameter("UserId", 123)   // Sin @
```

`SqlParameter` de Microsoft **no** agrega el `@` por sí sola — si ejecutaras este parámetro con
`SqlCommand` puro y olvidaras el `@`, fallaría en tiempo de ejecución (el nombre no coincidiría
con `@UserId` en tu SQL o procedimiento almacenado). Este servicio lo maneja internamente: tanto
`SqlParams.Param`/`Int`/`String`/etc. como `SqlCommandExecutor` (el punto por el que pasa *toda*
ejecución, incluyendo `SqlParameter` armados a mano) normalizan el nombre agregando el `@` si
falta, antes de mandarlo a SQL Server. No necesitas acordarte de ponerlo.

---

## 🎯 **Mejores Prácticas**

1. **Usa parámetros siempre** - Previene SQL Injection
2. **Especifica tipos para decimales y fechas** - Evita problemas de conversión
3. **Usa `SqlParams.Max` (-1) para NVARCHAR(MAX)** - Para textos largos, en vez del número mágico -1
4. **Maneja valores NULL correctamente** - Usa DBNull.Value
5. **Nombra parámetros con @** - Es la convención estándar
6. **Reutiliza helpers** - Crea métodos helper para casos comunes
7. **Valida tamaños** - Especifica Size para strings

---

## 📚 **Referencia Rápida de Tipos**

| Tipo C# | SqlDbType | Ejemplo |
|---------|-----------|---------|
| `int` | `Int` | `new SqlParameter("@Id", 123)` |
| `long` | `BigInt` | `new SqlParameter("@BigId", 999999L)` |
| `string` | `NVarChar` | `new SqlParameter("@Name", "Juan")` |
| `decimal` | `Decimal` | `new SqlParameter("@Price", 99.99m)` |
| `DateTime` | `DateTime` / `DateTime2` | `new SqlParameter("@Date", DateTime.Now)` |
| `bool` | `Bit` | `new SqlParameter("@IsActive", true)` |
| `Guid` | `UniqueIdentifier` | `new SqlParameter("@Guid", Guid.NewGuid())` |
| `byte[]` | `VarBinary` | `new SqlParameter("@Data", byteArray)` |

---

## ✅ **Resumen**

**Para el 90% de los casos:**
```csharp
var parametros = new[]
{
    new SqlParameter("@Param1", valor1),
    new SqlParameter("@Param2", valor2)
};

var result = await service.ExecuteAsync("sp_MiProcedimiento", parametros);
```

**Para casos complejos, crea helpers personalizados** según las necesidades de tu proyecto.

