<template>
  <div class="center-layout">
    <v-card v-if="isLoggedIn && !loggedInUser.isAdmin" class="mb-4 pa-4" elevation="6">
      <v-card-title>Welcome, {{ loggedInUser.email }}</v-card-title>
      <v-card-text>
        Your birthday is {{ new Date(loggedInUser.birthday).toDateString()}}.
      </v-card-text>
      <v-card-text class="pt-0">
        Your secret cousin is {{ loggedInUser.publicFigure }}.
      </v-card-text>
      <div class="text-center">
        <v-btn color="error" @click="logout">Sign out</v-btn>
      </div>
    </v-card>

    <v-card v-else-if="isLoggedIn && loggedInUser.isAdmin" class="mb-4 pa-4" elevation="6">
      <v-card-title>Welcome, {{ loggedInUser.email }}</v-card-title>
      <v-card-text class="pt-3">
        List of all users in the database.
      </v-card-text>
      <v-data-table
        :headers="headers"
        :items="loggedInUser?.users || []"
        class="elevation-1"
        hide-default-footer
        hide-default-header
        density="comfortable" />
      <div class="text-center pt-5">
        <v-btn color="error" @click="logout">Sign out</v-btn>
      </div>
    </v-card>

    <v-card v-else elevation="10" width="500">
      <v-window v-model="currentWindow">
        <v-window-item value="login">
          <v-card-title class="pa-5">Login</v-card-title>
          <v-card-subtitle class="pb-2"></v-card-subtitle>
          <v-card-text class="pb-0">
            <form @submit.prevent="login">
              <v-text-field v-model="loginEmail" label="Email" type="email" required />
              <v-text-field 
                v-model="loginPassword" 
                label="Password" 
                :type="showPassword ? 'text' : 'password'" 
                :append-icon="showPassword ? 'mdi-eye' : 'mdi-eye-off'"
                @click:append="showPassword = !showPassword"
                required />
              <v-switch
                v-model="sanitizeInput"
                label="Protect Against SQL Injection"
                color="primary"
                class="mb-3" />
            </form>
          </v-card-text>
          <div class="text-center">
            <v-btn @click="login()" color="primary" :loading="busy" width="220">
              Login
              <v-icon right>mdi-arrow-right</v-icon>
            </v-btn>
          </div>
          <v-divider class="mt-3" />
          <div class="pt-3 pb-3 text-center">
            <v-btn text variant="text" small color="primary" @click="currentWindow = 'register'; clearAllText()">
              Register Account
              <v-icon right color="primary">mdi-chevron-right</v-icon>
            </v-btn>
          </div>
        </v-window-item>
        <v-window-item value="register">
          <v-card-title class="pa-5">Create Account</v-card-title>
          <v-card-subtitle class="pb-2"></v-card-subtitle>
          <v-card-text class="pb-0">
            <form @submit.prevent="register">
              <v-text-field v-model="registerBirthday" label="Birthday" type="date" required />
              <v-text-field v-model="registerEmail" label="Email" type="email" required />
              <v-text-field v-model="registerPassword" label="Password" type="password" required />
              <v-switch v-model="hashPassword" label="Hash Password"  color="primary" />
              <div class="text-center">
                <v-btn color="success" type="submit" width="220">Register</v-btn>
              </div>
              <v-divider class="mt-7" />
          </form>
          <div class="pt-3 pb-3 text-center">
          <v-btn variant="text" small color="primary"
              :disabled="busy"
              @click="currentWindow = 'login'; clearAllText()">
            <v-icon left>mdi-chevron-left</v-icon>
            Back to Login
          </v-btn>
        </div>
          </v-card-text>
        </v-window-item>
      </v-window>
    </v-card>
    <v-snackbar
      v-model="snackbar"
      :color="snackbarColor"
      timeout="3000"
      location="top"
      multi-line >
      {{ snackbarText }}
    </v-snackbar>
  </div>
</template>

  
<script setup>

import { ref } from 'vue'
import axios from 'axios'

const api = axios.create({
  baseURL: import.meta.env.VITE_API_URL,
  headers: {
    'Content-Type': 'application/json'
  }
});

const snackbar = ref(false)
const snackbarText = ref('')
const snackbarColor = ref('success') 


const loginEmail = ref('')
const loginPassword = ref('')
const registerEmail = ref('')
const registerPassword = ref('')
const registerBirthday = ref('')

const hashPassword = ref(true)
const sanitizeInput = ref(true)
const showPassword = ref(false)

const headers = ref([
  { text: 'Email', value: 'email' },
  { text: 'Birthday', value: 'birthday' },
  { text: 'Public Figure', value: 'publicFigure' },
  { text: 'Password', value: 'password' },
])

function showMessage(message, color = 'success') {
  snackbarText.value = message
  snackbarColor.value = color
  snackbar.value = true
}

function clearAllText(){
  loginEmail.value = ''
  loginPassword.value = ''
  registerEmail.value = ''
  registerPassword.value = ''
  registerBirthday.value = ''
}

const isLoggedIn = ref(false)
const loggedInUser = ref(null)

const currentWindow = ref('login')
const busy = ref(false)
  
async function login() {
  busy.value = true
  try {
    const res = await api.post('/api/users/login', {
      email: loginEmail.value,
      password: loginPassword.value,
      sanitize: sanitizeInput.value
    }, {
      headers: {
        'Content-Type': 'application/json'
      }
  });

  if (res.data.isAdmin) {
    res.data.users.forEach(user => {
      user.birthday = new Date(user.birthday).toDateString()
    })
  }

  loggedInUser.value = res.data
  isLoggedIn.value = true

  } catch (err) {
    showMessage('Login failed: ' + (err.response?.data || err.message), 'error')
  } finally {
    busy.value = false
  }
}

function logout() {
  isLoggedIn.value = false
  loggedInUser.value = null
  loginEmail.value = ''
  loginPassword.value = ''
}
  
async function register() {
    try {
      await api.post('/api/users/register?hashPassword=' + hashPassword.value, {
        email: registerEmail.value,
        password: registerPassword.value,
        birthday: registerBirthday.value,
      }, {
        headers: {
          'Content-Type': 'application/json'
        }
      });

    showMessage('Registration successful!')
    clearAllText()
    } catch (err) {
      showMessage('Registration failed: ' + (err.response?.data || err.message), 'error')
    }
  }
</script>
  
