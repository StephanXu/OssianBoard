<template>
  <v-container fluid>
    <v-row align="center" justify="center">
      <v-col cols="12">
        <v-card v-if="!isEmptyId">
          <v-data-table
            fixed-header
            dense
            calculate-widths
            :headers="logTableHeader"
            :items="logs"
            item-key="time"
          >
            <template v-slot:top>
              <v-toolbar flat>
                <v-toolbar-title>Raw log</v-toolbar-title>
                <v-spacer></v-spacer>
                <v-switch
                  v-model="prettier"
                  label="Prettier"
                  class="mt-2"
                  disabled
                ></v-switch>
              </v-toolbar>
            </template>
            <template v-slot:item.level="{ item }">
              <v-chip x-small :color="getLogColor(item.level)" dark>
                {{ item.level }}</v-chip
              >
            </template>
          </v-data-table>
        </v-card>
        <v-card
          v-for="item in onlineVariants"
          :key="item.varName"
          class="mx-auto"
          style="margin-bottom: 20px;max-width: 850px"
          color="grey lighten-4"
          tle
        >
          <v-card-title>
            <v-icon color="indigo" class="mr-12" size="64">
              mdi-record-circle
            </v-icon>
            <v-row align="start">
              <div class="caption grey--text text-uppercase">
                {{ item.varName }}
              </div>
              <div>
                <span
                  class="display-2 font-weight-black black--text"
                  v-text="item.dots[item.dots.length - 1] || '—'"
                ></span>
                <strong class="black--text">Currently</strong>
              </div>
            </v-row>
          </v-card-title>
          <!--          <v-card-title>{{item.varName}}</v-card-title>-->
          <plot
            style="height: 500px;width: 100%"
            :dots="item.dots"
            :times="item.times"
          ></plot>
        </v-card>
      </v-col>
    </v-row>
  </v-container>
</template>

<script>
import Plot from "@/components/Plot";
import { mapGetters } from "vuex";
export default {
  components: {
    Plot
  },
  props: {
    logId: {
      type: String,
      default: () => null
    }
  },
  data() {
    return {
      fab: false,
      onlineVariants: [],
      prettier: true,
      logs: [],
      logTableHeader: [
        {
          text: "Time",
          value: "time",
          width: "20%"
        },
        { text: "Thread", value: "threadId", width: "10%" },
        { text: "Level", value: "level", width: "10%" },
        { text: "Content", value: "content" }
      ],
      loading: false
    };
  },
  computed: {
    ...mapGetters("log", ["connection", "connectionStatus"]),
    connected() {
      try {
        return this.connection.connectionState === "Connected";
      } catch (e) {
        return false;
      }
    },
    isEmptyId() {
      return this.logId == "index" || this.logId == "";
    }
  },
  async created() {
    if (this.isEmptyId) {
      return;
    }
    if (!this.connectionStatus) {
      await this.$store.dispatch("log/connect");
    }
    this.connection.on("ReceiveLog", (logId, logs) => {
      if (this.logId === logId) {
        logs = logs.sort(
          (lhs, rhs) => Date.parse(rhs.time) - Date.parse(lhs.time)
        );
        this.logs.splice(0, 0, ...logs);
      }
    });
    this.loading = true;
    this.connection.invoke("ListenLog", this.logId);
    this.connection.invoke("GetLog", this.logId).then(res => {
      res.log = res.log.sort(
        (lhs, rhs) => Date.parse(rhs.time) - Date.parse(lhs.time)
      );
      this.logs.push(...res.log);
      this.loading = false;
    });
  },
  async destroyed() {
    if (this.connectionStatus) {
      await this.connection.invoke("UnListenLog", this.logId);
      this.logs = [];
    }
  },
  methods: {
    handlePanelChange(index) {
      if (this.panelStatus.find(item => item == index) == undefined) {
        // panel open
      }
    },
    getLogColor(level) {
      if (level === "info") return "green";
      if (level === "debug") return "primary";
      else if (level === "trace") return "";
      else if (level === "error") return "indigo";
      else if (level === "warning") return "orange";
      else if (level === "critical") return "red";
      return "";
    },
    async connect() {
      this.connection.on("ReceiveLog", (clientId, varName, value, times) => {
        let varObj = this.onlineVariants.find(item => item.varName === varName);
        if (!varObj) {
          this.onlineVariants.push({
            varName: varName,
            dots: value,
            times: times
          });
          return;
        }
        varObj.dots = varObj.dots.concat(value);
        varObj.times = varObj.times.concat(times);
        if (varObj.dots.length > 100) {
          varObj.dots.splice(0, varObj.dots.length - 100);
          varObj.times.splice(0, varObj.times.length - 100);
        }
      });
    },
    async disconnect() {
      await this.connection.stop();
    }
  }
};
</script>
