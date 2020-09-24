<template>
  <v-dialog v-model="value" max-width="500px" persistent>
    <v-snackbar v-model="errorTip.visible" color="error" top>
      {{ errorTip.message }}
      <template v-slot:action="{ attrs }">
        <v-btn text v-bind="attrs" @click="errorTip.visible = false">OK</v-btn>
      </template>
    </v-snackbar>

    <v-card>
      <v-card-title class="justify-center">
        <strong>New Snapshot</strong>
      </v-card-title>
      <v-divider></v-divider>
      <v-card-text>
        <v-container class="mt-5">
          <v-text-field outlined v-model="snapshotName" label="Snapshot name*"></v-text-field>
          <v-text-field outlined v-model="snapshotTag" label="Tag"></v-text-field>
        </v-container>
      </v-card-text>
      <v-divider></v-divider>
      <v-card-actions>
        <v-spacer></v-spacer>
        <v-btn color="blue darken-1" text @click="$emit('input', false)">Cancel</v-btn>
        <v-btn color="blue darken-1" text @click="handleApply" :loading="isSaving">Apply</v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script>
import {createArgumentsSnapshot, tagArgumentsSnapshot} from "@/api/arguments";
import {timeToString} from "@/utils/utility";

export default {
  name: "NewSnapshot",
  props: {
    value: {
      type: Boolean,
      default: () => false
    },
    argId: {
      type: String,
      default: () => ''
    }
  },
  computed: {},
  data() {
    return {
      snapshotName: '',
      snapshotTag: '',
      isSaving: false,
      errorTip: {
        message: "",
        visible: false,
      }
    }
  },
  methods: {
    handleApply() {
      this.isSaving = true
      createArgumentsSnapshot(this.argId, this.snapshotName).then(async response => {
        if (this.snapshotTag.length > 0) {
          await tagArgumentsSnapshot(response.id, this.snapshotTag)
        }
        this.$emit('input', false)
        this.isSaving = false
      }).catch(() => {
        this.errorTipTip = {
          message: 'Create snapshot failed',
          visible: true
        }
        this.isSaving = false
      })
    },
  },
  watch: {
    value(newVal) {
      if (newVal) {
        this.snapshotName = `Snapshot-${timeToString(Date.now())}`
        this.snapshotTag = ''
      }
    }
  }
}
</script>

<style scoped>

</style>