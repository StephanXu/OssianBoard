<template>
  <div>
    <v-snackbar v-model="successTip.visible" color="success" top>
      {{ successTip.message }}
      <template v-slot:action="{ attrs }">
        <v-btn text v-bind="attrs" @click="successTip.visible = false"
          >OK</v-btn
        >
      </template>
    </v-snackbar>

    <v-snackbar v-model="errorTip.visible" color="error" top>
      {{ errorTip.message }}
      <template v-slot:action="{ attrs }">
        <v-btn text v-bind="attrs" @click="errorTip.visible = false">OK</v-btn>
      </template>
    </v-snackbar>

    <v-row no-gutters>
      <v-col>
        <v-card flat :style="rowStyle" :class="scrollbarTheme">
          <v-container fluid flat class="d-flex justify-space-between flex-wrap">
            <div class="ms-2">
              <v-btn color="primary" text @click="handleBackButton">
                <v-icon left>mdi-arrow-left</v-icon>
                Back to {{ isSnapshot ? "argument" : "list" }}
              </v-btn>
              <h1>{{ arg.name }}</h1>
              <p class="text-subtitle-1">{{ arg.description }}</p>
              <v-btn
                outlined
                dense
                :color="isSnapshot ? 'gray' : 'primary'"
                @click="handleCopyId"
                :disabled="isNew"
              >
                <v-icon left>{{
                  isSnapshot ? "event" : "mdi-link-variant"
                }}</v-icon>
                {{ presentId }}
              </v-btn>
              <v-subheader class="ml-n4">{{ presentCreateTime }}</v-subheader>
            </div>
            <div class="d-flex flex-column">
              <v-btn
                small
                depressed
                v-if="!isNew && !isSnapshot"
                class="mt-2"
                @click="isSnapshotListVisible = true"
              >
                <v-icon left>event</v-icon>
                Snapshot
              </v-btn>
              <v-btn
                small
                depressed
                v-if="!isNew"
                color="error"
                class="mt-2"
                @click="handleDelete"
                :loading="isDeleting"
              >
                <v-icon left>delete</v-icon>
                Delete
              </v-btn>
              <v-btn
                small
                depressed
                v-if="!isNew"
                class="mt-2"
                @click="isShowRaw = !isShowRaw"
              >
                <v-icon left>code</v-icon>
                {{ isShowRaw ? "Hide raw" : "Show raw" }}
              </v-btn>
              <v-btn
                small
                depressed
                v-if="!isNew"
                class="mt-2"
                @click="isImportDialogVisible = true"
              >
                <v-icon left>mdi-file-import</v-icon>
                Import
              </v-btn>
            </div>
          </v-container>
          <form-schema :schema="arg.schema" v-model="arg.content">
            <template v-slot:operation>
              <v-btn
                small
                depressed
                color="success"
                @click="handleSave"
                :loading="isSaving"
              >
                <v-icon left>save</v-icon>
                Save
              </v-btn>
            </template>
          </form-schema>
        </v-card>
      </v-col>
      <v-col cols="6" v-if="isNew || isShowRaw">
        <div :style="rowStyle" v-if="!isShowRaw">
          <editor
            v-model="newSchema"
            @init="editorInit"
            :options="editorOptions"
            lang="json"
            :theme="editorTheme"
            width="100%"
            height="100%"
          >
          </editor>
        </div>
        <div :style="rowStyle" v-if="isShowRaw">
          <editor
            v-model="rawSchema"
            @init="editorInit"
            :options="contentEditorOptions"
            lang="json"
            :theme="editorTheme"
            width="100%"
            height="50%"
          >
          </editor>
          <editor
            v-model="rawContent"
            @init="editorInit"
            :options="contentEditorOptions"
            lang="json"
            :theme="editorTheme"
            width="100%"
            height="50%"
          >
          </editor>
        </div>
      </v-col>
    </v-row>
    <snapshot-list
      :arg-id="this.argId"
      v-model="isSnapshotListVisible"
    ></snapshot-list>
    <import-argument
      v-model="isImportDialogVisible"
      :schema="arg.schema"
      :content="arg.content"
      @import="onImport"
    >
    </import-argument>
  </div>
</template>

<script>
import {
  getSingleArguments,
  createArguments,
  updateSingleArguments,
  deleteArguments,
  getSingleArgumentsSnapshot,
  deleteArgumentsSnapshot,
} from "@/api/arguments";
import { mapGetters } from "vuex";
import FormSchema from "@/views/arguments/FormSchema";
import SnapshotList from "@/views/arguments/SnapshotList";
import ImportArgument from "@/views/arguments/ImportArgument";
import { timeToString } from "@/utils/utility";

