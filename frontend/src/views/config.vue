<template>
  <v-container fluid>
    <v-snackbar v-model="successTip.visible" top color="success">
      {{ successTip.message }}
      <v-btn text @click="successTip.visible = false">好</v-btn>
    </v-snackbar>

    <v-snackbar v-model="errorTip.visible" top color="error">
      {{ errorTip.message }}
      <v-btn text @click="errorTip.visible = false">好</v-btn>
    </v-snackbar>
    <v-row align="center" justify="center">
      <v-col cols="12">
        <v-card
          v-for="cate in table"
          :key="cate.alias"
          style="padding:20px;margin-bottom:20px;max-width: 850px"
          class="mx-auto"
          tle
        >
          <v-card-title>{{cate.alias}}</v-card-title>
          <div v-for="item in cate.child" :key="item.alias">
            <v-text-field
              v-if="item.type==='input'"
              :label="item.alias"
              v-model="form[cate.alias][item.alias]"
            />
            <v-switch
              v-else-if="item.type==='bool'"
              v-model="form[cate.alias][item.alias]"
              :label="item.alias"
            ></v-switch>
            <v-select v-else-if="item.type==='enum'" v-model="form[cate.alias][item.alias]" :items="item.values"
                      :label="item.alias"></v-select>
          </div>
        </v-card>
      </v-col>
    </v-row>
    <v-speed-dial
      v-model="fab"
      bottom
      right
      fixed
      direction="top"
      open-on-hover
      transition="slide-y-reverse-transition"
    >
      <template v-slot:activator>
        <v-btn v-model="fab" color="blue darken-2" dark fab
        >
          <v-icon v-if="fab">mdi-close</v-icon>
          <v-icon v-else>mdi-account-circle</v-icon>
        </v-btn>
      </template>
      <v-btn color="red" dark fab small>
        <v-icon @click="onReset">restore</v-icon>
      </v-btn>
      <v-btn color="green" dark fab small>
        <v-icon @click="onSubmit">save</v-icon>
      </v-btn>
      <v-btn color="indigo" dark fab small>
        <v-icon @click="refresh">refresh</v-icon>
      </v-btn>
    </v-speed-dial>
  </v-container>
</template>

<script>
import {getArguments, setArguments, resetArguments} from "@/api/arguments";
import protoRoot from "../proto/proto";
import {createTable, createTableContentObject} from "../proto/parser";

export default {
  props: {
    source: String
  },
  data: () => ({
    drawer: null,
    fab: '',
    form: {},
    table: [],
    successTip: {
      message: "",
      visible: false
    },
    errorTip: {
      message: "",
      visible: false
    }
  }),
  methods: {
    onSubmit() {
      let configType = protoRoot.lookupType("Configuration");
      let config = configType.fromObject(this.form);
      setArguments(config.toJSON())
        .then(() => {
          this.successTip = {
            message: "保存成功",
            visible: true
          };
        })
        .catch(() => {
          this.errorTip = {
            message: "保存失败",
            visible: true
          };
        });
    },
    onReset() {
      resetArguments()
        .then(() => {
          this.successTip = {
            message: "还原成功",
            visible: true
          };
          this.refresh();
        })
        .catch(err => {
          if (err) {
            console.log(err)
            this.errorTip = {
              message: "还原失败",
              visible: true
            };
          }
        });
    },
    refresh() {
      getArguments()
        .then(response => {
          this.form = response;
        })
        .catch(() => {
          this.errorTip = {
            message: "加载失败",
            visible: true
          };
        });

      this.table = createTable("Configuration");
      this.form = createTableContentObject(this.table);
    },
    switchTheme() {
      this.$vuetify.theme.dark = !this.$vuetify.theme.dark;
    }
  },
  created() {
    this.refresh();
  }
};
</script>
