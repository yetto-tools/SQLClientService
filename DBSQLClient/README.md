#OtherInformation: <Nullable>enable</Nullable>
[]: # OtherInformation: <ImplicitUsings>enable</ImplicitUsings>

```csharp 
# Configuración avanzada para formateo salida del JSON (JsonSerializerOptions) 
# ============================================


var options = new JsonSerializerOptions
{
    // ============================================
    // FORMATO Y PRESENTACIÓN
    // ============================================
    
    // Formato con indentación (más legible)
    WriteIndented = true,  // Por defecto: true en tu clase
    
    // Tamaño de la indentación
    IndentCharacter = ' ',  // Carácter para indentar (espacio por defecto)
    IndentSize = 2,         // Número de caracteres (2 espacios por defecto)
    
    // ============================================
    // NOMENCLATURA DE PROPIEDADES
    // ============================================
    
    // Convierte nombres de propiedades
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,  // "Name" → "name"
    // Opciones: null (sin cambios), JsonNamingPolicy.CamelCase, JsonNamingPolicy.SnakeCaseLower, JsonNamingPolicy.SnakeCaseUpper, JsonNamingPolicy.KebabCaseLower, JsonNamingPolicy.KebabCaseUpper

    
    // Convierte nombres de diccionarios
    DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
    
    // ============================================
    // MANEJO DE VALORES
    // ============================================
    
    // Ignora propiedades null al escribir
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    // Opciones:
    // - Never: Incluye todo
    // - Always: Ignora todo
    // - WhenWritingNull: Ignora solo null
    // - WhenWritingDefault: Ignora valores por defecto (0, false, null, etc.)
    
    // Permite comentarios en JSON al leer
    ReadCommentHandling = JsonCommentHandling.Skip,
    
    // Permite comas finales
    AllowTrailingCommas = true,
    
    // ============================================
    // NÚMEROS Y TIPOS ESPECIALES
    // ============================================
    
    // Permite leer números como strings
    NumberHandling = JsonNumberHandling.AllowReadingFromString,
    // Opciones:
    // - Strict: Solo números
    // - AllowReadingFromString: "123" → 123
    // - WriteAsString: 123 → "123"
    // - AllowNamedFloatingPointLiterals: Permite NaN, Infinity
    
    // ============================================
    // CASE SENSITIVITY
    // ============================================
    
    // Ignora mayúsculas/minúsculas al deserializar
    PropertyNameCaseInsensitive = true,  // Por defecto: true en tu clase
    
    // ============================================
    // ENCODING Y CARACTERES
    // ============================================
    
    // Codifica caracteres HTML (<, >, &, ', ")
    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    // Opciones:
    // - JavaScriptEncoder.Default: Codifica todo
    // - JavaScriptEncoder.UnsafeRelaxedJsonEscaping: Más permisivo
    
    // ============================================
    // PROFUNDIDAD Y TAMAÑO
    // ============================================
    
    // Profundidad máxima de objetos anidados
    MaxDepth = 64,  // Por defecto: 64
    
    // Tamaño máximo del buffer
    DefaultBufferSize = 16384,  // 16KB por defecto
    
    // ============================================
    // REFERENCIAS Y CICLOS
    // ============================================
    
    // Manejo de referencias circulares
    ReferenceHandler = ReferenceHandler.IgnoreCycles,
    // Opciones:
    // - null: Error en ciclos
    // - ReferenceHandler.Preserve: Usa $id y $ref
    // - ReferenceHandler.IgnoreCycles: Ignora referencias circulares
    
    // ============================================
    // CONVERSORES PERSONALIZADOS
    // ============================================
    
    // Agregar conversores personalizados
    Converters = 
    {
        new JsonStringEnumConverter(),  // Convierte enums a strings
        // Puedes agregar tus propios conversores aquí
    },
    
    // ============================================
    // OTRAS OPCIONES
    // ============================================
    
    // Permite campos además de propiedades
    IncludeFields = false,
    
    // Ignora propiedades de solo lectura al serializar
    IgnoreReadOnlyProperties = false,
    
    // Ignora propiedades de solo lectura al deserializar
    IgnoreReadOnlyFields = false,
    
    // Orden de las propiedades
    PreferredObjectCreationHandling = JsonObjectCreationHandling.Replace,
    
    // Respeta [Required] attributes
    RespectRequiredConstructorParameters = false,
    
    // Respeta atributos de validación
    RespectNullableAnnotations = false,
    
    // Permite referencias no resueltas
    UnmappedMemberHandling = JsonUnmappedMemberHandling.Skip,
    
    // Comportamiento de strings desconocidas en enums
    UnknownTypeHandling = JsonUnknownTypeHandling.JsonElement
};



// ============================================
// Ejemplo 1: JSON Compacto (sin indentación)
// ============================================
var compactOptions = new JsonSerializerOptions
{
    WriteIndented = false
};
string compact = result.ToJson(compactOptions);
// Resultado: [{"Id":1,"Name":"Juan"},{"Id":2,"Name":"María"}]

// ============================================
// Ejemplo 2: CamelCase (estilo JavaScript)
// ============================================
var camelCaseOptions = new JsonSerializerOptions
{
    WriteIndented = true,
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
};
string camelJson = result.ToJson(camelCaseOptions);
/* Resultado:
[
  {
    "id": 1,
    "name": "Juan",
    "email": "juan@email.com"
  }
]
*/

// ============================================
// Ejemplo 3: Incluir valores null y por defecto
// ============================================
var includeAllOptions = new JsonSerializerOptions
{
    WriteIndented = true,
    DefaultIgnoreCondition = JsonIgnoreCondition.Never
};
string withNulls = result.ToJson(includeAllOptions);
/* Resultado:
[
  {
    "Id": 1,
    "Name": "Juan",
    "Email": null,
    "IsActive": false
  }
]
*/

// ============================================
// Ejemplo 4: Manejo especial de números
// ============================================
var numberOptions = new JsonSerializerOptions
{
    WriteIndented = true,
    NumberHandling = JsonNumberHandling.WriteAsString
};
string numbersAsStrings = result.ToJson(numberOptions);
/* Resultado:
[
  {
    "Id": "1",
    "Price": "99.99",
    "Name": "Producto"
  }
]
*/

// ============================================
// Ejemplo 5: Manejo de referencias circulares
// ============================================
var cyclicOptions = new JsonSerializerOptions
{
    WriteIndented = true,
    ReferenceHandler = ReferenceHandler.IgnoreCycles
};
string safeJson = result.ToJson(cyclicOptions);

// ============================================
// Ejemplo 6: Caracteres especiales sin codificar
// ============================================
var relaxedOptions = new JsonSerializerOptions
{
    WriteIndented = true,
    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
};
string unescaped = result.ToJson(relaxedOptions);
// "Nombre": "José & María" (en lugar de "Jos\u00E9 & Mar\u00EDa")

// ============================================
// Ejemplo 7: Configuración para APIs REST
// ============================================
var apiOptions = new JsonSerializerOptions
{
    WriteIndented = false,  // Compacto para red
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,  // Estándar JavaScript
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,  // Menos datos
    PropertyNameCaseInsensitive = true,  // Flexible en lectura
    Converters = { new JsonStringEnumConverter() }  // Enums legibles
};
string apiJson = result.ToJson(apiOptions);

// ============================================
// Ejemplo 8: Configuración para logs/debug
// ============================================
var debugOptions = new JsonSerializerOptions
{
    WriteIndented = true,  // Legible
    DefaultIgnoreCondition = JsonIgnoreCondition.Never,  // Ver todo
    ReferenceHandler = ReferenceHandler.IgnoreCycles,  // Evitar errores
    MaxDepth = 10  // Limitar profundidad
};
string debugJson = result.ToJson(debugOptions);






// ============================================
// Configuración por defecto reutilizable
private static readonly JsonSerializerOptions DefaultJsonOptions = new JsonSerializerOptions
{
    PropertyNameCaseInsensitive = true,  // Flexible al leer
    WriteIndented = true,  // Formato legible
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,  // No incluir nulls
    Converters = { new JsonStringEnumConverter() }  // Enums como texto
};
```

