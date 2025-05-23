# ModelSecurity

Sistema de gestión y seguridad para entornos educativos, enfocado en la administración de aprendices, instructores, programas y procesos formativos.

## Descripción

ModelSecurity es una aplicación web desarrollada en .NET que proporciona una plataforma para la gestión de procesos educativos, incluyendo el seguimiento de aprendices, la asignación de instructores a programas, la gestión de sedes y regionales, y un sistema completo de seguridad y control de acceso mediante roles y permisos.

## Estructura del Proyecto

El proyecto está organizado siguiendo una arquitectura de capas, con los siguientes componentes:

### Business

Contiene la lógica de negocio de la aplicación.

- Implementa los servicios y operaciones de negocio
- Realiza validaciones
- Coordina el flujo de datos entre la capa de presentación y la capa de acceso a datos
- Cada entidad del sistema tiene su correspondiente clase Business (AprendizBusiness, InstructorBusiness, etc.)

### Data

Maneja el acceso a datos y la comunicación con la base de datos.

- Implementa los repositorios para cada entidad
- Contiene las consultas y operaciones CRUD a la base de datos
- Utiliza patrones de acceso a datos para abstraer la fuente de datos
- Cada entidad tiene su correspondiente clase Data (AprendizData, InstructorData, etc.)

### Entity

Define todas las entidades (modelos) del sistema y los DTOs (Data Transfer Objects).

- **Model**: Contiene las clases de entidad que representan las tablas de la base de datos
- **DTOs**: Objetos de transferencia de datos utilizados para la comunicación entre capas
- **Enum**: Enumeraciones utilizadas en el sistema, como tipos de permisos
- **Context**: Contextos de Entity Framework para la conexión a la base de datos

### Diagram

Contiene el modelo de datos y diagramas de la base de datos.

- Incluye archivos EDMX que representan el modelo de Entity Framework
- Proporciona una representación visual de las relaciones entre entidades

### Utilities

Proporciona utilidades y funcionalidades comunes utilizadas en todo el proyecto.

- **Exceptions**: Definiciones de excepciones personalizadas (ValidationException, EntityNotFoundException, etc.)
- Contiene clases de ayuda y extensiones utilizadas en todo el proyecto

### Web

Capa de presentación y API de la aplicación.

- **Controllers**: Controladores de API REST para cada entidad del sistema
- Maneja las solicitudes HTTP y devuelve respuestas JSON
- Implementa la autenticación y autorización
- Define los endpoints de la API

## Entidades Principales

- **Aprendiz**: Estudiantes del sistema educativo
- **Instructor**: Profesores y formadores
- **Program**: Programas educativos
- **Process**: Procesos formativos
- **Center**: Centros de formación
- **Regional**: Regionales administrativas
- **Sede**: Sedes educativas
- **User**: Usuarios del sistema
- **Rol**: Roles de usuario con permisos específicos
- **Form**: Formularios y módulos del sistema
- **Module**: Módulos funcionales de la aplicación

## Relaciones entre Entidades

El sistema implementa las siguientes relaciones entre entidades:

### Relaciones Uno a Uno (1:1)
- **User - Person**: Cada usuario está asociado a una persona
- **Aprendiz - User**: Cada aprendiz tiene un usuario asociado
- **Instructor - User**: Cada instructor tiene un usuario asociado

### Relaciones Uno a Muchos (1:N)
- **Center - Sede**: Un centro puede tener múltiples sedes
- **Regional - Center**: Una regional puede tener múltiples centros
- **Center - Regional**: Cada centro pertenece a una regional

### Relaciones Muchos a Muchos (N:N)
- **User - Rol**: A través de la entidad intermedia UserRol
- **User - Sede**: A través de la entidad intermedia UserSede
- **Aprendiz - Program**: A través de la entidad intermedia AprendizProgram
- **Instructor - Program**: A través de la entidad intermedia InstructorProgram
- **Form - Module**: A través de la entidad intermedia FormModule
- **Rol - Form**: A través de la entidad intermedia RolForm

### Relaciones Complejas
- **AprendizProcessInstructor**: Relación compleja que conecta aprendices, instructores, procesos y otros elementos relacionados

## Propiedades de Navegación

Cada entidad incluye propiedades de navegación que permiten acceder a las entidades relacionadas:

### Ejemplos de Implementación

```csharp
// Relación 1:1
public class User
{
    public int Id { get; set; }
    public Person Person { get; set; }
}

// Relación 1:N
public class Center
{
    public int Id { get; set; }
    public ICollection<Sede> Sedes { get; set; }
}

// Relación N:N
public class UserRol
{
    public int Id { get; set; }
    public User User { get; set; }
    public Rol Rol { get; set; }
}
```

## Tecnologías Utilizadas

- .NET 8.0
- Entity Framework Core
- SQL Server
- Dapper (para algunas consultas de rendimiento)
- ASP.NET Core Web API
- Patrón Repositorio
- Arquitectura en capas

