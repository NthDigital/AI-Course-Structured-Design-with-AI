import axios from 'axios';

// Function to get the API base URL using multiple strategies
const getApiBaseUrl = (): string => {
  // Strategy 1: Check for explicit environment variable
  if (process.env.REACT_APP_API_URL) {
    console.log('Using explicit REACT_APP_API_URL:', process.env.REACT_APP_API_URL);
    return process.env.REACT_APP_API_URL;
  }

  // Strategy 2: Use service name resolution (works when both are in Aspire)
  // When React app has a reference to 'api' service, try to resolve it
  const serviceBasedUrl = `${window.location.protocol}//${window.location.hostname}:7101`;
  
  // Strategy 3: Development fallback URLs based on typical Aspire patterns
  const developmentUrls = [
    'https://localhost:7101', // Common HTTPS port
    'http://localhost:5213',  // Common HTTP port
    'https://localhost:7027', // Alternative HTTPS port
  ];

  // In development, try localhost with standard ports
  if (window.location.hostname === 'localhost' || window.location.hostname === '127.0.0.1') {
    console.log('Using development API URL:', developmentUrls[0]);
    return developmentUrls[0];
  }

  // Strategy 4: Production - assume API is on same host, different port or path
  const productionUrl = `${window.location.protocol}//${window.location.hostname}:7101`;
  console.log('Using production-style API URL:', productionUrl);
  return productionUrl;
};

const API_BASE_URL = getApiBaseUrl();
console.log('Final API Base URL:', API_BASE_URL);

const apiClient = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
  // Add timeout and retry logic
  timeout: 10000,
});

// Request interceptor to add auth token
apiClient.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('authToken');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

// Response interceptor for error handling
apiClient.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      localStorage.removeItem('authToken');
      window.location.href = '/login';
    }
    return Promise.reject(error);
  }
);

export default apiClient;
