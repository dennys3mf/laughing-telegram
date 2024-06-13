from flask import Blueprint

gestion_pelis_usuarios_bp = Blueprint('gestion_pelis_usuarios_bp', __name__)

from . import gestion_pelis_usuarios
