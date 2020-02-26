<template>
  <v-container fluid>
    <v-row align="center" justify="center">
      <v-col cols="12">
        <v-expansion-panels multiple v-model="panelStatus">
          <v-expansion-panel
            v-for="(item, index) in logList"
            :key="index"
            @change="handlePanelChange(index)"
          >
            <v-expansion-panel-header>
              <v-row no-gutters>
                <v-col cols="4">{{ item.name }}</v-col>
                <v-col cols="4" class="text--secondary">
                  <span>
                    {{ item.description }}
                  </span>
                </v-col>
                <v-col cols="4" class="text--secondary">
                  <span>
                    {{ item.createTime }}
                  </span>
                </v-col>
              </v-row>
            </v-expansion-panel-header>
            <v-expansion-panel-content style="margin: 0px;">
              <v-data-table
                fixed-header
                dense
                calculate-widths
                hide-default-footer
                height="300px"
                :headers="logTableHeader"
                :items="item.log"
                item-key="time"
                disable-pagination
                must-sort
              >
                <template v-slot:item.level="{ item }">
                  <v-chip x-small :color="getLogColor(item.level)" dark>
                    {{ item.level }}</v-chip
                  >
                </template>
              </v-data-table>
            </v-expansion-panel-content>
          </v-expansion-panel>
        </v-expansion-panels>

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

    <v-btn :color="connected ? 'pink' : 'green'" dark fixed bottom right fab>
      <v-icon v-if="connected" @click="disconnect">mdi-logout</v-icon>
      <v-icon v-else @click="connect">mdi-login</v-icon>
    </v-btn>
  </v-container>
</template>

<script>
import { HubConnectionBuilder, LogLevel } from "@microsoft/signalr";
import Plot from "@/components/Plot";
import { getToken } from "@/utils/auth";

export default {
  components: {
    Plot
  },
  data() {
    return {
      fab: false,
      connection: null,
      onlineVariants: [],
      logList: [],
      panelStatus: [],
      logTableHeader: [
        {
          text: "Time",
          value: "time",
          sort: (lhs, rhs) => Date.parse(rhs) - Date.parse(lhs),
          width: "15%"
        },
        { text: "Thread Id", value: "threadId", width: "10%" },
        { text: "Level", value: "level", width: "10%" },
        { text: "Content", value: "content" }
      ]
    };
  },
  computed: {
    connected() {
      try {
        return this.connection.connectionState === "Connected";
      } catch (e) {
        return false;
      }
    }
  },
  async created() {
    if (!this.connection || this.connection.connectionState != "Connected") {
      this.connection = new HubConnectionBuilder()
        .withUrl("/log-viewer", { accessTokenFactory: () => getToken() })
        .configureLogging(LogLevel.Information)
        .build();
      this.connection.on("RefreshLogsList", logList => {
        this.logList = logList;
      });
      this.connection.on("ReceiveLog", (logId, logs) => {
        console.log(logId)
        let logItem = this.logList.find(item => item.id === logId);
        if (logItem) {
          logItem.logs.push(...logs);
        }
      });
      await this.connection.start();
    }
    this.logList = (await this.connection.invoke("ListLogs")).map(item => {
      return {
        ...item,
        log: [],
        loading: false
      };
    });
    console.log(this.logList);
  },
  methods: {
    handlePanelChange(index) {
      if (this.panelStatus.find(item => item == index) == undefined) {
        console.log("open");
        // panel open
        this.logList[index].loading = true;
        this.connection.invoke("ListenLog", this.logList[index].id);
        this.connection.invoke("GetLog", this.logList[index].id).then(res => {
          this.logList[index].log.splice(0, 0, ...res.log);
          console.log(this.logList[index].log);
          this.logList[index].loading = false;
        });
      } else {
        console.log("close");
        // panel close
        this.connection.invoke("UnListenLog", this.logList[index].id);
        this.logList[index].log = [];
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
