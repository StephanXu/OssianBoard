<template>
  <div :class="scrollbarTheme" :style="rowStyle">
    <v-container fluid v-if="isEmptyId" class="d-flex">
    </v-container>

    <v-container v-if="!isEmptyId" fluid class="d-flex justify-space-between flex-wrap">
      <div>
        <h1>{{ logMeta.name }}</h1>
        <p class="text-subtitle-1">{{ logMeta.description }}</p>
        <v-subheader class="ml-n4">{{ presentCreateTime }}</v-subheader>
      </div>
      <div class="d-flex flex-column">
        <v-btn small depressed color="error" class="mt-2" @click="handleDelete"
               :loading="isDeleting">
          <v-icon left>mdi-delete</v-icon>
          Delete
        </v-btn>
      </div>
    </v-container>

    <v-container fluid>
      <v-row align="center" justify="center">
        <v-col cols="12">
          <v-card v-if="!isEmptyId">
            <v-row no-gutters>
              <v-col cols="12">
                <v-card flat>
                  <v-tabs v-model="activePlot" background-color="transparent">
                    <v-tab v-for="item in figures" :key="item.name">
                      {{ item.name }}
                    </v-tab>
                  </v-tabs>
                  <plot
                      style="height: 550px; width: 100%; padding-bottom:50px"
                      :values="figures[activePlot].variables"
                      @click="handlePlotClick"
                  ></plot>
                </v-card>
              </v-col>
            </v-row>
            <v-divider/>
            <v-row no-gutters>
              <v-col cols="12">
                <v-card flat>
                  <v-data-table
                      fixed-header
                      dense
                      calculate-widths
                      :headers="logTableHeader"
                      :items="logs"
                      item-key="id"
                      :server-items-length="logMeta.recordCount"
                      :items-per-page="itemsPerPage"
                      :page="currentPage"
                      @pagination="handlePagination"
                      :loading="loading"
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
                        {{ item.level }}
                      </v-chip>
                    </template>
                  </v-data-table>
                </v-card>
              </v-col>
            </v-row>
            <v-divider/>
            <v-row>
              <v-col cols="12">
                <v-card flat>
                  <v-card-title>Configuration</v-card-title>
                  <v-treeview
                      dense
                      transition
                      :items="configTreeView"
                      class="overflow-y-auto"
                      style="height:250px"
                  ></v-treeview>
                </v-card>
              </v-col>
            </v-row>
          </v-card>
        </v-col>
      </v-row>
    </v-container>
  </div>
</template>

<script>
import Plot from "./components/Plot";
import {mapGetters} from "vuex";
import {getLog, getLogByTime, getArchivedConfiguration, removeLog} from "@/api/log";

