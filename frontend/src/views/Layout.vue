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
      <v-list dense nav>
        <v-list-item v-for="item in items" :key="item.title" link>
          <v-list-item-icon>
            <v-icon>{{ item.icon }}</v-icon>
          </v-list-item-icon>
          <v-list-item-content>
            <v-list-item-title>{{ item.title }}</v-list-item-title>
          </v-list-item-content>
        </v-list-item>
      </v-list>

      <template v-slot:append>
        <div class="pa-2">
          <v-btn block @click="logout">Logout</v-btn>
        </div>
      </template>
    </v-navigation-drawer>

    <v-app-bar app clipped-left>
      <v-app-bar-nav-icon @click.stop="drawer = !drawer" />
      <v-toolbar-title>Nautilus Config Generator</v-toolbar-title>
      <v-spacer></v-spacer>
      <v-btn icon>
        <v-icon @click="switchTheme">invert_colors</v-icon>
      </v-btn>
    </v-app-bar>

    <v-content>
      <router-view :key="key"></router-view>
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
    items: [
      {
        icon: 'mdi-view-dashboard',
        title: 'Arguments'
      },
      {
        icon: 'mdi-settings',
        title: 'Settings'
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
    }
  }
};
</script>
