<template>
  <v-app id="inspire">
    <v-navigation-drawer v-model="drawer" app clipped>
      <v-list nav>
        <v-list-item link>
          <v-list-item-content>
            <v-list-item-title class="title">
              {{alias}}
            </v-list-item-title>
            <v-list-item-subtitle>
              {{name}}
            </v-list-item-subtitle>
          </v-list-item-content>
        </v-list-item>
      </v-list>

      <v-divider></v-divider>
      <v-list dense nav flat>
        <v-list-item-group v-model="currentView" color="primary">
          <v-list-item v-for="item in items" :key="item.title" link @click="switchPage(item)">
            <v-list-item-icon>
              <v-icon>{{ item.icon }}</v-icon>
            </v-list-item-icon>
            <v-list-item-content>
              <v-list-item-title>{{ item.title }}</v-list-item-title>
            </v-list-item-content>
          </v-list-item>
        </v-list-item-group>
      </v-list>

      <template v-slot:append>
        <div class="pa-2">
          <v-btn block @click="logout">Logout</v-btn>
        </div>
      </template>
    </v-navigation-drawer>

    <v-app-bar app clipped-left color="blue darken-3" dark>
      <v-app-bar-nav-icon @click.stop="drawer = !drawer" />
      <v-toolbar-title style="width: 300px" class="ml-0 pl-3">
        <span class="title">Ossian Logger</span>
      </v-toolbar-title>
      <v-spacer></v-spacer>
      <v-btn icon>
        <v-icon @click="switchTheme">invert_colors</v-icon>
      </v-btn>
    </v-app-bar>

    <v-content>
      <transition name="scroll-x-reverse-transition" mode="out-in">
        <router-view :key="key" />
      </transition>
    </v-content>

    <v-footer app>
      <span>&copy; 2020</span>
    </v-footer>
  </v-app>
</template>

<script>
import {mapGetters} from 'vuex'

export default {
  props: {
    source: String
  },
  data: () => ({
    drawer: null,
    currentView: null,
    items: [
      {
        icon: 'mdi-view-dashboard',
        title: 'Arguments',
        redirect: '/index'
      },
      {
        icon: 'mdi-chart-areaspline',
        title: 'Online Logger',
        redirect: '/board'
      },
      {
        icon: 'mdi-account',
        title: 'Profile',
        redirect: '/profile'
      }
    ]
  }),
  computed: {
    ...mapGetters(['name', 'alias']),
    key() {
      return this.$route.path
    }
  },
  methods: {
    switchTheme() {
      this.$vuetify.theme.dark = !this.$vuetify.theme.dark;
    },
    async logout() {
      await this.$store.dispatch('logout')
      this.$router.push({path: '/login'})
    },
    switchPage(row) {
      this.$router.push({path: row.redirect})
    }
  }
};
</script>