## Mapeo de relaciones (SqlResultMapper)

`SqlQueryResult` puede mapearse a objetos con relaciones (uno-a-uno, uno-a-muchos, muchos-a-uno
y muchos-a-muchos) usando los métodos de extensión en `DBSQLClient.Servicio.Mapper.RelationsMapper`.

Declara las relaciones en tus modelos con atributos:

```csharp
public class User
{
    [PrimaryKey]
    [Column("user_id")]
    public int Id { get; set; }

    [Column("user_name")]
    public string Name { get; set; }

    [OneToOne(typeof(UserProfile))]
    public UserProfile Profile { get; set; }

    [OneToMany(typeof(Order))]
    public List<Order> Orders { get; set; } = new();

    [ManyToMany(typeof(Role), typeof(UserRole))]
    public List<Role> Roles { get; set; } = new();
}

public class Order
{
    [PrimaryKey]
    [Column("order_id")]
    public int Id { get; set; }

    [ForeignKey(typeof(User))]
    [Column("user_id")]
    public int UserId { get; set; }

    [ManyToOne(typeof(User))]
    public User User { get; set; }
}
```

`[Column("nombre_columna")]` es opcional: si no se indica, se usa el nombre de la propiedad tal
cual viene en el `DataTable`. `[PrimaryKey]` y `[ForeignKey]` permiten que el mapper resuelva las
claves automáticamente al agrupar padres con hijos o al cruzar tablas de unión.

