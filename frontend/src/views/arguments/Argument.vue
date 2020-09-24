<template>
  <div>
    <v-snackbar v-model="successTip.visible" color="success" bottom>
      {{ successTip.message }}
      <v-btn text @click="successTip.visible = false">OK</v-btn>
    </v-snackbar>

    <v-snackbar v-model="errorTip.visible" color="error" bottom>
      {{ errorTip.message }}
      <v-btn text @click="errorTip.visible = false">OK</v-btn>
    </v-snackbar>


    <v-row no-gutters>
      <v-col>
        <v-container fluid :style="rowStyle" :class="scrollbarTheme">
          <v-container fluid class="d-flex justify-space-between">
            <div>
              <v-btn color="primary" text @click="handleBackButton">
                <v-icon left>mdi-arrow-left</v-icon>
                Back to list
              </v-btn>
              <h1>{{ arg.name }}</h1>
              <p class="text-subtitle-1">{{ arg.description }}</p>
              <v-btn
                  outlined
                  dense
                  color="primary"
                  @click="handleCopyId"
                  :disabled="isNew">
                <v-icon left>mdi-link-variant</v-icon>
                {{ isNew ? 'Present after creation' : arg.id }}
              </v-btn>
            </div>
            <div class="d-flex flex-column">
              <v-btn small color="success" @click="handleSave" :loading="isSaving">
                <v-icon left>save</v-icon>
                Save
              </v-btn>
              <v-btn small class="mt-2" @click="isSnapshotListVisible=true">
                <v-icon left>event</v-icon>
                Snapshot
              </v-btn>
            </div>
          </v-container>
          <FormSchema :schema="arg.schema" v-model="arg.content"></FormSchema>
        </v-container>
      </v-col>
      <v-col cols="6" v-if="isNew">
        <div :style="rowStyle">
          <editor
              v-model="newSchema"
              @init="editorInit"
              :options="editorOptions"
              lang="json"
              :theme="editorTheme"
              width="100%"
              height="100%">
          </editor>
        </div>
      </v-col>
    </v-row>
    <snapshot-list :arg-id="this.arg.id" v-model="isSnapshotListVisible"></snapshot-list>
  </div>
</template>

<script>
import {
  getSingleArguments,
  createArguments,
  updateSingleArguments
} from "@/api/arguments";
import {mapGetters} from 'vuex'
import FormSchema from "@/views/arguments/FormSchema";
import SnapshotList from "@/views/arguments/SnapshotList";

export default {
  name: "Argument",
  components: {
    FormSchema,
    SnapshotList,
    editor: require("vue2-ace-editor")
  },
  props: {
    argId: {
      type: String,
      default: () => 'new'
    },
    snapshotId: {
      type: String,
      default: () => 'latest'
    }
  },
  computed: {
    ...mapGetters('argument', ['argumentMetaList']),
    ...mapGetters('view', ['clientHeight', 'clientWidth']),
    newSchema: {
      get() {
        return this.newSchemaJson
      },
      set(val) {
        this.newSchemaJson = val
        this.arg.schema = JSON.parse(val)
        if (this.isNew) {
          localStorage.newSchema = val
        }
        if (this.arg.schema.hasOwnProperty('meta') && this.arg.schema.meta.hasOwnProperty('title')) {
          this.arg.name = this.arg.schema.meta.title
          this.arg.description = this.arg.schema.meta.description
        }
      }
    },
    editorTheme() {
      if (this.$vuetify.theme.dark) {
        return 'monokai'
      } else {
        return 'dawn'
      }
    },
    rowStyle() {
      return {
        'overflow-y': 'auto',
        'height': `${this.clientHeight - this.$vuetify.application.top - this.$vuetify.application.footer}px`
      }
    },
    scrollbarTheme() {
      return this.$vuetify.theme.dark ? 'dark' : 'light'
    },
    isSnapshot() {
      return this.snapshotId != 'latest'
    }
  },
  data() {
    return {
      arg: {
        id: '',
        createTime: Date.now(),
        name: 'Unknown',
        schema: {},
        content: {}
      },
      isNew: false,
      newSchemaJson: '',
      editorOptions: {
        fontSize: 15,
        animatedScroll: true,
        enableBasicAutocompletion: true,
        enableSnippets: true,
        enableLiveAutocompletion: true,
        readOnly: false
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
      isSnapshotListVisible: false
    }
  },
  methods: {
    editorInit: function () {
      require("brace/ext/language_tools")
      require("brace/mode/html")
      require("brace/mode/javascript")
      require("brace/mode/less")
      require("brace/theme/monokai")
      require("brace/theme/dawn")
      require("brace/snippets/javascript")

      require("brace")
      require("brace/mode/json")
      require("brace/snippets/json")
      require("brace/snippets/html")
    },
    handleBackButton() {
      this.$router.push({path: '/argument/index'})
    },
    handleSave() {
      this.isSaving = true
      let saveProc = this.isNew ? createArguments(this.arg) : updateSingleArguments(this.arg)
      saveProc.then(response => {
        this.successTip = {
          message: "Save success",
          visible: true,
        }
        this.isSaving = false
        if (this.isNew) {
          this.$router.push({path: `${response.id}`})
        }
      }).catch(error => {
        this.errorTip = {
          message: "Save failed",
          visible: true,
        }
        this.isSaving = false
        return Promise.reject(error)
      })
    },
    handleCopyId() {
      this.$copyText(this.arg.id).then(() => {
        this.successTip = {
          message: "Copied!",
          visible: true
        }
      }, () => {
        this.errorTip = {
          message: "Copy failed.",
          visible: true
        }
      })
    }
  },
  async created() {
    console.log(this.argId)
    console.log(this.snapshotId)
    if (this.argId === 'new') {
      this.isNew = true
      this.arg = {
        id: '',
        createTime: Date.now(),
        name: 'New Schema',
        schema: {},
        content: {}
      }
      if (localStorage.newSchema) {
        this.newSchema = localStorage.newSchema
      }
    } else {
      this.isNew = false
      let metaData = this.argumentMetaList.find(item => item.id === this.argId)
      if (metaData) {
        this.arg.id = metaData.id
        this.arg.createTime = metaData.createTime
        this.arg.name = metaData.name
      }
      let arg = await getSingleArguments(this.argId)
      this.arg = {
        ...arg,
        schema: JSON.parse(arg.schema),
        content: JSON.parse(arg.content)
      }
      if (arg && !metaData) {
        await this.$store.dispatch('argument/fetchArgumentList')
      }
    }

  }
}
</script>

<style scoped>
</style>