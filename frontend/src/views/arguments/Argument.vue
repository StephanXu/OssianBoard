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

    <editor
        v-if="isNew"
        v-model="newSchema"
        @init="editorInit"
        :options="editorOptions"
        lang="json"
        theme="monokai"
        width="100%"
        height="300px">
    </editor>
    <v-container fluid>
      <v-container fluid class="d-flex justify-space-between">
        <div>
          <v-btn color="primary" text @click="handleBackButton">
            <v-icon left>mdi-arrow-left</v-icon>
            Back to list
          </v-btn>
          <h1>{{ arg.name }}</h1>
          <span class="text-subtitle-1">{{ arg.description }}</span>
        </div>
        <v-btn depressed large color="success" @click="handleSave" :loading="isSaving">
          <v-icon left>save</v-icon>
          Save
        </v-btn>
      </v-container>
      <FormSchema :schema="arg.schema" v-model="arg.content"></FormSchema>
    </v-container>
  </div>
</template>

<script>
import {getSingleArguments, createArguments, updateSingleArguments} from "@/api/arguments";
import {mapGetters} from 'vuex'
import FormSchema from "@/views/arguments/FormSchema";

export default {
  name: "Argument",
  components: {
    FormSchema,
    editor: require("vue2-ace-editor")
  },
  props: {
    argId: {
      type: String,
      default: () => 'new'
    }
  },
  computed: {
    ...mapGetters('argument', ['argumentMetaList']),
    newSchema: {
      get() {
        return this.newSchemaJson
      },
      set(val) {
        this.newSchemaJson = val
        this.arg.schema = JSON.parse(val)
        if (this.arg.schema.hasOwnProperty('meta') && this.arg.schema.meta.hasOwnProperty('title')) {
          this.arg.name = this.arg.schema.meta.title
          this.arg.description = this.arg.schema.meta.description
        }
      }
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
      }
    }
  },
  methods: {
    editorInit: function () {
      require("brace/ext/language_tools");
      require("brace/mode/html");
      require("brace/mode/javascript");
      require("brace/mode/less");
      require("brace/theme/monokai");
      require("brace/snippets/javascript");

      require("brace");
      require("brace/mode/json");
      require("brace/snippets/json");
      require("brace/snippets/html");
    },
    handleBackButton() {
      this.$router.push({path: 'index'})
    },
    handleSave() {
      this.isSaving = true
      let saveProc = this.isNew ? createArguments(this.arg) : updateSingleArguments(this.arg)
      saveProc.then(() => {
        this.successTip = {
          message: "Save success",
          visible: true,
        }
        this.isSaving = false
        if (this.isNew) {
          this.$router.push({path: `index`})
        }
      }).catch(error => {
        this.errorTip = {
          message: "Save failed",
          visible: true,
        }
        this.isSaving = false
        return Promise.reject(error)
      })
    }
  },
  async created() {
    if (this.argId === 'new') {
      this.isNew = true
      this.arg = {
        id: '',
        createTime: Date.now(),
        name: 'New Schema',
        schema: {},
        content: {}
      }
    } else {
      let metaData = this.argumentMetaList.find(item => item.id === this.argId)
      this.arg.id = metaData.id
      this.arg.createTime = metaData.createTime
      this.arg.name = metaData.name
      this.arg = await getSingleArguments(this.argId)
    }

  }
}
</script>

<style scoped>

</style>