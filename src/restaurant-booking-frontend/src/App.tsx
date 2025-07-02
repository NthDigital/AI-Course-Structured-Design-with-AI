import React, { useEffect, useState } from 'react';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import { Provider } from 'react-redux';
import { store } from './store/store';
import apiClient from './services/api';
import './App.css';

function App() {
  return (
    <Provider store={store}>
      <Router>
        <div className="min-h-screen bg-gray-100">
          <header className="bg-white shadow-sm">
            <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
              <div className="flex justify-between h-16">
                <div className="flex items-center">
                  <h1 className="text-xl font-semibold text-gray-900">
                    Restaurant Booking System
                  </h1>
                </div>
              </div>
            </div>
          </header>
          
          <main className="max-w-7xl mx-auto py-6 sm:px-6 lg:px-8">
            <Routes>
              <Route path="/" element={<Home />} />
              {/* Add more routes here */}
            </Routes>
          </main>
        </div>
      </Router>
    </Provider>
  );
}

function Home() {
  const [apiStatus, setApiStatus] = useState<string>('Checking...');
  const [apiMessage, setApiMessage] = useState<string>('');
  const [isApiWorking, setIsApiWorking] = useState<boolean | null>(null);

  useEffect(() => {
    const testApiConnection = async () => {
      try {
        console.log('Testing API connection...');
        const response = await apiClient.get('/test');
        console.log('API Response:', response.data);
        setApiStatus('Connected');
        setApiMessage(response.data.message || 'API Working');
        setIsApiWorking(true);
      } catch (error) {
        console.error('API Connection Error:', error);
        setApiStatus('Failed');
        setApiMessage('Unable to connect to API');
        setIsApiWorking(false);
      }
    };

    testApiConnection();
  }, []);

  return (
    <div className="px-4 py-6 sm:px-0">
      <div className="text-center">
        {/* Main Title */}
        <div className="mb-8">
          <h1 className="text-6xl font-bold text-green-600 mb-4">
            Restaurant Booking
          </h1>
          <h2 className="text-4xl font-semibold text-gray-800">
            Is Running
          </h2>
        </div>

        {/* Status Cards */}
        <div className="grid md:grid-cols-2 gap-6 max-w-4xl mx-auto">
          {/* Frontend Status */}
          <div className="bg-white rounded-lg shadow-lg p-6">
            <div className="flex items-center justify-center mb-4">
              <div className="w-4 h-4 bg-green-500 rounded-full mr-3"></div>
              <h3 className="text-xl font-semibold text-gray-900">Frontend</h3>
            </div>
            <p className="text-gray-600">React application is running</p>
            <p className="text-sm text-green-600 font-medium mt-2">✓ Online</p>
          </div>

          {/* API Status */}
          <div className="bg-white rounded-lg shadow-lg p-6">
            <div className="flex items-center justify-center mb-4">
              <div className={`w-4 h-4 rounded-full mr-3 ${
                isApiWorking === true ? 'bg-green-500' : 
                isApiWorking === false ? 'bg-red-500' : 'bg-yellow-500'
              }`}></div>
              <h3 className="text-xl font-semibold text-gray-900">API</h3>
            </div>
            <p className="text-gray-600">Backend API connection</p>
            <div className="mt-2">
              <p className={`text-sm font-medium ${
                isApiWorking === true ? 'text-green-600' : 
                isApiWorking === false ? 'text-red-600' : 'text-yellow-600'
              }`}>
                {isApiWorking === true ? '✓' : isApiWorking === false ? '✗' : '⏳'} {apiStatus}
              </p>
              {apiMessage && (
                <p className="text-xs text-gray-500 mt-1">{apiMessage}</p>
              )}
            </div>
          </div>
        </div>

        {/* System Info */}
        <div className="mt-8 bg-white rounded-lg shadow-lg p-6 max-w-2xl mx-auto">
          <h3 className="text-lg font-semibold text-gray-900 mb-4">System Status</h3>
          <div className="grid grid-cols-2 gap-4 text-sm">
            <div>
              <span className="font-medium text-gray-700">Frontend:</span>
              <span className="text-green-600 ml-2">React + TypeScript</span>
            </div>
            <div>
              <span className="font-medium text-gray-700">Backend:</span>
              <span className="text-blue-600 ml-2">ASP.NET Core</span>
            </div>
            <div>
              <span className="font-medium text-gray-700">Styling:</span>
              <span className="text-purple-600 ml-2">Tailwind CSS</span>
            </div>
            <div>
              <span className="font-medium text-gray-700">Orchestration:</span>
              <span className="text-orange-600 ml-2">.NET Aspire</span>
            </div>
          </div>
          
          {/* Debug Info */}
          <div className="mt-4 pt-4 border-t">
            <h4 className="text-sm font-semibold text-gray-700 mb-2">Debug Info:</h4>
            <div className="text-xs text-gray-600 space-y-1">
              <div>Current Host: {window.location.host}</div>
              <div>API URL: {process.env.REACT_APP_API_URL || 'Using auto-detection'}</div>
              <div>Environment: {process.env.NODE_ENV}</div>
            </div>
          </div>
        </div>

        {/* Ready Message */}
        <div className="mt-8">
          <p className="text-lg text-gray-600">
            🚀 Ready for development!
          </p>
        </div>
      </div>
    </div>
  );
}

export default App;
