import React, { useEffect, useState } from 'react';
import apiClient from '../services/api';

interface WeatherForecast {
  date: string;
  temperatureC: number;
  temperatureF: number;
  summary: string;
}

const WeatherTest: React.FC = () => {
  const [weather, setWeather] = useState<WeatherForecast[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchWeather = async () => {
      try {
        const response = await apiClient.get<WeatherForecast[]>('/weatherforecast');
        setWeather(response.data);
      } catch (err) {
        setError('Failed to fetch weather data. Make sure the API is running.');
      } finally {
        setLoading(false);
      }
    };

    fetchWeather();
  }, []);

  if (loading) return <div className="text-center">Loading...</div>;
  if (error) return <div className="text-red-600 text-center">{error}</div>;

  return (
    <div className="bg-white shadow rounded-lg p-6">
      <h3 className="text-lg font-medium text-gray-900 mb-4">Weather Forecast (API Test)</h3>
      <div className="grid gap-4">
        {weather.map((forecast, index) => (
          <div key={index} className="border border-gray-200 rounded p-4">
            <div className="flex justify-between items-center">
              <span className="font-medium">{forecast.date}</span>
              <span className="text-blue-600">{forecast.summary}</span>
            </div>
            <div className="text-sm text-gray-600">
              {forecast.temperatureC}°C / {forecast.temperatureF}°F
            </div>
          </div>
        ))}
      </div>
    </div>
  );
};

export default WeatherTest;
