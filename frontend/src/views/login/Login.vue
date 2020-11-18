<template>
  <v-app id="inspire">
    <span class="bg"></span>
    <v-content>
      <v-container class="fill-height d-flex justify-center">
        <v-card class="elevation-3 flex-grow-1" max-width="600px" :loading="loading">
          <v-card-title class="mt-5">
            <div class="flex">
              <h3 class="text-center">Welcome back</h3>
              <p class="text-center subtitle-1">Login and continue</p>
            </div>
          </v-card-title>
          <v-card-text>
            <v-form class="ml-5 mr-5">
              <v-text-field
                  label="Username"
                  v-model="loginForm.username"
                  name="login"
                  prepend-icon="person"
                  type="text"
                  outlined
              />
              <v-text-field
                  id="password"
                  v-model="loginForm.password"
                  label="Password"
                  name="password"
                  prepend-icon="lock"
                  type="password"
                  outlined
              />
            </v-form>
            <v-alert v-if="loginAlert.visible" outlined color="red" type="error" dense class="ml-5 mr-5">
              {{ loginAlert.text }}
            </v-alert>
          </v-card-text>
          <v-card-actions class="mb-4 mr-7">
            <v-spacer/>
            <v-btn color="primary" @click="onLogin" large>Login</v-btn>
          </v-card-actions>
        </v-card>
      </v-container>
    </v-content>
  </v-app>
</template>

<script>
export default {
  data() {
    return {
      connection: null,
      loading: false,
      loginAlert: {
        visible: false,
        text: ''
      },
      loginForm: {
        username: '',
        password: ''
      }
    }
  },
  methods: {
    onLogin() {
      this.loading = true
      this.$store.dispatch('user/login', this.loginForm)
          .then(() => {
            this.loading = false
            this.$router.push({path: '/'})
          })
          .catch(() => {
            this.loginAlert.text = 'Login failed'
            this.loginAlert.visible = true
            this.loading = false
          })
    }
  },
  created() {
  }
}
</script>
<style>
.bg {
  width: 100%;
  height: 100%;
  position: absolute;
  top: 0px;
  left: 0px;
  background: url('https://i.picsum.photos/id/1021/1920/1080.jpg?hmac=DS2TfmvLwtjDGFoSGOPJw6PprUlsIUz-0huiqVL3N_Y') no-repeat center center;
  background-size: cover;
}
</style>