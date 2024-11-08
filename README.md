# TallerIDWMBackend

Este es el proyecto backend para TallerIDWM, construido con C# y .NET. El proyecto proporciona endpoints API para gestionar usuarios, productos y artículos en el carrito en una plataforma de comercio electrónico. Incluye autenticación de usuarios, operaciones CRUD y autorización basada en roles.

## Características

- **Autenticación de Usuarios**: Registro, inicio de sesión y autenticación con JWT.
- **Gestión de Perfiles**: Actualización de información del perfil del usuario.
- **Autorización por Roles**: Control de acceso para roles de `Customer` y `Admin`.
- **Gestión de Productos**: CRUD para productos y artículos del carrito.
- **Gestión de Pedidos**: Operaciones básicas para pedidos.

## Tecnologías

- **ASP.NET Core**: Framework backend.
- **Entity Framework Core**: ORM para interacción con la base de datos.
- **JWT**: Autenticación segura con tokens.
- **PostgreSQL**: Base de datos (configurable en `appsettings.json`).

## Requisitos

Antes de comenzar, asegúrate de haber cumplido con los siguientes requisitos:

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Visual Studio o Visual Studio Code]()
- [Base de datos SQL (SQL Server u otro)]()

## Instalación

1. Clona el repositorio:

   ```bash
   git clone https://github.com/xSharkz/TallerIDWMBackend.git
   ```

2. Navega al directorio del proyecto:

   ```bash
   cd TallerIDWMBackend
   ```

3. Restaura las dependencias:

   ```bash
   dotnet restore
   ```

4. Configura la conexión a la base de datos en `appsettings.json`.

5. Ejecuta la aplicación:

   ```bash
   dotnet run
   ```

## Endpoints API

### Usuarios

- `POST /api/auth/login`: Autenticar un usuario y obtener un token JWT
- `POST /api/auth/register`: Registrar un nuevo usuario
- `POST /api/auth/logout`: Cerrar la sesión actual (Admin y Customer)
- `DEL /api/auth/delete-account`: Eliminar la cuenta de la sesión actual (Admin y Customer)

- `PUT /api/user/edit-profile`: Editar el perfil de la sesión actual (Admin y Customer)
- `PUT /api/user/change-password`: Cambiar la contraseña del perfil de la sesión actual (Admin y Customer)
- `GET /api/user/customers?page=&pageSize=&searchQuery=`: Obtener todos los usuarios (solo Admin)
- `PUT /api/user/update-status`: Actualizar usuarios (solo Admin)

### Productos

- `GET /api/product/available?searchQuery=&type=&sortOrder=&pageNumber=&pageSize=`: Obtener todos los productos disponibles
- `GET /api/product/all?searchQuery&type&sortOrder=&pageNumber=&pageSize=`: Obtener todos los productos (solo Admin)
- `POST /api/product`: Crear un nuevo producto (solo Admin)
- `PUT /api/product/{id}`: Actualizar un producto (solo Admin)
- `DEL /api/product/{id}`: Eliminar un producto (solo Admin)
  
### Carrito

- `GET /api/cart/view`: Obtener el carrito del usuario actual
- `POST /api/cart/add-to-cart/{id}`: Agregar un producto al carrito
- `POST /api/cart/update-quantity/{id}`: Actualizar la cantidad de un producto del carrito
- `POST /api/cart/remove-item/{id}`: Eliminar un producto del carrito
- `POST /api/cart/checkout`: Pagar el carrito actual (Admin y Customer)

### Ordenes
- `GET /api/Order/orders?pageNumber=&pageSize=`: Obtener las ordenes del usuario (Admin y Customer)
- `GET /api/Order/orders?pageNumber=&pageSize=`: Obtener las ordenes de los usuarios (Solo Admin)

## Configuración

1. **Crear archivo `.env`** en la raíz del proyecto con las siguientes variables:

   ```env
   DB_CONNECTION_STRING=<tu_cadena_de_conexión_a_la_base_de_datos>
   JWT_SECRET_KEY=<tu_clave_secreta_para_JWT>

2. **Configurar `appsettings.json`** con la configuración de Cloudinary:
   "CloudinarySettings": {
   "CloudName": "<tu_cloud_name>",
   "ApiKey": "<tu_api_key>",
   "ApiSecret": "<tu_api_secret>"
   }