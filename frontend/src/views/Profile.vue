<template>
  <v-container>
    <v-row align="center" justify="center">
      <v-col cols="12">
        <v-card class="mx-auto profile-card" tle>
          <v-card-title>
            Profile
          </v-card-title>

          <v-list-item>
            <v-list-item-avatar color="grey"></v-list-item-avatar>
            <v-list-item-content>
              <v-list-item-title class="headline">{{alias}}</v-list-item-title>
              <v-list-item-subtitle>{{name}}</v-list-item-subtitle>
            </v-list-item-content>
          </v-list-item>
        </v-card>
      </v-col>
    </v-row>
  </v-container>
</template>

<script>
import {mapGetters} from 'vuex'

export default {
  computed: {
    ...mapGetters(['name', 'alias']),
  }
}
</script>

<style scoped>
.profile-card {
  margin-bottom: 20px;
  max-width: 850px
}
</style>
