import axios from 'axios';

const API_BASE_URL = 'http://localhost:5258/api';

const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

export const tasksApi = {
  getAllTasks: () => api.get('/tasks'),
  
  getTaskById: (id) => api.get(`/tasks/${id}`),
  
  createTask: (task) => api.post('/tasks', task),
  
  updateTask: (id, task) => api.put(`/tasks/${id}`, task),
  
  deleteTask: (id) => api.delete(`/tasks/${id}`),
  
  updateTaskStatus: (id, status) => api.patch(`/tasks/${id}/status`, { status }),
  
  getTasksByStatus: (status) => api.get(`/tasks/by-status/${status}`),
  
  getRecentTasks: () => api.get('/tasks/recent'),
  
  getTaskStatistics: () => api.get('/tasks/statistics'),
};

export default api;


