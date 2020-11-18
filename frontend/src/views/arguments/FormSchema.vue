<template>
  <v-container fluid>
    <v-tabs v-model="tabIndex" background-color="transparent">
      <v-tab v-for="(tab,index) in schema.properties" :key="index">{{ tab.title }}</v-tab>
    </v-tabs>
    <v-tabs-items v-model="tabIndex" class="transparent">
      <v-tab-item v-for="(tab,index) in schema.properties" :key="index">
        <div class="d-flex flex-column">
          <v-card
              v-for="(card,cardIndex) in tab.properties"
              :key="cardIndex"
              outlined
              class="mt-5">
            <v-card-title>{{ card.title }}</v-card-title>
            <v-card-subtitle>{{ card.description }}</v-card-subtitle>
            <v-card-text>
              <div
                  v-for="(field,fieldIndex) in card.properties"
                  :key="fieldIndex"
                  class="mt-2">
                <v-text-field
                    v-if="field.component==='text' && field.type==='string'"
                    type="text"
                    v-model.trim="refreshedValue[index][cardIndex][fieldIndex]"
                    :label="field.title"
                    outlined
                    :hint="field.description"
                    persistent-hint>
                </v-text-field>
                <v-text-field
                    v-if="field.component==='text' && field.type==='number'"
                    type="number"
                    v-model.number="refreshedValue[index][cardIndex][fieldIndex]"
                    :label="field.title"
                    outlined
                    :hint="field.description"
                    persistent-hint>
                </v-text-field>
                <v-select
                    v-if="field.component==='select'"
                    v-model="refreshedValue[index][cardIndex][fieldIndex]"
                    :label="field.title"
                    item-text="label"
                    item-value="value"
                    :items="selectFieldItemObjectConverter(field)"
                    outlined
                    :hint="field.description"
                    persistent-hint
                ></v-select>
              </div>
            </v-card-text>
          </v-card>
        </div>
      </v-tab-item>
    </v-tabs-items>
  </v-container>
</template>

<script>
import {initSchemaContent} from '@/utils/utility'

export default {
  name: "FormSchema",
  props: {
    value: {
      type: Object,
      default: () => {
        return {}
      }
    },
    schema: {
      type: Object,
      default: () => {
        return {}
      }
    }
  },
  computed: {
    refreshedValue: {
      get() {
        initSchemaContent(this.schema, this.value)
        return this.value
      },
      set(val) {
        this.value = val
        initSchemaContent(this.schema, this.value)
      }
    },
    selectFieldItemObjectConverter() {
      return (fieldObject) => fieldObject.enum.map((item, index) => {
        return {label: fieldObject.enumName[index], value: item}
      })
    }
  },
  data() {
    return {
      tabIndex: 0
    }
  },
  async updated() {
  }
}
</script>

<style scoped>

</style>