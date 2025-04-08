<template>
  <div class="center-layout">
    <Login />
  </div>
</template>

<style scoped>
.center-layout {
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: 100vh;
  padding: 1rem;
  box-sizing: border-box;
}
</style>


<script>

import Login from './views/Login.vue';

export default {
  components: {
    Login
  }
};

</script>
