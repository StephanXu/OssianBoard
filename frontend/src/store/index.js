import Vue from 'vue'
import Vuex from 'vuex'
import user from './modules/user'
import log from './modules/log'

Vue.use(Vuex)

export default new Vuex.Store({
  modules: {
    user,
    log
  }
})