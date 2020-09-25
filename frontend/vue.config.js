module.exports = {
    "transpileDependencies": [
        "vuetify",
        'vue-echarts',
        'resize-detector'
    ],
    devServer: {
        host: '0.0.0.0',
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