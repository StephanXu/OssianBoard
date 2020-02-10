<template>
  <v-app id="inspire">
    <v-content>
      <v-container
        class="fill-height"

      >
        <v-row
          align="center"
          justify="center"
        >
          <v-col
            cols="12"
            sm="8"
            md="4"
          >
            <v-card class="elevation-12">
              <v-toolbar
                color="primary"
                dark
                flat
              >
                <v-toolbar-title>认证</v-toolbar-title>
                <v-spacer />
              </v-toolbar>
              <v-card-text>
                <v-form>
                  <v-text-field
                    label="账户名"
                    v-model="loginForm.username"
                    name="login"
                    prepend-icon="person"
                    type="text"
                  />

                  <v-text-field
                    id="password"
                    v-model="loginForm.password"
                    label="密码"
                    name="password"
                    prepend-icon="lock"
                    type="password"
                  />
                </v-form>
              </v-card-text>
              <v-card-actions>
                <v-spacer />
                <v-btn color="primary" @click="onLogin">登陆</v-btn>
              </v-card-actions>
            </v-card>
          </v-col>
        </v-row>
      </v-container>
    </v-content>
  </v-app>
</template>

<script>
export default {
  data() {
    return {
      connection: null,
      loginForm: {
        username: '',
        password: ''
      }
    }
  },
  methods: {
    onLogin() {
      this.$store.dispatch('login', this.loginForm)
        .then(() => {
          console.log('loggin success')
          this.$router.push({path: '/'})
        })
    }
  },
  created() {
  }
}
</script>