export default {
  name: "Argument",
  components: {
    ImportArgument,
    FormSchema,
    SnapshotList,
    editor: require("vue2-ace-editor"),
  },
  props: {
    argId: {
      type: String,
      default: () => "new",
    },
    snapshotId: {
      type: String,
      default: () => "latest",
    },
  },
  computed: {
    ...mapGetters("argument", ["argumentMetaList"]),
    ...mapGetters("view", ["clientHeight", "clientWidth"]),
    newSchema: {
      get() {
        return this.newSchemaJson;
      },
      set(val) {
        this.newSchemaJson = val;
        this.arg.schema = JSON.parse(val);
        if (this.isNew) {
          localStorage.newSchema = val;
        }
        if (
          this.arg.schema.hasOwnProperty("description") &&
          this.arg.schema.hasOwnProperty("title")
        ) {
          this.arg.name = this.arg.schema.title;
          this.arg.description = this.arg.schema.description;
        }
      },
    },
    rawSchema() {
      return JSON.stringify(this.arg.schema, null, 2);
    },
    rawContent: {
      get() {
        console.log(this.arg.content);
        return JSON.stringify(this.arg.content, null, 2);
      },
      set() {},
    },
    editorTheme() {
      if (this.$vuetify.theme.dark) {
        return "monokai";
      } else {
        return "dawn";
      }
    },
    rowStyle() {
      return {
        "overflow-y": "auto",
        height: `${
          this.clientHeight -
          this.$vuetify.application.top -
          this.$vuetify.application.footer
        }px`,
      };
    },
    scrollbarTheme() {
      return this.$vuetify.theme.dark ? "dark" : "light";
    },
    isSnapshot() {
      return this.snapshotId != "latest";
    },
    presentId() {
      if (this.isNew) {
        return "Present after creation";
      } else if (this.isSnapshot) {
        return this.snapshotId;
      } else {
        return this.arg.id;
      }
    },
    presentCreateTime() {
      if (this.isNew) {
        return "";
      } else if (this.isSnapshot) {
        return `Snapshot created at ${timeToString(this.arg.createTime)}`;
      } else {
        return `Created at ${timeToString(this.arg.createTime)}`;
      }
    },
  },
  data() {
    return {
      arg: {
        id: "",
        createTime: Date.now(),
        name: "Unknown",
        schema: {},
        content: {},
      },
      isNew: false,
      isShowRaw: false,
      newSchemaJson: "",
      editorOptions: {
        fontSize: 15,
        animatedScroll: true,
        enableBasicAutocompletion: true,
        enableSnippets: true,
        enableLiveAutocompletion: true,
        readOnly: false,
      },
      contentEditorOptions: {
        fontSize: 15,
        animatedScroll: true,
        enableBasicAutocompletion: true,
        enableSnippets: true,
        enableLiveAutocompletion: true,
        readOnly: true,
      },
      isSaving: false,
      successTip: {
        message: "",
        visible: false,
      },
      errorTip: {
        message: "",
        visible: false,
      },
      isSnapshotListVisible: false,
      isDeleting: false,
      isImportDialogVisible: false,
    };
  },
  methods: {
    editorInit: function () {
      require("brace/ext/language_tools");
      require("brace/mode/html");
      require("brace/mode/javascript");
      require("brace/mode/less");
      require("brace/theme/monokai");
      require("brace/theme/dawn");
      require("brace/snippets/javascript");

      require("brace");
      require("brace/mode/json");
      require("brace/snippets/json");
      require("brace/snippets/html");
    },
    handleBackButton() {
      if (this.isSnapshot) {
        this.$router.push({ path: `/argument/${this.argId}` });
      } else {
        this.$router.push({ path: "/argument/index" });
      }
    },
    handleSave() {
      this.isSaving = true;
      let saveProc = this.isNew
        ? createArguments(this.arg)
        : updateSingleArguments(this.arg);
      saveProc
        .then((response) => {
          this.successTip = {
            message: "Save success",
            visible: true,
          };
          this.isSaving = false;
          if (this.isNew) {
            this.$router.push({ path: `${response.id}` });
          }
        })
        .catch((error) => {
          this.errorTip = {
            message: "Save failed",
            visible: true,
          };
          this.isSaving = false;
          return Promise.reject(error);
        });
    },
    handleCopyId() {
      this.$copyText(this.presentId).then(
        () => {
          this.successTip = {
            message: "Copied!",
            visible: true,
          };
        },
        () => {
          this.errorTip = {
            message: "Copy failed.",
            visible: true,
          };
        }
      );
    },
    async handleDelete() {
      this.isDeleting = true;
      if (this.isSnapshot) {
        await deleteArgumentsSnapshot(this.snapshotId);
        await this.$router.push({ path: `/argument/${this.argId}` });
        this.isDeleting = false;
      } else {
        await deleteArguments(this.arg.id);
        await this.$store.dispatch("argument/fetchArgumentList");
        await this.$router.push({ path: "/argument/index" });
        this.isDeleting = false;
      }
    },
    onImport(val) {
      this.arg.content = val;
    },
  },
  async created() {
    if (this.argId === "new") {
      this.isNew = true;
      this.arg = {
        id: "",
        createTime: Date.now(),
        name: "New Schema",
        schema: {},
        content: {},
      };
      if (localStorage.newSchema) {
        this.newSchema = localStorage.newSchema;
      }
    } else {
      this.isNew = false;
      let metaData = this.argumentMetaList.find(
        (item) => item.id === this.argId
      );
      if (metaData) {
        this.arg.id = metaData.id;
        this.arg.createTime = metaData.createTime;
        this.arg.name = metaData.name;
      }
      let arg = await getSingleArguments(this.argId);
      if (this.isSnapshot) {
        let snapshot = await getSingleArgumentsSnapshot(this.snapshotId);
        this.arg = {
          ...snapshot,
          schema: JSON.parse(arg.schema),
          content: JSON.parse(snapshot.content),
        };
      } else {
        this.arg = {
          ...arg,
          schema: JSON.parse(arg.schema),
          content: JSON.parse(arg.content),
        };
      }
      if (arg && !metaData) {
        await this.$store.dispatch("argument/fetchArgumentList");
      }
    }
  },
};
</script>

<style scoped>
</style>