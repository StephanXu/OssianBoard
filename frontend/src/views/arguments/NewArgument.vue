<template>
  <v-dialog v-model="value" persistent max-width="800px">
    <v-card>
      <v-card-title class="headline align-center">
        New schema
      </v-card-title>
      <v-divider></v-divider>
      <v-card-text>
        <v-container>
          <v-row>
            <v-col cols="12" sm="12" md="12">
              <v-text-field label="Name*" required></v-text-field>
            </v-col>
            <v-col cols="12" sm="12" md="12">
              <v-textarea
                  label="JSON schema*"
                  hint="Put json schema here"
                  auto-grow
                  required
                  outlined
              ></v-textarea>
            </v-col>
          </v-row>
        </v-container>
        <small>*indicates required field</small>
      </v-card-text>
      <v-divider></v-divider>
      <v-card-actions>
        <v-spacer></v-spacer>
        <v-btn color="blue darken-1" text @click="$emit('input', false)">Cancel</v-btn>
        <v-btn color="blue darken-1" text @click="$emit('input', false)">Apply</v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script>
export default {
  name: "NewArgument",
  props: {
    value: {
      type: Boolean,
      default: () => false,
    },
  },
  data() {
    return {
      newSchema: {
        name: '',
        schema: '',
        content: ''
      }
    }
  }
}
</script>

<style scoped>

</style>