Con las relaciones declaradas, el nombre de la propiedad de navegación y las claves se resuelven
solas — no hace falta pasarlas como string:

```csharp
// Tabla 0 = User, tabla 1 = UserProfile
var user = result.MapOneToOne<User, UserProfile>();

// Tabla 0 = User, tabla 1 = Orders
var user = result.MapOneToMany<User, Order>();

// Tabla 0 = Orders, tabla 1 = User (un único padre compartido por todos los hijos)
var orders = result.MapManyToOne<Order, User>();

// Tabla 0 = Users, tabla 1 = Roles, tabla 2 = UserRole (tabla de unión)
var users = result.MapManyToMany<User, Role, UserRole>();

// Varios padres, cada uno con sus propios hijos (tabla 0 = Users, tabla 1 = Orders)
var users = result.OneToManyMultiple<User, Order>();
```

Si tus nombres de propiedad o de claves no siguen la convención de los atributos (o el modelo no
tiene atributos), todos los métodos aceptan los nombres explícitos como parámetros opcionales,
por ejemplo `result.MapOneToMany<User, Order>("Orders")` o
`result.MapManyToMany<User, Role, UserRole>("Roles", leftKey: "Id", joinLeftKey: "UserId", joinRightKey: "RoleId")`.

## Origen del modelo: [Table] y [StoredProcedure]

Estos atributos de clase documentan de dónde vienen los datos de un modelo. Por ahora son solo
metadata (consultable por reflexión vía `MetadataCache`): `SqlClientService.QueryAsync`/`ExecuteAsync`
siguen recibiendo el nombre de la tabla o del procedimiento como `string`, no lo resuelven todavía
a partir del atributo.

```csharp
// El modelo representa filas de una tabla, para lectura directa (SELECT).
[Table("Users")]
public class User { /* ... */ }

// El modelo representa el resultado de un procedimiento almacenado específico.
[StoredProcedure("sp_GetUserOrderTotal")]
public class UserOrderTotal
{
    public int UserId { get; set; }
    public int TotalOrders { get; set; }
}
```

Son mutuamente excluyentes: un modelo no puede tener ambos a la vez. `MetadataCache` lanza
`InvalidOperationException` si detecta los dos atributos en el mismo tipo.

## Excluir propiedades del mapeo: [NotMapped]

