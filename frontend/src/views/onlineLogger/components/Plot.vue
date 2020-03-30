<template>
  <v-chart autoresize :options="option" />
</template>

<script>
import ECharts from "vue-echarts";
import "echarts/lib/chart/line";
import "echarts/lib/component/polar";
import "echarts/lib/component/dataZoom";
import "echarts/lib/component/tooltip";
import "echarts/lib/component/toolbox";
// import echarts from "echarts/lib/echarts";

export default {
  props: {
    values: {
      type: Array,
      default: () => []
    },
    times: {
      type: Array,
      default: () => []
    }
  },
  components: {
    "v-chart": ECharts
  },
  computed: {
    stringTimes() {
      return this.times.map(
        item =>
          `${item.getHours()}:${item.getMinutes()}:${item.getSeconds()}.${item.getMilliseconds()}`
      );
    },
    option() {
      return {
        tooltip: {
          trigger: "axis",
          axisPointer: {
            type: "cross"
          }
        },
        toolbox: {
          feature: {
            dataView: { show: true, readOnly: false },
            restore: { show: true },
            saveAsImage: { show: true }
          }
        },
        xAxis: {
          type: "category",
          data: this.stringTimes
        },
        yAxis: {
          type: "value"
        },
        grid: {},
        dataZoom: [
          {
            type: "inside",
            startValue:
              this.stringTimes.length >= 100
                ? this.stringTimes[this.stringTimes.length - 100]
                : this.stringTimes[0],
            endValue: this.stringTimes[this.stringTimes.length - 1]
          },
          {
            startValue:
              this.stringTimes.length >= 100
                ? this.stringTimes[this.stringTimes.length - 100]
                : this.stringTimes[0],
            endValue: this.stringTimes[this.stringTimes.length - 1],
            handleIcon:
              "M10.7,11.9v-1.3H9.3v1.3c-4.9,0.3-8.8,4.4-8.8,9.4c0,5,3.9,9.1,8.8,9.4v1.3h1.3v-1.3c4.9-0.3,8.8-4.4,8.8-9.4C19.5,16.3,15.6,12.2,10.7,11.9z M13.3,24.4H6.7V23h6.6V24.4z M13.3,19.6H6.7v-1.4h6.6V19.6z",
            handleSize: "80%",
            handleStyle: {
              color: "#fff",
              shadowBlur: 3,
              shadowColor: "rgba(0, 0, 0, 0.6)",
              shadowOffsetX: 2,
              shadowOffsetY: 2
            }
          }
        ],
        series: [
          ...this.values.map(item => {
            return {
              name: item.varName,
              data: item.values,
              type: "line"
            };
          })
        ]
      };
    }
  },
  data() {
    return {};
  }
};
</script>

<style>
.echarts {
  height: 100%;
  width: 100%;
}
</style>
