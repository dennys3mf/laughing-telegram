import requests
import numpy as np
from sklearn.metrics.pairwise import cosine_similarity
import time

# URL de las API
movie_rating_api_url = "http://localhost:5218/api/MovieRating/"
movies_api_url = "http://localhost:5218/api/Peliculas"

def obtener_datos_api(url):
    response = requests.get(url)
    if response.status_code == 200:
        return response.json()
    else:
        print("Error al obtener datos de la API:", response.status_code)
        return []

def recomendar_peliculas():
    # Obtener datos de las API de películas y visualizaciones
    peliculas = obtener_datos_api(movies_api_url)
    visualizaciones = obtener_datos_api(movie_rating_api_url)
    
    # Verificar si hay datos disponibles para procesar
    if not peliculas or not visualizaciones:
        print("Esperando a que veas videos para hacer recomendaciones")
        return [], None
    
    # Obtener las películas vistas por el usuario
    peliculas_vistas = {}
    for v in visualizaciones:
        pelicula_id = v["PeliculaId"]
        if pelicula_id not in peliculas_vistas or v["ViewingTime"] > peliculas_vistas[pelicula_id]["ViewingTime"]:
            peliculas_vistas[pelicula_id] = v
    
    # Ordenar las películas vistas por tiempo de visualización
    peliculas_vistas_ordenadas = sorted(peliculas_vistas.values(), key=lambda x: x["ViewingTime"], reverse=True)
    
    # Mostrar las películas vistas por el usuario con mayor tiempo de visualización
    print("Películas vistas con mayor tiempo de visualización:")
    for v in peliculas_vistas_ordenadas:
        pelicula = next((p for p in peliculas if p["Id"] == v["PeliculaId"]), None)
        if pelicula:
            print("- Película:", pelicula["Titulo"])
            print("  Géneros:", [genero["GeneroNombre"] for genero in pelicula["Generos"]])
            print("  Tiempo visto:", v["ViewingTime"], "segundos")
    
    # Calcular el tiempo total que el usuario ha pasado viendo cada género
    tiempo_por_genero = {}
    for v in visualizaciones:
        pelicula = next((p for p in peliculas if p["Id"] == v["PeliculaId"]), None)
        if pelicula:
            for genero in pelicula["Generos"]:
                genero_nombre = genero["GeneroNombre"]
                tiempo_por_genero[genero_nombre] = tiempo_por_genero.get(genero_nombre, 0) + v["ViewingTime"] * genero["Peso"]
    
    # Normalizar los tiempos por género para que sumen 1
    total_tiempo = sum(tiempo_por_genero.values())
    preferencias_usuario = {genero: tiempo / total_tiempo for genero, tiempo in tiempo_por_genero.items()}
    
    # Calcular la similitud de cosenos entre las preferencias del usuario y las características de cada película
    similitudes = []
    for pelicula in peliculas:
        caracteristicas_pelicula = np.array([next((g["Peso"] for g in pelicula["Generos"] if g["GeneroNombre"] == genero), 0) for genero in preferencias_usuario.keys()]).reshape(1, -1)
        similitud = cosine_similarity([list(preferencias_usuario.values())], caracteristicas_pelicula)[0][0]
        similitudes.append((pelicula, similitud))
    
    # Ordenar las películas por similitud de cosenos (recomendación)
    recomendaciones = sorted(similitudes, key=lambda x: x[1], reverse=True)
    
    # Filtrar las películas que ya ha visto el usuario
    peliculas_vistas = {v["PeliculaId"] for v in visualizaciones}
    recomendaciones_filtradas = [pelicula for pelicula, _ in recomendaciones if pelicula["Id"] not in peliculas_vistas]
    
    # Devolver las 3 primeras recomendaciones y la película más cercana
    return recomendaciones_filtradas[:3], recomendaciones_filtradas[-1]

# Ciclo infinito para escuchar las actualizaciones en tiempo real
while True:
    recomendaciones_top3, recomendacion_cercana = recomendar_peliculas()

    # Verificar si hay recomendaciones disponibles
    if recomendaciones_top3:
        # Imprimir las recomendaciones
        print("\nRecomendaciones:")
        for pelicula in recomendaciones_top3:
            print("- Película:", pelicula["Titulo"], "(ID:", pelicula["Id"], ")")
            print("  Géneros:", [genero["GeneroNombre"] for genero in pelicula["Generos"]])
            print("--------------------------------------------------")
        print("----------------------------------------------------------------")
    else:
        # Mostrar mensaje de espera
        print("Cargando ...")

    time.sleep(3)  # Esperar 3 segundos antes de revisar las actualizaciones nuevamente



