<template>
  <v-dialog v-model="value" max-width="800px" persistent>
    <v-card>
      <v-card-title class="justify-center">
        <strong>Import</strong>
      </v-card-title>
      <v-divider></v-divider>
      <editor
          v-model="importContent"
          @init="editorInit"
          :options="contentEditorOptions"
          lang="json"
          :theme="editorTheme"
          width="100%"
          height="500px">
      </editor>
      <v-divider></v-divider>
      <v-alert dense text type="success" tile style="margin-bottom:0px">
        Modified <strong>{{ changeSummary.modifyCount }}</strong>.
      </v-alert>
      <v-alert dense text type="info" tile style="margin-bottom:0px">
        Keep <strong>{{ changeSummary.defaultCount }}</strong>.
      </v-alert>
      <v-card-actions>
        <v-btn text @click="onImport">
          <v-icon left>mdi-file-import</v-icon>
          Import
        </v-btn>
        <v-spacer></v-spacer>
        <v-btn color="blue darken-1" text @click="$emit('input', false)">Done</v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script>
import {joinSchemaContent} from "@/utils/utility";

export default {
  name: "ImportArgument.vue",
  components: {
    editor: require("vue2-ace-editor")
  },
  props: {
    value: {
      type: Boolean,
      default: () => false
    },
    schema: {
      type: Object,
      default: () => null
    },
    content: {
      type: Object,
      default: () => null
    }
  },
  computed: {
    editorTheme() {
      if (this.$vuetify.theme.dark) {
        return 'monokai'
      } else {
        return 'dawn'
      }
    },
  },
  data() {
    return {
      importContent: '',
      newContent: {},
      changeSummary: {
        modifyCount: 0,
        addedCount: 0,
        defaultCount: 0
      },
      contentEditorOptions: {
        fontSize: 15,
        animatedScroll: true,
        enableBasicAutocompletion: true,
        enableSnippets: true,
        enableLiveAutocompletion: true,
        readOnly: false
      },
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

    onImport: function () {
      this.$emit('import', this.newContent)
      this.$emit('input', false)
    }
  },
  watch: {
    importContent(val) {
      try {
        let {content, defaultCount, modifyCount, addedCount} = joinSchemaContent(this.schema, this.content, JSON.parse(val))
        this.newContent = content
        this.changeSummary = {
          modifyCount,
          defaultCount,
          addedCount
        }
      } catch (err) {
        return
      }
    }
  }
}
</script>

<style scoped>

</style>