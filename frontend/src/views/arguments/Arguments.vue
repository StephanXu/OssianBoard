<template>
  <v-container fluid>
    <v-snackbar v-model="successTip.visible" top color="success">
      {{ successTip.message }}
      <v-btn text @click="successTip.visible = false">OK</v-btn>
    </v-snackbar>

    <v-snackbar v-model="errorTip.visible" top color="error">
      {{ errorTip.message }}
      <v-btn text @click="errorTip.visible = false">OK</v-btn>
    </v-snackbar>

    <v-dialog v-model="customConfigDialog.visible" persistent max-width="850px">
      <v-card>
        <v-card-title>
          <span class="headline">Custom Configuration</span>
        </v-card-title>
        <v-card-text>
          <v-container>
            <v-row>
              <v-col cols="12">
                <v-textarea
                  filled
                  label="Configuration"
                  auto-grow
                  v-model="customConfigDialog.content"
                ></v-textarea>
              </v-col>
            </v-row>
          </v-container>
          <small>Only support configuration in JSON.</small>
        </v-card-text>
        <v-card-actions>
          <v-spacer></v-spacer>
          <v-btn
            color="blue darken-1"
            text
            @click="customConfigDialog.visible = false"
            >Cancel</v-btn
          >
          <v-btn color="blue darken-1" text @click="saveCustomConfiguration"
            >Save</v-btn
          >
        </v-card-actions>
      </v-card>
    </v-dialog>

    <v-row align="center" justify="center">
      <v-col align-self="start" :cols="12 / colum.length" v-for="col in colum" :key="col">
        <v-card
          v-for="cate in columTable(col, colum.length)"
          :key="cate.alias"
          style="padding:20px;margin-bottom:20px;max-width: 850px"
          class="mx-auto"
          tle
        >
          <v-card-title>{{ cate.alias }}</v-card-title>
          <div v-for="item in cate.child" :key="item.alias">
            <v-text-field
              v-if="item.type === 'input'"
              :label="item.alias"
              v-model="form[cate.alias][item.alias]"
            />
            <v-switch
              v-else-if="item.type === 'bool'"
              v-model="form[cate.alias][item.alias]"
              :label="item.alias"
            ></v-switch>
            <v-select
              v-else-if="item.type === 'enum'"
              v-model="form[cate.alias][item.alias]"
              :items="item.values"
              :label="item.alias"
            ></v-select>
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
        <v-btn v-model="fab" color="blue darken-2" dark fab>
          <v-icon v-if="fab">mdi-close</v-icon>
          <v-icon v-else>mdi-account-circle</v-icon>
        </v-btn>
      </template>
      <v-btn color="primary" dark fab small>
        <v-icon @click="customConfigDialog.visible = true">cloud_upload</v-icon>
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
import { getArguments, setArguments } from "@/api/arguments";
import protoRoot from "@/proto/proto";
import { createTable, createTableContentObject } from "@/proto/parser";

export default {
  props: {
    source: String,
  },
  computed: {
    columTable() {
      return (columIndex, columCount) => {
        let res = [];
        this.table.forEach((item, index) => {
          if (index % columCount == columIndex) {
            res.push(item);
          }
        });
        return res;
      };
    },
  },
  data: () => ({
    drawer: null,
    fab: "",
    form: {},
    table: [],
    successTip: {
      message: "",
      visible: false,
    },
    errorTip: {
      message: "",
      visible: false,
    },
    customConfigDialog: {
      visible: false,
      content: "",
    },
    colum: [0, 1, 2, 3],
  }),
  methods: {
    onSubmit() {
      let configType = protoRoot.lookupType("Configuration");
      let config = configType.fromObject(this.form);
      setArguments(config.toJSON())
        .then(() => {
          this.successTip = {
            message: "Save success",
            visible: true,
          };
        })
        .catch(() => {
          this.errorTip = {
            message: "Save failed",
            visible: true,
          };
        });
    },
    refresh() {
      getArguments()
        .then((response) => {
          let configType = protoRoot.lookupType("Configuration");
          let verifyResult = configType.verify(configType.fromObject(response));
          if (verifyResult) throw verifyResult;
          this.form = response;
        })
        .catch(() => {
          this.errorTip = {
            message: "Load failed",
            visible: true,
          };
          this.table = createTable("Configuration");
          this.form = createTableContentObject(this.table);
        });

      this.table = createTable("Configuration");
      this.form = createTableContentObject(this.table);
    },
    switchTheme() {
      this.$vuetify.theme.dark = !this.$vuetify.theme.dark;
    },
    saveCustomConfiguration() {
      try {
        let configType = protoRoot.lookupType("Configuration");
        let config = configType.fromObject(
          JSON.parse(this.customConfigDialog.content)
        );
        let verifyResult = configType.verify(configType.fromObject(config));
        if (verifyResult) throw verifyResult;
        setArguments(config.toJSON())
          .then(() => {
            this.successTip = {
              message: "Upload success",
              visible: true,
            };
            this.refresh();
          })
          .catch(() => {
            this.errorTip = {
              message: "Upload failed",
              visible: true,
            };
          });
        this.customConfigDialog.visible = false;
      } catch (err) {
        this.errorTip = {
          message: `Import failed: ${err}`,
          visible: true,
        };
      }
    },
    test(num) {
      return num % 3 == 0;
    },
  },
  created() {
    this.$nextTick(() => {
      this.refresh();
    });
  },
};
</script>
