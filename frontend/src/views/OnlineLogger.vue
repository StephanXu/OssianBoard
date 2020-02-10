<template>
  <v-container fluid>
    <v-row align="center" justify="center">
      <v-col cols="12">
        <v-card
          v-for="item in onlineVariants"
          :key="item.varName"
          class="mx-auto"
          style="margin-bottom: 20px;max-width: 850px"
          color="grey lighten-4"
          tle
        >
          <v-card-title>
            <v-icon color="indigo" class="mr-12" size="64">
              mdi-record-circle
            </v-icon>
            <v-row align="start">
              <div class="caption grey--text text-uppercase">
                {{item.varName}}
              </div>
              <div>
                <span class="display-2 font-weight-black black--text" v-text="item.dots[item.dots.length-1] || '—'"></span>
                <strong class="black--text">Currently</strong>
              </div>
            </v-row>
          </v-card-title>
          <!--          <v-card-title>{{item.varName}}</v-card-title>-->
          <plot style="height: 500px;width: 100%" :dots="item.dots" :times="item.times"></plot>
        </v-card>
      </v-col>
    </v-row>

    <v-btn :color="connected?'pink':'green'" dark fixed bottom right fab>
      <v-icon v-if="connected" @click="disconnect">mdi-logout</v-icon>
      <v-icon v-else @click="connect">mdi-login</v-icon>
    </v-btn>
  </v-container>
</template>

<script>
import {HubConnectionBuilder, LogLevel} from '@microsoft/signalr'
import Plot from '@/components/Plot'

export default {
  components: {
    Plot
  },
  data() {
    return {
      fab: false,
      connection: null,
      onlineVariants: [
      ]
    }
  },
  computed: {
    connected() {
      try {
        return this.connection.connectionState === 'Connected'
      } catch (e) {
        return false
      }
    }
  },
  methods: {
    async connect() {
      this.connection = new HubConnectionBuilder()
        .withUrl('/logger')
        .configureLogging(LogLevel.Critical)
        .build();
      this.connection.on('ReceiveLog', (clientId, varName, value, times) => {
        let varObj = this.onlineVariants.find(item => item.varName === varName)
        if (!varObj) {
          this.onlineVariants.push({
            varName: varName,
            dots: value,
            times: times
          })
          return
        }
        varObj.dots = varObj.dots.concat(value)
        varObj.times = varObj.times.concat(times)
        if (varObj.dots.length > 100) {
          varObj.dots.splice(0, varObj.dots.length - 100)
          varObj.times.splice(0, varObj.times.length - 100)
        }
        // console.log('Client: ' + clientId + '  varName: ' + varName + '  value: ' + value + times)
      })
      await this.connection.start()
      await this.connection.invoke('AddToViewer')
    },
    async disconnect() {
      await this.connection.stop()
    }
  }
}
</script>
