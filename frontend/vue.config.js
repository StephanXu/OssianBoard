module.exports = {
  "transpileDependencies": [
    "vuetify"
  ],
  devServer: {
    proxy: {
      '^/log-viewer': {
        target: 'http://localhost:5000',
        ws: true,
        changeOrigin: true
      },
      '^/api': {
        target: 'http://localhost:5000'
      }
    }
  }
}
