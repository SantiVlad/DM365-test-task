<template>
  <div class="task-board">
    <div class="board-header">
      <h1>Task Management System</h1>
      <div class="header-actions">
        <button @click="showCreateModal = true" class="btn btn-primary">
          + New Task
        </button>
        <button @click="loadTasks" class="btn btn-secondary">
          ↻ Refresh
        </button>
      </div>
    </div>

    <div class="statistics" v-if="statistics">
      <div class="stat-card">
        <span class="stat-label">To Do</span>
        <span class="stat-value">{{ statistics['To Do'] || 0 }}</span>
      </div>
      <div class="stat-card">
        <span class="stat-label">In Progress</span>
        <span class="stat-value">{{ statistics['In Progress'] || 0 }}</span>
      </div>
      <div class="stat-card">
        <span class="stat-label">Done</span>
        <span class="stat-value">{{ statistics['Done'] || 0 }}</span>
      </div>
    </div>

    <div class="board-columns">
      <div class="column" v-for="status in statuses" :key="status">
        <div class="column-header">
          <h3>{{ status }}</h3>
          <span class="task-count">{{ getTasksByStatus(status).length }}</span>
        </div>
        
        <draggable
          class="task-list"
          :list="getTasksByStatus(status)"
          group="tasks"
          @change="onDragEnd($event, status)"
          :item-key="'taskId'"
        >
          <template #item="{ element }">
            <div class="task-card">
              <div class="task-header">
                <h4>{{ element.title }}</h4>
                <div class="task-actions">
                  <button @click="editTask(element)" class="btn-icon">✏️</button>
                  <button @click="deleteTaskHandler(element.taskId)" class="btn-icon">🗑️</button>
                </div>
              </div>
              <p class="task-description">{{ element.description }}</p>
              <div class="task-meta">
                <span class="task-id">#{{ element.taskId }}</span>
                <span class="task-date">{{ formatDate(element.createdAt) }}</span>
              </div>
              <div v-if="element.assignedTo" class="task-assignee">
                Assigned to: User {{ element.assignedTo }}
              </div>
            </div>
          </template>
        </draggable>
      </div>
    </div>

    <div v-if="showCreateModal" class="modal-overlay" @click.self="closeCreateModal">
      <div class="modal">
        <div class="modal-header">
          <h2>{{ editingTask ? 'Edit Task' : 'Create New Task' }}</h2>
          <button @click="closeCreateModal" class="close-btn">×</button>
        </div>
        <form @submit.prevent="saveTask" class="task-form">
          <div class="form-group">
            <label for="title">Title *</label>
            <input
              id="title"
              v-model="formData.title"
              type="text"
              required
              maxlength="200"
              placeholder="Enter task title"
            />
          </div>
          
          <div class="form-group">
            <label for="description">Description</label>
            <textarea
              id="description"
              v-model="formData.description"
              maxlength="1000"
              rows="4"
              placeholder="Enter task description"
            ></textarea>
          </div>
          
          <div class="form-group">
            <label for="status">Status *</label>
            <select id="status" v-model="formData.status" required>
              <option v-for="status in statuses" :key="status" :value="status">
                {{ status }}
              </option>
            </select>
          </div>
          
          <div class="form-group">
            <label for="assignedTo">Assigned To</label>
            <input
              id="assignedTo"
              v-model.number="formData.assignedTo"
              type="number"
              min="1"
              placeholder="User ID (optional)"
            />
          </div>
          
          <div class="form-actions">
            <button type="button" @click="closeCreateModal" class="btn btn-cancel">
              Cancel
            </button>
            <button type="submit" class="btn btn-primary">
              {{ editingTask ? 'Update' : 'Create' }}
            </button>
          </div>
        </form>
      </div>
    </div>

    <div v-if="loading" class="loading-overlay">
      <div class="spinner"></div>
    </div>
  </div>
</template>

<script>
import { ref, computed, onMounted } from 'vue';
import draggable from 'vuedraggable';
import { tasksApi } from '../api/api';

