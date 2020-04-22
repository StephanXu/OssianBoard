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
                @contextmenu.prevent="handleContextMenu($event, item.id)"
                outlined
                :hover="!active"
                link
                :disabled="item.id == deletingLogId"
                :loading="item.id == deletingLogId"
                :to="`/board/${item.id}`"
              >
                <v-card-title>
                  {{ item.name }}
                </v-card-title>
                <v-card-subtitle>{{ item.description }}</v-card-subtitle>
                <v-card-subtitle>{{ item.createTime }}</v-card-subtitle>
              </v-card>
            </v-item>
          </v-col>
        </v-row>
      </v-container>
    </v-item-group>

    <!-- Menu -->
    <v-menu
      v-model="showMenu"
      :position-x="menuPosX"
      :position-y="menuPosY"
      absolute
      offset-y
    >
      <v-list dense>
        <v-list-item @click="handleEditLog">
          <v-list-item-icon
            ><v-icon>mdi-file-edit-outline</v-icon></v-list-item-icon
          >
          <v-list-item-title>编辑</v-list-item-title>
        </v-list-item>
        <v-list-item @click="handleDeleteLog">
          <v-list-item-icon
            ><v-icon>mdi-trash-can-outline</v-icon></v-list-item-icon
          >
          <v-list-item-title>删除</v-list-item-title>
        </v-list-item>
      </v-list>
    </v-menu>

    <!-- Edit dialog -->
    <v-dialog v-model="showLogEditDialog" persistent max-width="600px">
      <v-card :loading="editSaveingStatus">
        <v-card-title>
          <span class="headline">Edit log</span>
        </v-card-title>
        <v-card-text>
          <v-container>
            <v-row>
              <v-col cols="12">
                <v-text-field
                  label="Log name"
                  required
                  v-model="editingLog.name"
                  :disabled="editSaveingStatus"
                ></v-text-field>
              </v-col>
              <v-col cols="12">
                <v-text-field
                  label="Description"
                  required
                  v-model="editingLog.description"
                  :disabled="editSaveingStatus"
                ></v-text-field>
              </v-col>
            </v-row>
          </v-container>
        </v-card-text>
        <v-card-actions>
          <v-spacer></v-spacer>
          <v-btn
            color="blue darken-1"
            text
            @click="handleEditLogDialogCancel"
            :disabled="editSaveingStatus"
          >
            Cancel
          </v-btn>
          <v-btn
            color="blue darken-1"
            text
            @click="handleSaveLog"
            :disabled="editSaveingStatus"
            >Save
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </v-navigation-drawer>
</template>

<script>
import { mapGetters } from "vuex";
import { removeLog, updateLog } from "@/api/log";
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
      showMenu: false,
      menuPosX: 0,
      menuPosY: 0,
      curtMenuLogId: "",
      deletingLogId: "",
      showLogEditDialog: false,
      curtEditLogId: "",
      editSaveingStatus: false,
      editingLog: {
        name: "",
        description: "",
      },
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
      set() {},
    },
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
    handleContextMenu(event, logId) {
      this.showMenu = false;
      this.menuPosX = event.clientX;
      this.menuPosY = event.clientY;
      this.$nextTick(() => {
        this.showMenu = true;
      });
      this.curtMenuLogId = logId;
    },
    async handleDeleteLog() {
      this.deletingLogId = this.curtMenuLogId;
      await removeLog(this.curtMenuLogId);
      await this.$store.dispatch("log/refreshLogList");
      if (this.curtMenuLogId == this.selectedLogId) {
        this.$router.push({ path: "/board/index" });
      }
    },
    handleEditLogDialogCancel() {
      this.curtEditLogId = "";
      this.showLogEditDialog = false;
    },
    handleEditLog() {
      this.curtEditLogId = this.curtMenuLogId;
      let log = this.logList.find((item) => item.id == this.curtEditLogId);
      if (log == null) {
        return;
      }
      this.editingLog.name = log.name;
      this.editingLog.description = log.description;
      this.showLogEditDialog = true;
    },
    async handleSaveLog() {
      this.editSaveingStatus = true;
      await updateLog(this.curtEditLogId, this.editingLog);
      await this.$store.dispatch("log/refreshLogList");
      this.showLogEditDialog = false;
      this.editSaveingStatus = false;
    },
  },
};
</script>
