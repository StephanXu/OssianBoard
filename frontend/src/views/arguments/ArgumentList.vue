<template>
  <v-container fluid>
    <v-container class="pl-3">
      <v-btn small color="primary" @click="handleCreateNewSchema">New</v-btn>
    </v-container>
    <v-container class="pl-3">
      <div>
        <h2 class="ml-0 pl-0 pt-0">All schema:</h2>
      </div>
      <v-item-group class="ml-n3">
        <v-container class="d-flex">
          <v-card v-for="argItem in argumentMetaList"
                  :key="argItem.id"
                  height="200"
                  width="325"
                  class="d-flex align-center justify-center mr-5"
                  @click="handleArgumentsCardClick(argItem.id)">
            <div class="display-2 flex-grow-1 text-center d-block">
              {{ argItem.name }}
            </div>
          </v-card>
        </v-container>
      </v-item-group>
    </v-container>
  </v-container>
</template>

<script>
import {mapGetters} from 'vuex'

export default {
  name: "ArgumentList",
  computed: {
    ...mapGetters('argument', ['argumentMetaList'])
  },
  data() {
    return {
      newSchemaVisible: false
    }
  },
  async created() {
    if (this.argumentMetaList.length == 0) {
      await this.$store.dispatch('argument/fetchArgumentList')
    }
  },
  methods: {
    handleArgumentsCardClick(argId) {
      this.$router.push({path: `/argument/${argId}`})
    },
    handleCreateNewSchema() {
      this.$router.push({path: '/argument/new'})
    }
  }
}
</script>

<style scoped>

</style>