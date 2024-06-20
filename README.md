# Proyecto BookStoreApi

Este proyecto consiste en una aplicación de recomendación de películas que utiliza una API construida con .NET, un backend para manejar visualizaciones y un frontend desarrollado con React Native.

## Componentes del Proyecto

### API .NET

La API .NET es el corazón del sistema de recomendación de películas. Está desarrollada utilizando .NET 8.0 y se encarga de gestionar las películas y las recomendaciones.

Para ejecutar la API, navegue al directorio de la API y ejecute:

```bash
dotnet run
La API estará disponible en http://localhost:5218/api/Peliculas.

Backend
El backend se encarga de manejar las visualizaciones de las películas. Está desarrollado en Node.js y se comunica con la API .NET para obtener información sobre las películas.

Para iniciar el backend, navegue al directorio del backend y ejecute:

npm install
npm start
El backend estará disponible en http://localhost:5000/api/visualizaciones.

Frontend
El frontend es una aplicación móvil desarrollada con React Native. Proporciona la interfaz de usuario para interactuar con el sistema de recomendación de películas.

Para iniciar el frontend, asegúrese de tener instalado React Native y luego ejecute:

npm install
npm start
Configuración
Las URLs de la API .NET, el backend y el frontend están configuradas en el archivo App.js del frontend:

const apiUrl = 'http://localhost:5218/api/Peliculas';
const backendUrl = 'http://localhost:5000/api/visualizaciones';
const recommendationUrl = 'http://localhost:5001/api/recomendacion';
Asegúrese de que estas URLs coincidan con las configuraciones de su entorno local.

Contribuir
Para contribuir al proyecto, por favor clone el repositorio, realice sus cambios y envíe un pull request con una descripción detallada de los cambios propuestos.

Licencia
Este proyecto está licenciado bajo la Licencia MIT. Vea el archivo LICENSE para más detalles.


Este `.readme` proporciona una visión general del proyecto, incluyendo cómo ejecutar cada componente y la configuración necesaria para que el sistema funcione correctamente.
