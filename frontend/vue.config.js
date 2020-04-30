module.exports = {
  "transpileDependencies": [
    "vuetify",
    'vue-echarts',
    'resize-detector'
  ],
  devServer: {
    proxy: {
      '^/log-viewer': {
        target: 'https://localhost:5000',
        ws: true,
        changeOrigin: true
      },
      '^/api': {
        target: 'https://localhost:5000'
      }
    }
  }
}