Marca con `[NotMapped]` cualquier propiedad que no venga de la base de datos (por ejemplo, una
propiedad calculada). Queda completamente fuera del mapeo: no se busca su columna, no participa
como PK/FK ni en relaciones. Es necesario en propiedades de solo lectura (sin `set`), ya que de
lo contrario el mapper fallaría al intentar asignarles un valor:

```csharp
public class User
{
    public string Name { get; set; }
    public string Email { get; set; }

    [NotMapped]
    public string DisplayName => $"{Name} <{Email}>";
}
```

Aplica tanto a `SqlResultMapper` (`MapOneToOne`, `MapOneToMany`, etc.) como a `SqlQueryResult.ToList<T>()`/`FirstOrDefault<T>()`.

## Procedimientos con parámetros de salida (Output / InputOutput / ReturnValue)

Los parámetros creados con `SqlParams.OutParam`, `SqlParams.InOutParam` o `SqlParams.ReturnParam`
se combinan con los de entrada en el mismo arreglo. Después de ejecutar, el valor de salida se lee
desde el resultado (`SqlQueryResult`), no desde la variable original del parámetro:

```csharp
var parameters = SqlParams.AddParams(("UserId", 1))
    .Append(SqlParams.OutParam("TotalOrders", SqlDbType.Int))
    .ToArray();

var result = await db.ExecuteAsync("sp_GetUserOrderTotal", parameters);

var total = result.GetOutputValue<int>("TotalOrders"); // funciona con o sin "@"
bool huboSalida = result.HasOutputParameters;
foreach (var (name, value) in result.OutputParameters)
{
    Console.WriteLine($"{name} = {value}");
}
```

Esto funciona igual para `QueryAsync`/`ExecuteAsync` (async) y `Query`/`Execute` (sync). Si el
procedimiento no asigna un valor a un parámetro de salida, `GetOutputValue<T>` devuelve el valor
por defecto de `T` en vez de lanzar una excepción.

---

# ⚡ Referencia Rápida - SqlParams

## 🎯 Cheat Sheet

```csharp
using DBSQLClient.Helpers;
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
SqlParams.String(name, value, size = -1)
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

### Múltiples
```csharp
SqlParams.AddParams((name, value), ...)
SqlParams.FromDictionary(dict)
SqlParams.FromObject(obj)
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
SqlParams.String("Text", longText, -1)  // NVARCHAR(MAX)
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

## ⚠️ **Errores Comunes**

| ❌ Incorrecto | ✅ Correcto |
|--------------|-------------|
| `Param("Name", null)` | `Param("Name", value ?? null)` ✓ |
| `String("Text", text)` sin tamaño | `String("Text", text, -1)` ✓ |
| No verificar null en output | `GetOutputValue<int>(p) ?? 0` ✓ |

---

## 🚀 **Inicio Rápido - 3 Pasos**

### 1. Copia SqlParams.cs a tu proyecto

### 2. Agrega using
```csharp
using DBSQLClient.Helpers;
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
new SqlParameter("@Description", SqlDbType.NVarChar, -1) { Value = longText } // -1 = MAX
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
new SqlParameter("@FileData", SqlDbType.VarBinary, -1) { Value = fileData }
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
    new SqlParameter("@Name", SqlDbType.NVarChar).WithSize(100).Value = "Juan",
    new SqlParameter("@Price", SqlDbType.Decimal).WithPrecision(18, 2).Value = 99.99m,
    new SqlParameter("@Result", SqlDbType.Int).AsOutput()
};
```

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
new SqlParameter("@LongText", SqlDbType.NVarChar, -1) { Value = longString } // -1 = MAX
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
// ✅ AMBOS SON VÁLIDOS
new SqlParameter("@UserId", 123)  // Con @
new SqlParameter("UserId", 123)   // Sin @ (se agrega automáticamente)
```

---

## 🎯 **Mejores Prácticas**

1. **Usa parámetros siempre** - Previene SQL Injection
2. **Especifica tipos para decimales y fechas** - Evita problemas de conversión
3. **Usa -1 para NVARCHAR(MAX)** - Para textos largos
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