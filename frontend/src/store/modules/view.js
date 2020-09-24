export default {
    namespaced: true,
    state: {
        clientWidth: 0,
        clientHeight: 0,
    },
    getters: {
        clientWidth: state => state.clientWidth,
        clientHeight: state => state.clientHeight
    },
    mutations: {
        SET_CLIENT_WIDTH: (state, value) => {
            state.clientWidth = value
        },
        SET_CLIENT_HEIGHT: (state, value) => {
            state.clientHeight = value
        }
    },
    actions: {}
}