export default {
  name: 'TaskBoard',
  components: {
    draggable,
  },
  setup() {
    const tasks = ref([]);
    const statuses = ['To Do', 'In Progress', 'Done'];
    const statistics = ref(null);
    const loading = ref(false);
    const showCreateModal = ref(false);
    const editingTask = ref(null);
    const formData = ref({
      title: '',
      description: '',
      status: 'To Do',
      createdBy: 1,
      assignedTo: null,
    });

    const getTasksByStatus = (status) => {
      return tasks.value.filter(task => task.status === status);
    };

    const loadTasks = async () => {
      loading.value = true;
      try {
        const response = await tasksApi.getAllTasks();
        tasks.value = response.data;
        await loadStatistics();
      } catch (error) {
        console.error('Failed to load tasks:', error);
        alert('Failed to load tasks. Please try again.');
      } finally {
        loading.value = false;
      }
    };

    const loadStatistics = async () => {
      try {
        const response = await tasksApi.getTaskStatistics();
        statistics.value = response.data;
      } catch (error) {
        console.error('Failed to load statistics:', error);
      }
    };

    const onDragEnd = async (evt, newStatus) => {
      if (evt.added) {
        const task = evt.added.element;
        try {
          await tasksApi.updateTaskStatus(task.taskId, newStatus);
          task.status = newStatus;
          await loadStatistics();
        } catch (error) {
          console.error('Failed to update task status:', error);
          alert('Failed to update task status. Please try again.');
          await loadTasks();
        }
      }
    };

    const editTask = (task) => {
      editingTask.value = task;
      formData.value = {
        title: task.title,
        description: task.description,
        status: task.status,
        assignedTo: task.assignedTo,
      };
      showCreateModal.value = true;
    };

    const saveTask = async () => {
      loading.value = true;
      try {
        if (editingTask.value) {
          await tasksApi.updateTask(editingTask.value.taskId, formData.value);
        } else {
          await tasksApi.createTask(formData.value);
        }
        await loadTasks();
        closeCreateModal();
      } catch (error) {
        console.error('Failed to save task:', error);
        alert('Failed to save task. Please check your input and try again.');
      } finally {
        loading.value = false;
      }
    };

    const deleteTaskHandler = async (taskId) => {
      if (confirm('Are you sure you want to delete this task?')) {
        loading.value = true;
        try {
          await tasksApi.deleteTask(taskId);
          await loadTasks();
        } catch (error) {
          console.error('Failed to delete task:', error);
          alert('Failed to delete task. Please try again.');
        } finally {
          loading.value = false;
        }
      }
    };

    const closeCreateModal = () => {
      showCreateModal.value = false;
      editingTask.value = null;
      formData.value = {
        title: '',
        description: '',
        status: 'To Do',
        createdBy: 1,
        assignedTo: null,
      };
    };

    const formatDate = (dateString) => {
      const date = new Date(dateString);
      return date.toLocaleDateString('en-US', {
        month: 'short',
        day: 'numeric',
        year: 'numeric',
      });
    };

    onMounted(() => {
      loadTasks();
    });

    return {
      tasks,
      statuses,
      statistics,
      loading,
      showCreateModal,
      editingTask,
      formData,
      getTasksByStatus,
      loadTasks,
      onDragEnd,
      editTask,
      saveTask,
      deleteTaskHandler,
      closeCreateModal,
      formatDate,
    };
  },
};
</script>

<style scoped>
.task-board {
  padding: 20px;
  max-width: 1400px;
  margin: 0 auto;
  font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, 'Helvetica Neue', Arial, sans-serif;
}

.board-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 30px;
  padding-bottom: 20px;
  border-bottom: 2px solid #e0e0e0;
}

.board-header h1 {
  font-size: 28px;
  color: #2c3e50;
  margin: 0;
}

.header-actions {
  display: flex;
  gap: 10px;
}

.statistics {
  display: flex;
  gap: 20px;
  margin-bottom: 30px;
}

.stat-card {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  padding: 20px;
  border-radius: 10px;
  flex: 1;
  text-align: center;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
}

.stat-label {
  display: block;
  font-size: 14px;
  opacity: 0.9;
  margin-bottom: 5px;
}

.stat-value {
  display: block;
  font-size: 32px;
  font-weight: bold;
}

.board-columns {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 20px;
}

.column {
  background: #f8f9fa;
  border-radius: 10px;
  padding: 15px;
  min-height: 400px;
}