## Seguridad

El sistema implementa un completo modelo de seguridad basado en:

- Usuarios con roles asignados
- Roles con permisos específicos para operaciones CRUD
- Asignación de usuarios a sedes específicas
- Control de acceso a formularios y módulos según el rol 

## Documentación Detallada

- [Estructura de Base de Datos](Entity/DDL/README.md)

# ModelSecurity - Documentación de Cambios

## Cambios en el Contexto de Base de Datos

### Resolución de Ambigüedad en Module

Se ha realizado una modificación importante en el archivo `Entity/Context/ApplicationDbContext.cs` para resolver un problema de ambigüedad en la referencia al tipo `Module`.

#### Problema Original
Existía una ambigüedad en la referencia al tipo `Module` debido a que este nombre puede referirse tanto a:
- `Entity.Model.Module` (nuestro modelo de dominio)
- `System.Reflection.Module` (tipo del framework .NET)

#### Solución Implementada
Se modificó la declaración del DbSet para usar el nombre completo del tipo:

```csharp
// Antes
public DbSet<Module> Module { get; set; }

// Después
public DbSet<Entity.Model.Module> Module { get; set; }
```

#### Impacto
- Se resolvió el error de compilación relacionado con la ambigüedad
- No hay impacto en la funcionalidad existente
- Mejora la claridad del código al especificar explícitamente el namespace del tipo

### Estructura del Contexto

El `ApplicationDbContext` mantiene las siguientes características principales:

1. **Configuración de Entidades**
   - Gestión de roles y usuarios
   - Manejo de formularios y módulos
   - Control de procesos y programas
   - Administración de sedes y centros

2. **Relaciones Configuradas**
   - Relaciones uno a uno (1:1)
   - Relaciones uno a muchos (1:N)
   - Relaciones muchos a muchos (N:M)

3. **Características Adicionales**
   - Soporte para consultas personalizadas con Dapper
   - Auditoría automática de cambios
   - Configuración de precisión decimal
   - Logging de datos sensibles

## Notas de Implementación

- Se mantiene la compatibilidad con el código existente
- No se requieren cambios en las migraciones de base de datos
- La documentación XML se mantiene actualizada

## Cambios Recientes / Mejoras (CRUD y Borrado Lógico)

En las últimas sesiones de trabajo, se ha implementado la funcionalidad completa de **CRUD (Create, Read, Update, Delete)** y **Borrado Lógico (Soft Delete)** para varias entidades clave del sistema.

Los siguientes controladores ahora exponen los endpoints estándar:

*   `GET /` (Obtener todos)
*   `GET /{id}` (Obtener por ID)
*   `POST /` (Crear)
*   `PUT /{id}` (Actualizar completo)
*   `PATCH /{id}` (Actualizar parcial)
*   `DELETE /{id}` (Eliminar persistente)
*   `DELETE /{id}/soft` (Borrado lógico / Desactivar)

**Controladores Actualizados:**

*   `AprendizProgramController`: Se corrigió la implementación de `PATCH` para usar `AprendizProgramDto` y se verificó la existencia de todos los endpoints CRUD + Soft Delete. Se añadió el método `SoftDeleteAsync` a `AprendizProgramData`.
*   `CenterController`: Se añadieron los endpoints `PUT`, `PATCH`, `DELETE` y `DELETE /soft`. Se corrigió la implementación de `PATCH` en el controlador y en `CenterBusiness` para usar `CenterDto`. Se añadieron los métodos correspondientes a `CenterBusiness`.
*   `ConceptController`: Se añadieron los endpoints `PUT`, `PATCH`, `DELETE` y `DELETE /soft`. Se implementaron los métodos correspondientes en `ConceptBusiness`.
*   `EnterpriseController`: Se añadieron los endpoints `PUT`, `PATCH`, `DELETE` y `DELETE /soft`. Se implementaron los métodos correspondientes en `EnterpriseBusiness`.
*   `UserRolController`: Se creó el controlador con los endpoints `GET`, `POST`, `PUT` y `DELETE`. Se corrigió e implementó la funcionalidad de borrado lógico (`SoftDeleteUserRolAsync` en `UserRolBusiness`, `SoftDeleteAsync` en `UserRolData`) y se añadió el endpoint `DELETE /{id}/soft`.

**Consideraciones:**

*   La implementación de `PATCH` se ha estandarizado para recibir el DTO principal de la entidad (ej. `ConceptDto`) en lugar de objetos `JsonPatchDocument` o DTOs específicos para PATCH, simplificando la interacción con la API. La lógica de actualización parcial reside en la capa de negocio.
*   El borrado lógico (`soft delete`) se implementa generalmente cambiando un campo `Active` a `false` y opcionalmente registrando la fecha en `DeleteDate` en la entidad correspondiente.
