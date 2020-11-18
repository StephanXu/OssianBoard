<template>
  <v-dialog v-model="value" max-width="800px" persistent>
    <v-card>
      <v-card-title class="justify-center">
        <strong>Snapshots</strong>
      </v-card-title>
      <v-divider></v-divider>
      <v-virtual-scroll
          :items="snapshotList"
          :item-height="50"
          height="500"
          bench="2"
          :class="scrollbarTheme"
      >
        <template v-slot="{ item }">
          <v-list-item>
            <v-list-item-content>
              <v-list-item-title>
                <span>{{ item.name }}</span>
                <v-chip class="ml-5" x-small outlined v-if="item.tag && item.tag.length>0">
                  <v-icon x-small left>mdi-tag</v-icon>
                  {{ item.tag }}
                </v-chip>
              </v-list-item-title>
              <v-list-item-subtitle>{{ convertCreateTime(item.createTime) }}</v-list-item-subtitle>
            </v-list-item-content>

            <v-list-item-action>
              <div>
                <v-btn icon depressed class="ml-2" @click="handleViewSnapshot(item.id)">
                  <v-icon>mdi-open-in-new</v-icon>
                </v-btn>
                <v-btn icon depressed color="error" @click="handleDeleteSnapshot(item.id)" class="ml-2">
                  <v-icon>mdi-delete</v-icon>
                </v-btn>
              </div>
            </v-list-item-action>
          </v-list-item>
        </template>
      </v-virtual-scroll>

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
    <new-snapshot v-model="isNewSnapshotVisible" :arg-id="argId"></new-snapshot>
    <loading-dialog v-model="isDeleting"></loading-dialog>
  </v-dialog>
</template>

<script>
import NewSnapshot from "@/views/arguments/NewSnapshot";
import LoadingDialog from "@/views/utils/LoadingDialog";
import {listArgumentsSnapshot, deleteArgumentsSnapshot} from "@/api/arguments";
import {timeToString} from "@/utils/utility";

export default {
  name: "SnapshotList",
  components: {
    NewSnapshot,
    LoadingDialog
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
  computed: {
    scrollbarTheme() {
      return this.$vuetify.theme.dark ? 'dark' : 'light'
    },
  },
  data() {
    return {
      isNewSnapshotVisible: false,
      snapshotList: [],
      isDeleting: false
    }
  },
  methods: {
    async handleDeleteSnapshot(snapshotId) {
      this.isDeleting = true
      await deleteArgumentsSnapshot(snapshotId)
      this.snapshotList = await listArgumentsSnapshot(this.argId)
      this.isDeleting = false
    },
    handleViewSnapshot(snapshotId) {
      this.$router.push({path: `/argument/${this.argId}/snapshot/${snapshotId}`})
    },
    convertCreateTime(t) {
      return timeToString(t)
    }
  },
  async mounted() {
    this.snapshotList = await listArgumentsSnapshot(this.argId)
  },
  watch: {
    async isNewSnapshotVisible(newVal) {
      if (!newVal) {
        this.snapshotList = await listArgumentsSnapshot(this.argId)
      }
    }
  }
}
</script>

<style scoped>

</style>