import React, { useState, useEffect, useRef } from 'react';
import { View, StyleSheet, Dimensions, TouchableOpacity, Text, Platform, ActivityIndicator } from 'react-native';
import { MaterialIcons } from '@expo/vector-icons';

const HEIGHT = Dimensions.get('window').height;
const WIDTH = Dimensions.get('window').width;

const apiUrl = 'http://localhost:5218/api/Peliculas';
const backendUrl = 'http://localhost:5000/api/visualizaciones';
const recommendationUrl = 'http://localhost:5001/api/recomendacion';

const VideoPlayer = () => {
  const [videoList, setVideoList] = useState([]);
  const [recommendationList, setRecommendationList] = useState([]);
  const [currentVideoIndex, setCurrentVideoIndex] = useState(0);
  const [loading, setLoading] = useState(true);
  const [elapsedTime, setElapsedTime] = useState(0);
  const intervalRef = useRef(null);
  const startTimeRef = useRef(null);

  useEffect(() => {
    const fetchVideos = async () => {
      try {
        const response = await fetch(apiUrl);
        const data = await response.json();
        setVideoList(shuffleArray(data)); // Shuffle videos for random order
        setLoading(false);
      } catch (error) {
        console.error(error);
        setLoading(false);
      }
    };

    fetchVideos();

    const fetchRecommendation = async () => {
      try {
        const response = await fetch(recommendationUrl);
        const data = await response.json();
        if (data.first_recommendation_id && data.second_recommendation_id) {
          const recommendedVideo1 = await fetch(`${apiUrl}/${data.first_recommendation_id}`);
          const recommendedData1 = await recommendedVideo1.json();
          const recommendedVideo2 = await fetch(`${apiUrl}/${data.second_recommendation_id}`);
          const recommendedData2 = await recommendedVideo2.json();
          setRecommendationList([recommendedData1, recommendedData2]); // Store recommended videos
        }
      } catch (error) {
        console.error('Error al obtener la recomendación:', error);
      }
    };

    fetchRecommendation();

    // Actualizar recomendaciones en intervalos regulares
    const recommendationInterval = setInterval(() => {
      fetchRecommendation();
    }, 5000); // Cada 5 segundos

    return () => clearInterval(recommendationInterval);
  }, []);

  useEffect(() => {
    // Limpiar cualquier intervalo anterior
    if (intervalRef.current) {
      clearInterval(intervalRef.current);
    }

    // Reiniciar el tiempo transcurrido y guardar el tiempo de inicio
    setElapsedTime(0);
    startTimeRef.current = Date.now();

    // Establecer un nuevo intervalo para actualizar el tiempo transcurrido
    intervalRef.current = setInterval(() => {
      const elapsed = Math.min(Math.floor((Date.now() - startTimeRef.current) / 1000) - 2, 15); // Restar 2 segundos y limitar a 15 segundos
      setElapsedTime(elapsed);
    }, 1000);

    // Limpiar el intervalo al desmontar el componente o cambiar de video
    return () => {
      clearInterval(intervalRef.current);
    };
  }, [currentVideoIndex]);

  const handleNextVideo = () => {
    // Enviar el tiempo de visualización actual antes de cambiar de video
    sendTimeDataToBackend(elapsedTime);

    // Reiniciar el tiempo transcurrido y cambiar al siguiente video
    clearInterval(intervalRef.current);
    setCurrentVideoIndex((prevIndex) => {
      if (prevIndex + 1 < 2) {
        // Mantener los primeros dos videos aleatorios
        return prevIndex + 1;
      } else if (recommendationList.length > 0) {
        // Usar recomendaciones a partir del tercer video
        if (prevIndex - 1 < recommendationList.length) {
          setVideoList(recommendationList);
          return prevIndex - 1; // Continuar con la siguiente recomendación
        }
        return prevIndex + 1;
      }
      return prevIndex + 1;
    });
  };

  const sendTimeDataToBackend = async (elapsedTime) => {
    const videoId = videoList[currentVideoIndex]?.Id;
    console.log('ID del video:', videoId);
    console.log('Tiempo de visualización:', elapsedTime);

    try {
      await fetch(backendUrl, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({ usuario_id: 'id_del_usuario', tiempo_visto: elapsedTime, nombre_pelicula: videoId }),
      });
    } catch (error) {
      console.error('Error al enviar el tiempo de visualización:', error);
    }
  };

  const shuffleArray = (array) => {
    for (let i = array.length - 1; i > 0; i--) {
      const j = Math.floor(Math.random() * (i + 1));
      [array[i], array[j]] = [array[j], array[i]];
    }
    return array;
  };

  const renderVideo = () => {
    if (loading) {
      return <ActivityIndicator size="large" color="#0000ff" />;
    }

    if (videoList.length === 0) {
      return <Text>No hay videos disponibles</Text>;
    }

    const videoId = videoList[currentVideoIndex]?.VideoURL;
    const videoUrl = `https://www.youtube.com/embed/${videoId}?autoplay=1&end=15`;

    if (Platform.OS === 'web') {
      return (
        <iframe
          width={WIDTH - 200}
          height={HEIGHT - 150}
          src={videoUrl}
          frameBorder="0"
          allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture"
          allowFullScreen
          title="YouTube video"
        />
      );
    } else {
      return (
        <View style={{ width: WIDTH - 200, height: HEIGHT - 150 }}>
          <Text>La reproducción de videos no es compatible en esta plataforma.</Text>
        </View>
      );
    }
  };

  return (
    <View style={styles.container}>
      {renderVideo()}
      {!loading && videoList.length > 0 && (
        <TouchableOpacity onPress={handleNextVideo} style={styles.iconContainer}>
          <MaterialIcons name="skip-next" size={24} color="black" />
          <Text>Siguiente</Text>
        </TouchableOpacity>
      )}
    </View>
  );
};

const styles = StyleSheet.create({
  container: {
    flex: 1,
    justifyContent: 'center',
    alignItems: 'center'
  },
  iconContainer: {
    position: 'absolute',
    bottom: 20,
    right: 20,
    flexDirection: 'row',
    alignItems: 'center'
  }
});

export default VideoPlayer;
