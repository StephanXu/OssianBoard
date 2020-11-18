import {
    listArguments
} from "@/api/arguments";

export default {
    namespaced: true,
    state: {
        argumentMetaList: []
    },
    getters: {
        argumentMetaList: state => state.argumentMetaList
    },
    mutations: {
        SET_ARGUMENT_META_LIST: (state, value) => {
            state.argumentMetaList = value
        }
    },
    actions: {
        async fetchArgumentList({commit}) {
            commit('SET_ARGUMENT_META_LIST', await listArguments())
        }
    }
}