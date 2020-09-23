import Vue from 'vue'
import App from './App.vue'
import 'element-ui/lib/theme-chalk/index.css';
import vuetify from './plugins/vuetify';
import router from './router'
import store from './store'
import VueClipboard from "vue-clipboard2";

Vue.config.productionTip = false
Vue.use(VueClipboard)

new Vue({
    vuetify,
    router,
    store,
    render: h => h(App)
}).$mount('#app')
