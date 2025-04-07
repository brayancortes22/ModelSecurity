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