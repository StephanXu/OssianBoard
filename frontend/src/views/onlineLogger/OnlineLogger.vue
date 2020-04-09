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
                {{ item.level }}</v-chip
              >
            </template>
          </v-data-table>
        </v-card>
      </v-col>
    </v-row>
    <v-row v-for="item in figures" :key="item.name">
      <v-col cols="12">
        <v-card class="mx-auto" tle>
          <v-card-title>{{ item.name }}</v-card-title>
          <plot
            style="height: 500px; width: 100%;"
            :values="item.variables"
          ></plot>
        </v-card>
      </v-col>
    </v-row>
  </v-container>
</template>

<script>
import Plot from "./components/Plot";
import { mapGetters } from "vuex";
import { getLog } from "@/api/log";
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
        { text: "Thread", value: "threadId", width: "10%" },
        { text: "Level", value: "level", width: "10%" },
        { text: "Content", value: "content" },
      ],
      loading: false,
      itemsPerPage: 10,
      currentPage: 1,
      logMeta: {
        id: "",
        createTime: "",
        description: "",
        name: "",
        recordCount: 0,
      },
    };
  },
  computed: {
    ...mapGetters("log", ["connection", "connectionStatus", "logList"]),
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
  },
  async created() {
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

    this.connection.on("ReceiveLog", (increment) => {
      this.mergePlot(increment.plots);
      this.logMeta.recordCount += increment.records.length;
      if (this.currentPage == 1) {
        if (increment.records.length >= this.itemsPerPage) {
          this.logs = increment.records.slice(0, this.itemsPerPage);
        } else {
          this.logs = this.logs
            .slice(0, this.itemsPerPage - increment.records.length)
            .concat(increment.records);
        }
      }
    });
    this.loading = true;
    this.connection.invoke("ListenLog", this.logId);
    this.connection.invoke("GetPlots", this.logId).then((res) => {
      this.mergePlot(res);
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
            this.figures[plotIndex].variables.push(variable);
            return;
          }
          let dots = this.figures[plotIndex].variables[variableIndex].dots;
          this.figures[plotIndex].variables[variableIndex].dots = dots.concat(
            variable.dots
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
    async handlePagination({ page, itemsPerPage }) {
      this.loading = true;
      this.currentPage = page;
      this.itemsPerPage = itemsPerPage;
      this.logs = await getLog(this.logId, page, itemsPerPage);
      this.loading = false;
    },
  },
};
</script>
