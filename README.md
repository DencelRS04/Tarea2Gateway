# Tarea Corta 2 — Microservicios + API Gateway (.NET + MongoDB)

Este repositorio contiene la implementación mínima de un sistema de microservicios expuestos mediante un **API Gateway**, cumpliendo los requerimientos de la tarea:

- Microservicios independientes (puertos distintos)
- Persistencia en BD **NoSQL (MongoDB)**
- Comunicación inter-servicio (Órdenes consulta Productos)
- API Gateway con entrada única, routing, políticas y observabilidad (logs)

---

##  Arquitectura

### Servicios
| Componente | Descripción | Puerto |
|----------|-------------|-------|
| `ProductosService` | Catálogo/Productos (CRUD / GET/POST mínimo) | `5001` |
| `OrdenesService` | Órdenes/Pedidos (crear/listar + consulta a productos) | `5002` |
| `ClientesService` *(opcional)* | Clientes (registrar/listar) | `5003` |
| `ApiGateway` | Gateway (YARP) | `5000` |

### Routing en Gateway
- `/api/productos/*` → ProductosService
- `/api/ordenes/*` → OrdenesService
- `/api/clientes/*` → ClientesService (si existe)
- Agregación: `/api/resumen-orden/{id}` → orden + productos + total

---

##  Requisitos (Software)

- .NET SDK 8 (o compatible con el proyecto)
- MongoDB Community Server (local) **o** MongoDB Atlas (cloud)
- Postman (recomendado)

---

##  Base de datos (MongoDB local)

Asegúrate de que MongoDB esté corriendo en:
- `mongodb://localhost:27017`

### Bases/colecciones recomendadas
- `MS_Productos` → `Productos`
- `MS_Pedidos` → `Pedidos`
- `MS_Clientes` → `Clientes` (si aplica)

Puedes crearlas en Mongo Compass o por consola:

```js
use MS_Productos
db.createCollection("Productos")

use MS_Pedidos
db.createCollection("Pedidos")

use MS_Clientes
db.createCollection("Clientes")
```
### INSTALACION Y EJECUCION
Para su instalacion debe de clonar el repositorio mediante C# en su apartado GIT, una vez hecha su correcta clonacion debe de configurar
la ejecucion de todos los proyectos existentes a la vez utilizando el multiple startup proyects y seleccionando:

ProductosService → Start
OrdenesService → Start
ClientesService → Start
ApiGateway → Start

Una vez ejecutado puede realizar las pruebas tanto en Swagger como en PostMan(RECOMENDADO) y configurar las URLs y si es necesario los json de entrada

