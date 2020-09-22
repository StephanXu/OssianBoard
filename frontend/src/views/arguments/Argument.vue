<template>
  <v-container fluid>
    <h2>{{ arg.name }}</h2>
      <v-navigation-drawer
          absolute
          permanent
          right
          width="600px"
      >
        <v-divider></v-divider>

        <v-container>
          <v-textarea auto-grow>

          </v-textarea>
        </v-container>

      </v-navigation-drawer>
  </v-container>
</template>

<script>
import {getSingleArguments} from "@/api/arguments";
import {mapGetters} from 'vuex'

export default {
  name: "Argument",
  props: {
    argId: {
      type: String,
      default: () => 'new'
    }
  },
  computed: {
    ...mapGetters('argument', ['argumentMetaList'])
  },
  data() {
    return {
      arg: {
        id: null,
        createTime: Date.now(),
        name: 'Unknown',
        schema: {},
        content: {}
      },
      isNew: false
    }
  },
  async created() {
    let metaData = this.argumentMetaList.find(item => item.id === this.argId)
    this.arg.id = metaData.id
    this.arg.createTime = metaData.createTime
    this.arg.name = metaData.name
    if (this.argId === 'new') {
      this.isNew = true
      this.arg = {
        id: null,
        createTime: Date.now(),
        name: 'New Schema',
        schema: {},
        content: {}
      }
    } else {
      this.arg = await getSingleArguments(this.argId)
    }

  }
}
</script>

<style scoped>

</style>