export default {
  components: {
    Plot,
  },
  props: {
    logId: {
      type: String,
      default: () => null,
    },
  },
  data() {
    return {
      fab: false,
      onlineVariants: [],
      figures: [],
      prettier: true,
      logs: [],
      logTableHeader: [
        {
          text: "Time",
          value: "time",
          width: "20%",
        },
        {text: "Thread", value: "threadId", width: "10%"},
        {text: "Level", value: "level", width: "10%"},
        {text: "Content", value: "content"},
      ],
      loading: false,
      plotLoading: false,
      itemsPerPage: 10,
      currentPage: 1,
      logMeta: {
        id: "",
        createTime: "",
        description: "",
        name: "",
        recordCount: 0,
      },
      activePlot: 0,
      configurationData: {},
      isDeleting: false
    };
  },
  computed: {
    ...mapGetters("log", ["connection", "connectionStatus", "logList"]),
    ...mapGetters("view", ["clientHeight"]),
    connected() {
      try {
        return this.connection.connectionState === "Connected";
      } catch (e) {
        return false;
      }
    },
    isEmptyId() {
      return this.logId == "index" || this.logId == "";
    },
    configTreeView() {
      return this.configTree(this.configurationData, 0);
    },
    haveConfigTreeView() {
      return this.configurationData != {};
    },
    scrollbarTheme() {
      return this.$vuetify.theme.dark ? 'dark' : 'light'
    },
    rowStyle() {
      return {
        'overflow-y': 'auto',
        'height': `${this.clientHeight - this.$vuetify.application.top - this.$vuetify.application.footer}px`
      }
    },
    presentCreateTime() {
      return `Created at ${this.logMeta.createTime}`
    }
  },
  async created() {
    this.$nextTick(async () => {
      if (this.isEmptyId) {
        return;
      }
      if (!this.connectionStatus) {
        await this.$store.dispatch("log/connect");
      }

      let logMetaCache = this.logList.find((item) => item.id === this.logId);
      this.logMeta =
          logMetaCache == null
              ? await this.connection.invoke("GetLogMeta", this.logId)
              : logMetaCache;
      if (this.logMeta == null) {
        this.$router.push({path: "/board/index"});
      }

      this.connection.on("ReceiveLog", (increment) => {
        this.mergePlot(increment.plots);
        this.logMeta.recordCount += increment.records.length;
        if (this.currentPage == 1) {
          if (increment.records.length >= this.itemsPerPage) {
            this.logs = this.processLogs(
                increment.records.slice(0, this.itemsPerPage)
            );
          } else {
            this.logs = this.processLogs(
                this.logs
                    .slice(0, this.itemsPerPage - increment.records.length)
                    .concat(increment.records)
            );
          }
        }
      });
      this.loading = true;
      this.plotLoading = true;
      this.connection.invoke("ListenLog", this.logId);
      this.connection.invoke("GetPlots", this.logId).then((res) => {
        this.mergePlot(res);
        this.plotLoading = false;
        this.loading = false;
      });
      this.configurationData = await getArchivedConfiguration(this.logId);
    });
  },
  async destroyed() {
    if (this.connectionStatus) {
      await this.connection.invoke("UnListenLog", this.logId);
      this.logs = [];
    }
  },
  methods: {
    mergePlot(plots) {
      plots.forEach((element) => {
        let plotIndex = this.figures.findIndex(
            (item) => item.name === element.name
        );
        if (plotIndex == -1) {
          this.figures.push(element);
          return;
        }
        element.variables.forEach((variable) => {
          let variableIndex = this.figures[plotIndex].variables.findIndex(
              (item) => item.name === variable.name
          );
          if (variableIndex == -1) {
            variable.dots.sort((lhs, rhs) => lhs.time - rhs.time);
            this.figures[plotIndex].variables.push(variable);
            return;
          }
          let dots = this.figures[plotIndex].variables[variableIndex].dots;
          this.figures[plotIndex].variables[variableIndex].dots = dots.concat(
              variable.dots
          );
          this.figures[plotIndex].variables[variableIndex].dots.sort(
              (lhs, rhs) => lhs.time - rhs.time
          );
        });
      });
    },
    parseLog(log) {
      try {
        let figureName = log.content.match(/@(\w+)/)[1];
        let figureIndex = this.figures.findIndex(
            (item) => item.name === figureName
        );
        if (figureIndex == -1) {
          this.figures.push({
            name: figureName,
            variables: [],
          });
          figureIndex = this.figures.length - 1;
        }
        const regexAssignPattern = /\$(?<varName>\w+)\s*=\s*(?<value>[-0-9e+.]*)/g;
        let assignMatch = null;
        while (
            (assignMatch = regexAssignPattern.exec(
                log.content.match(/\[(.*?)\]/)[1]
            ))
            ) {
          let varIndex = this.figures[figureIndex].variables.findIndex(
              (item) => item.varName === assignMatch.groups.varName
          );
          if (varIndex == -1) {
            this.figures[figureIndex].variables.push({
              varName: assignMatch.groups.varName,
              times: [new Date(log.time)],
              values: [Number(assignMatch.groups.value)],
            });
          } else {
            this.figures[figureIndex].variables[varIndex].times.push(
                new Date(log.time)
            );
            this.figures[figureIndex].variables[varIndex].values.push(
                Number(assignMatch.groups.value)
            );
          }
        }
      } catch (err) {
        return;
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
    async handlePagination({page, itemsPerPage}) {
      if (
          this.currentPage == page &&
          itemsPerPage == this.itemsPerPage &&
          this.logs.length != 0
      ) {
        return;
      }
      this.loading = true;
      this.currentPage = page;
      this.itemsPerPage = itemsPerPage;
      this.logs = this.processLogs(
          await getLog(this.logId, page, itemsPerPage)
      );
      this.loading = false;
    },
    async handlePlotClick({data}) {
      this.loading = true;
      let {page, records} = await getLogByTime(
          this.logId,
          data[0],
          this.itemsPerPage
      );
      this.currentPage = page;
      this.logs = this.processLogs(records);
      this.loading = false;
    },
    processLogs(logs) {
      return logs.map((item) => {
        let time = new Date(item.time);
        return {
          ...item,
          time: `${time.toLocaleString()}.${time.getMilliseconds()}`,
        };
      });
    },
    configTree(config, idCount) {
      let res = [];
      for (const property in config) {
        ++idCount;
        if (typeof config[property] === "object") {
          res.push({
            id: idCount,
            name: property,
            children: this.configTree(config[property], idCount),
          });
        } else {
          res.push({
            id: idCount,
            name: `${property}: ${config[property]}`,
          });
        }
      }
      return res;
    },
    async handleDelete() {
      this.isDeleting = true
      this.deletingLogId = this.logMeta.id;
      await removeLog(this.curtMenuLogId);
      await this.$store.dispatch("log/refreshLogList");
      this.isDeleting = false
      this.$router.push({path: "/board/index"});
    }
  },
};
</script>
