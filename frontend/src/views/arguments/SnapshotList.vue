<template>
  <v-dialog v-model="value" persistent max-width="800px">
    <v-card>
      <v-card-title class="headline align-center">
        Snapshots
      </v-card-title>
      <v-divider></v-divider>
      <v-card-text>
        <v-container>

        </v-container>
      </v-card-text>
      <v-divider></v-divider>
      <v-card-actions>
        <v-btn text @click="isNewSnapshotVisible=true">
          <v-icon left>mdi-plus</v-icon>
          Create snapshot
        </v-btn>
        <v-spacer></v-spacer>
        <v-btn color="blue darken-1" text @click="$emit('input', false)">Done</v-btn>
      </v-card-actions>
    </v-card>
    <new-snapshot v-model="isNewSnapshotVisible"></new-snapshot>
  </v-dialog>
</template>

<script>
import NewSnapshot from "@/views/arguments/NewSnapshot";
import {listArgumentsSnapshot} from "@/api/arguments";

export default {
  name: "SnapshotList",
  components: {
    NewSnapshot
  },
  props: {
    value: {
      type: Boolean,
      default: () => false
    },
    argId: {
      type: String,
      default: () => null
    }
  },
  data() {
    return {
      isNewSnapshotVisible: false,
      snapshotList: []
    }
  },
  async created() {
    this.snapshotList = await listArgumentsSnapshot(this.argId)

  }
}
</script>

<style scoped>

</style>