from flask import Flask
from flask_cors import CORS
from api.gestion_pelis_usuarios import gestion_pelis_usuarios_bp

app = Flask(__name__)
CORS(app)  # Agrega esta línea para permitir todas las solicitudes desde cualquier origen
app.register_blueprint(gestion_pelis_usuarios_bp)

if __name__ == "__main__":
    app.run(debug=True)
