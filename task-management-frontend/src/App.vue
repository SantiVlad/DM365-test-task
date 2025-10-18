<template>
  <div id="app">
    <TaskBoard />
  </div>
</template>

<script>
import TaskBoard from './components/TaskBoard.vue';

export default {
  name: 'App',
  components: {
    TaskBoard,
  },
};
</script>

<style>
* {
  margin: 0;
  padding: 0;
  box-sizing: border-box;
}

body {
  font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, 'Helvetica Neue', Arial, sans-serif;
  background: linear-gradient(135deg, #f5f7fa 0%, #c3cfe2 100%);
  min-height: 100vh;
}

#app {
  min-height: 100vh;
}
</style>