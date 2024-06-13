from flask import Blueprint, request, jsonify
from .redis_client import redis_client

gestion_pelis_usuarios_bp = Blueprint('gestion_pelis_usuarios_bp', __name__)

@gestion_pelis_usuarios_bp.route('/api/visualizaciones', methods=['POST'])
def almacenar_visualizacion():
    data = request.json
    usuario_id = data.get('usuario_id')
    tiempo_visto = data.get('tiempo_visto')
    titulo_pelicula = data.get('nombre_pelicula')

    # Combina usuario_id y titulo_pelicula como clave en Redis
    clave_redis = f"{usuario_id}:{titulo_pelicula}"

    # Almacena temporalmente los datos de visualización en Redis como un hash
    redis_client.hset(clave_redis, 'tiempo_visto', tiempo_visto)
    redis_client.hset(clave_redis, 'ID_pelicula', titulo_pelicula)

    return jsonify({'message': 'Datos de visualización almacenados temporalmente'}), 200















