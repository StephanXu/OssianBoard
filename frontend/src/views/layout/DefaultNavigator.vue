<template>
  <v-navigation-drawer app clipped :mini-variant.sync="drawer" permanent>
    <v-list>
      <v-list-item class="px-2">
        <v-list-item-avatar>
          <v-img src="https://i.loli.net/2020/09/23/3mQ6Xy2wqIEn9xW.png"></v-img>
        </v-list-item-avatar>

        <v-list-item-title></v-list-item-title>

        <v-btn icon @click.stop="drawer = !drawer">
          <v-icon>mdi-chevron-left</v-icon>
        </v-btn>
      </v-list-item>

      <v-list-item link>
        <v-list-item-content>
          <v-list-item-title class="title">{{ alias }}</v-list-item-title>
          <v-list-item-subtitle>{{ name }}</v-list-item-subtitle>
        </v-list-item-content>
      </v-list-item>
    </v-list>

    <v-divider></v-divider>
    <v-list dense nav flat>
      <v-list-item-group v-model="currentView" color="primary">
        <v-list-item
            v-for="item in items"
            :key="item.title"
            link
            :to="item.redirect"
        >
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
</template>

<script>
import {mapGetters} from "vuex";

export default {
  name: "DefaultNavigator",
  props: {
    value: {
      type: Boolean,
      default: () => null
    }
  },
  data() {
    return {
      currentView: null,
      items: [
        {
          icon: "mdi-view-dashboard",
          title: "Arguments",
          redirect: "/argument"
        },
        {
          icon: "mdi-chart-areaspline",
          title: "Online Logger",
          redirect: "/board"
        },
        {
          icon: "mdi-account",
          title: "Profile",
          redirect: "/profile"
        }
      ]
    };
  },
  computed: {
    ...mapGetters("user", ["name", "alias"]),
    drawer: {
      get() {
        return this.value;
      },
      set(val) {
        this.$emit("input", val);
      }
    }
  },
  methods: {
    async logout() {
      await this.$store.dispatch("user/logout");
      this.$router.push({path: "/login"});
    }
  }
};
</script>
