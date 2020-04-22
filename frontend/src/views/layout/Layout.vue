<template>
  <v-app id="inspire">
    <component :is="currentNavigatorDrawer" v-model="drawer" />
    <v-app-bar
      app
      clipped-left
      color="blue darken-3"
      dark
      src="https://picsum.photos/1920/1080?random"
    >
      <template v-slot:img="{ props }">
        <v-img
          v-bind="props"
          gradient="to top right, rgba(100,115,201,.7), rgba(25,32,72,.7)"
        ></v-img>
      </template>

      <v-app-bar-nav-icon
        v-if="currentNavigatorDrawer"
        @click.stop="drawer = !drawer"
      />

      <v-toolbar-title style="width: 300px" class="ml-0 pl-3">
        <span class="title">Ossian Board</span>
      </v-toolbar-title>

      <v-spacer></v-spacer>

      <v-btn icon>
        <v-icon @click="switchTheme">invert_colors</v-icon>
      </v-btn>

      <template v-slot:extension>
        <v-tabs align-with-title dark background-color="transparent">
          <v-tab
            v-for="item in items"
            :key="item.title"
            link
            :to="item.redirect"
            >{{ item.title }}</v-tab
          >
        </v-tabs>
      </template>
    </v-app-bar>

    <v-content>
      <transition name="slide-fade" mode="out-in">
        <router-view :key="key" />
      </transition>
    </v-content>

    <v-footer app>
      <span>&copy; 2020</span>
    </v-footer>
  </v-app>
</template>

<script>
import { mapGetters } from "vuex";
import DefaultNavigator from "./DefaultNavigator";
export default {
  components: {
    DefaultNavigator,
  },
  props: {
    source: String,
  },
  data: () => ({
    drawer: null,
    items: [
      {
        icon: "mdi-view-dashboard",
        title: "Arguments",
        redirect: "/index",
      },
      {
        icon: "mdi-chart-areaspline",
        title: "Online Logger",
        redirect: "/board",
      },
      {
        icon: "mdi-account",
        title: "Profile",
        redirect: "/profile",
      },
    ],
  }),
  computed: {
    ...mapGetters("user", ["name", "alias"]),
    key() {
      return this.$route.path;
    },
    currentNavigatorDrawer() {
      return this.$route.meta.drawer;
    },
  },
  methods: {
    switchTheme() {
      this.$vuetify.theme.dark = !this.$vuetify.theme.dark;
    },
  },
};
</script>

<style scoped>
.slide-fade-enter-active {
  transition: all 0.3s ease;
}
.slide-fade-leave-active {
  transition: all 0.1s ease;
}
.slide-fade-enter {
  transform: translateX(10px);
  opacity: 0;
}
.slide-fade-leave-to {
  transform: translateX(-10px);
  opacity: 0;
}
</style>
