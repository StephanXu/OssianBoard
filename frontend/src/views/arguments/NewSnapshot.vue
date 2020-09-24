<template>
  <v-dialog v-model="value" persistent max-width="500px">
    <v-snackbar v-model="successTip.visible" color="success" bottom>
      {{ successTip.message }}
      <v-btn text @click="successTip.visible = false">OK</v-btn>
    </v-snackbar>

    <v-snackbar v-model="errorTip.visible" color="error" bottom>
      {{ errorTip.message }}
      <v-btn text @click="errorTip.visible = false">OK</v-btn>
    </v-snackbar>

    <v-card>
      <v-card-title class="headline align-center">
        New Snapshot
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
import {createArgumentsSnapshot} from "@/api/arguments";

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
  data() {
    return {
      snapshotName: '',
      snapshotTag: '',
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
    handleApply() {
      this.isSaving = true
      createArgumentsSnapshot(this.argId, this.snapshotName).then(() => {
        this.successTip = {
          message: 'Create snapshot success',
          visible: true
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
    }
  },
  created() {
    let t = new Date(Date.now())
    this.snapshotName =
        `Snapshot-${t.getFullYear()}-${t.getMonth().toString().padStart(2, "0")}-${t.getDay().toString().padStart(2, "0")} ${t.getHours().toString().padStart(2, "0")}:${t.getMinutes().toString().padStart(2, "0")}:${t.getSeconds().toString().padStart(2, "0")}`
  }
}
</script>

<style scoped>

</style>