.column-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 15px;
  padding-bottom: 10px;
  border-bottom: 2px solid #dee2e6;
}

.column-header h3 {
  margin: 0;
  font-size: 18px;
  color: #495057;
}

.task-count {
  background: #6c757d;
  color: white;
  padding: 2px 8px;
  border-radius: 12px;
  font-size: 12px;
  font-weight: bold;
}

.task-list {
  min-height: 350px;
}

.task-card {
  background: white;
  border-radius: 8px;
  padding: 15px;
  margin-bottom: 10px;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
  cursor: move;
  transition: transform 0.2s, box-shadow 0.2s;
}

.task-card:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.15);
}

.task-header {
  display: flex;
  justify-content: space-between;
  align-items: start;
  margin-bottom: 10px;
}

.task-header h4 {
  margin: 0;
  font-size: 16px;
  color: #2c3e50;
  flex: 1;
}

.task-actions {
  display: flex;
  gap: 5px;
}

.btn-icon {
  background: none;
  border: none;
  cursor: pointer;
  font-size: 16px;
  padding: 2px;
  transition: transform 0.2s;
}

.btn-icon:hover {
  transform: scale(1.2);
}

.task-description {
  color: #6c757d;
  font-size: 14px;
  margin: 10px 0;
  line-height: 1.4;
}

.task-meta {
  display: flex;
  justify-content: space-between;
  font-size: 12px;
  color: #adb5bd;
  margin-top: 10px;
}

.task-id {
  font-weight: bold;
}

.task-assignee {
  margin-top: 10px;
  padding-top: 10px;
  border-top: 1px solid #e9ecef;
  font-size: 12px;
  color: #6c757d;
}

.btn {
  padding: 10px 20px;
  border: none;
  border-radius: 5px;
  font-size: 14px;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.3s;
}

.btn-primary {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
}

.btn-primary:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 8px rgba(102, 126, 234, 0.4);
}

.btn-secondary {
  background: #6c757d;
  color: white;
}

.btn-secondary:hover {
  background: #5a6268;
}

.btn-cancel {
  background: #e9ecef;
  color: #495057;
}

.btn-cancel:hover {
  background: #dee2e6;
}

.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0, 0, 0, 0.5);
  display: flex;
  justify-content: center;
  align-items: center;
  z-index: 1000;
}

.modal {
  background: white;
  border-radius: 10px;
  width: 90%;
  max-width: 500px;
  max-height: 90vh;
  overflow-y: auto;
}

.modal-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 20px;
  border-bottom: 1px solid #e9ecef;
}

.modal-header h2 {
  margin: 0;
  color: #2c3e50;
}

.close-btn {
  background: none;
  border: none;
  font-size: 30px;
  cursor: pointer;
  color: #6c757d;
  line-height: 1;
}

.close-btn:hover {
  color: #495057;
}

.task-form {
  padding: 20px;
}

.form-group {
  margin-bottom: 20px;
}

.form-group label {
  display: block;
  margin-bottom: 5px;
  font-weight: 500;
  color: #495057;
}

.form-group input,
.form-group textarea,
.form-group select {
  width: 100%;
  padding: 10px;
  border: 1px solid #ced4da;
  border-radius: 5px;
  font-size: 14px;
}

.form-group input:focus,
.form-group textarea:focus,
.form-group select:focus {
  outline: none;
  border-color: #667eea;
  box-shadow: 0 0 0 3px rgba(102, 126, 234, 0.1);
}

.form-actions {
  display: flex;
  justify-content: flex-end;
  gap: 10px;
  margin-top: 30px;
}

.loading-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(255, 255, 255, 0.9);
  display: flex;
  justify-content: center;
  align-items: center;
  z-index: 2000;
}

.spinner {
  width: 50px;
  height: 50px;
  border: 5px solid #f3f3f3;
  border-top: 5px solid #667eea;
  border-radius: 50%;
  animation: spin 1s linear infinite;
}

@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}

@media (max-width: 768px) {
  .board-columns {
    grid-template-columns: 1fr;
  }
  
  .statistics {
    flex-direction: column;
  }
  
  .board-header {
    flex-direction: column;
    align-items: flex-start;
    gap: 15px;
  }
}
</style>


