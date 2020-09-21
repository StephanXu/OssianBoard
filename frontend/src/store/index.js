import Vue from 'vue'
import Vuex from 'vuex'
import user from '@/store/modules/user'
import log from '@/store/modules/log'
import argument from "@/store/modules/arguments"

Vue.use(Vuex)

export default new Vuex.Store({
  modules: {
    user,
    log,
    argument
  }
})