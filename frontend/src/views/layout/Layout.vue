<template>
  <v-app id="inspire">
    <transition name="slide-fade" mode="out-in">
      <component :is="rightNavigatorDrawer" v-model="rightDrawer"></component>
    </transition>
    <component :is="currentNavigatorDrawer" v-model="drawer"></component>

    <v-app-bar
        app
        clipped-left
        clipped-right
        outlined
        flat
        dense
    >
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
    </v-app-bar>

    <v-main>
      <transition name="slide-fade" mode="out-in">
        <router-view :key="key"/>
      </transition>
    </v-main>

    <v-footer app outlined>
      <v-row no-gutters>
        <v-col class="text-center" cols="12">
          {{ new Date().getFullYear() }} — <strong>Ossian Board</strong>
        </v-col>
      </v-row>
    </v-footer>
  </v-app>
</template>

<script>
import {mapGetters} from "vuex";
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
    rightDrawer: true,
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
    rightNavigatorDrawer() {
      return this.$route.meta.rightDrawer;
    }
  },
  methods: {
    switchTheme() {
      this.$vuetify.theme.dark = !this.$vuetify.theme.dark;
    },
  }
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
