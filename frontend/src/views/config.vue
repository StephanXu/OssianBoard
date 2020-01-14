<template>
  <v-app id="inspire">
    <v-navigation-drawer v-model="drawer" app clipped>
      <v-list dense>
        <v-list-item link>
          <v-list-item-action>
            <v-icon>mdi-view-dashboard</v-icon>
          </v-list-item-action>
          <v-list-item-content>
            <v-list-item-title>Arguments</v-list-item-title>
          </v-list-item-content>
        </v-list-item>
        <v-list-item link>
          <v-list-item-action>
            <v-icon>mdi-settings</v-icon>
          </v-list-item-action>
          <v-list-item-content>
            <v-list-item-title>Settings</v-list-item-title>
          </v-list-item-content>
        </v-list-item>
      </v-list>
    </v-navigation-drawer>

    <v-app-bar app clipped-left>
      <v-app-bar-nav-icon @click.stop="drawer = !drawer" />
      <v-toolbar-title>Nautilus Config Generator</v-toolbar-title>
      <v-spacer></v-spacer>
      <v-btn color="error" icon>
        <v-icon @click="onReset">restore</v-icon>
      </v-btn>
      <v-btn icon>
        <v-icon @click="switchTheme">invert_colors</v-icon>
      </v-btn>
      <v-btn icon>
        <v-icon @click="refresh">refresh</v-icon>
      </v-btn>
      <v-btn icon>
        <v-icon @click="onSubmit">save</v-icon>
      </v-btn>
    </v-app-bar>

    <v-snackbar v-model="successTip.visible" top color="success">
      {{ successTip.message }}
      <v-btn text @click="successTip.visible = false">好</v-btn>
    </v-snackbar>

    <v-snackbar v-model="errorTip.visible" top color="error">
      {{ errorTip.message }}
      <v-btn text @click="errorTip.visible = false">好</v-btn>
    </v-snackbar>

    <v-content>
      <v-container class="fill-height" fluid>
        <v-row align="center" justify="center">
          <v-col cols="12">
            <v-card
              v-for="cate in table"
              :key="cate.alias"
              style="padding:20px;margin:20px"
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
                <v-select v-else-if="item.type==='enum'" v-model="form[cate.alias][item.alias]" :items="item.values" :label="item.alias"></v-select>
              </div>
            </v-card>
          </v-col>
        </v-row>
      </v-container>
    </v-content>

    <v-footer app>
      <span>&copy; 2020</span>
    </v-footer>
  </v-app>
</template>

<script>
import { get, put, post } from "../http";
import protoRoot from "../proto/proto";
import { createTable, createTableContentObject } from "../proto/parser";
export default {
  props: {
    source: String
  },
  data: () => ({
    drawer: null,
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
      put("/config", config.toJSON())
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
      post("/config/reset")
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
      get("/config")
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