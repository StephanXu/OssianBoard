<template>
  <v-chart :options="option" />
</template>

<script>
import ECharts from 'vue-echarts'
import 'echarts/lib/chart/line'
import 'echarts/lib/component/polar'

export default {
  props: {
    dots: {
      type: Array,
      default: () => []
    },
    times: {
      type: Array,
      default: () => []
    }
  },
  components: {
    'v-chart': ECharts
  },
  computed: {
    option() {
      return {
        xAxis: {
          type: 'category',
          data: this.times
        },
        yAxis: {
          type: 'value'
        },
        series: [{
          data: this.dots,
          type: 'line'
        }]
      }
    }
  },
  data() {
    return {
      polar: {
        xAxis: {
          type: 'category',
          data: this.times
        },
        yAxis: {
          type: 'value'
        },
        series: [{
          data: this.dots,
          type: 'line',
          areaStyle: {}
        }]
      }
    }
  }
}
</script>

<style>
.echarts {
  height: 100%;
  width: 100%;
}
</style>
