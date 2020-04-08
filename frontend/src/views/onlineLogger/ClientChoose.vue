<template>
  <v-navigation-drawer v-model="drawer" app clipped width="400px">
    <v-divider />

    <v-item-group v-model="selectedLogIndex">
      <v-container>
        <v-row v-for="item in logList" :key="item.id">
          <v-col cols="12">
            <v-item v-slot:default="{ active, toggle }">
              <v-card
                :class="{
                  grey: active && $vuetify.theme.dark,
                  'darken-4': active && $vuetify.theme.dark,
                  blue: active && !$vuetify.theme.dark,
                  'lighten-4': active && !$vuetify.theme.dark,
                }"
                @click="toggle"
                outlined
                :hover="!active"
                link
                :to="`/board/${item.id}`"
              >
                <v-card-title>
                  {{ item.name }}
                  <v-spacer />
                  <v-btn icon @click="handleDeleteLog(item.id)">
                    <v-icon>delete</v-icon>
                  </v-btn>
                </v-card-title>
                <v-card-subtitle>{{ item.description }}</v-card-subtitle>
                <v-card-subtitle>{{ item.createTime }}</v-card-subtitle>
              </v-card>
            </v-item>
          </v-col>
        </v-row>
      </v-container>
    </v-item-group>
  </v-navigation-drawer>
</template>

<script>
import { mapGetters } from "vuex";
import { removeLog } from "@/api/log";
export default {
  name: "DefaultNavigator",
  props: {
    value: {
      type: Boolean,
      default: () => null,
    },
  },
  data() {
    return {
      currentView: null,
      logListData: [],
      selectedLogId: this.$route.params.logId,
    };
  },
  computed: {
    ...mapGetters("log", ["connection", "connectionStatus", "logList"]),
    drawer: {
      get() {
        return this.value;
      },
      set(val) {
        this.$emit("input", val);
      },
    },
    selectedLogIndex: {
      get() {
        return this.logList.findIndex((item) => item.id === this.selectedLogId);
      },
      set() {
      },
    }
  },
  async created() {
    await this.$store.dispatch("log/refreshLogList");
  },
  methods: {
    async logout() {
      await this.$store.dispatch("user/logout");
      this.$router.push({ path: "/login" });
    },
    switchPage(row) {
      this.$router.push({ path: row.redirect });
    },
    handleDeleteLog(logId) {
      removeLog(logId);
    },
  },
};
</script>
