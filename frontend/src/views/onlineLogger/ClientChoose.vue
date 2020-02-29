<template>
  <v-navigation-drawer v-model="drawer" app clipped width="400px">
    <v-divider />

    <v-item-group>
      <v-container>
        <v-row v-for="item in logList" :key="item.id">
          <v-col cols="12">
            <v-item v-slot:default="{ active, toggle }">
              <v-card
                :color="active ? '#e9f1fe' : ''"
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
      logListData: []
    };
  },
  computed: {
    ...mapGetters("log", ["connection", "connectionStatus"]),
    drawer: {
      get() {
        return this.value;
      },
      set(val) {
        this.$emit("input", val);
      }
    },
    logList: {
      get() {
        return this.logListData;
      },
      set(val) {
        this.logListData = val
          .sort(
            (lhs, rhs) =>
              Date.parse(rhs.createTime) - Date.parse(lhs.createTime)
          )
          .map(item => {
            return {
              ...item,
              createTime: new Date(item.createTime).toLocaleString()
            };
          });
      }
    }
  },
  async created() {
    if (!this.connectionStatus) {
      await this.$store.dispatch("log/connect");
      this.connection.on("RefreshLogsList", logList => {
        this.logList = logList;
      });
    }
    this.logList = await this.connection.invoke("ListLogs");
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
      this.connection.invoke('RemoveLog',logId)
    }
  }
};
</script>
