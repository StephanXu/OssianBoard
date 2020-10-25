import {
    setToken,
    removeToken
} from '@/utils/auth'
import {
    login,
    getInfo,
    logout
} from '@/api/user'
import store from "@/store";


export default {
    namespaced: true,
    state: {
        token: '',
        name: '',
        alias: '',
        roles: []
    },
    getters: {
        token: state => state.token,
        name: state => state.name,
        alias: state => state.alias,
        roles: state => state.roles
    },
    mutations: {
        SET_TOKEN: (state, token) => {
            setToken(token)
            state.token = token
        },
        SET_NAME: (state, name) => {
            state.name = name
        },
        SET_ALIAS: (state, alias) => {
            state.alias = alias
        },
        SET_ROLES: (state, roles) => {
            state.roles = roles
        }
    },
    actions: {
        login({
                  commit
              }, userInfo) {
            const {
                username,
                password
            } = userInfo
            return new Promise((resolve, reject) => {
                login({
                    username: username.trim(),
                    password: password
                }).then(response => {
                    const {
                        token,
                        alias,
                        userName
                    } = response
                    commit('SET_TOKEN', token)
                    commit('SET_ALIAS', alias)
                    commit('SET_NAME', userName)
                    resolve()
                }).catch((err) => {
                    reject(err)
                })
            })
        },
        // get user info
        getInfo({
                    commit
                }) {
            return new Promise((resolve, reject) => {
                getInfo().then(response => {
                    const data = response
                    if (!data) {
                        reject('Verification failed, please Login again.')
                    }
                    const {
                        roles,
                        alias,
                        userName
                    } = data
                    // roles must be a non-empty array
                    if (!roles || roles.length <= 0) {
                        reject('getInfo: roles must be a non-null array!')
                    }
                    commit('SET_ROLES', roles)
                    commit('SET_NAME', userName)
                    commit('SET_ALIAS', alias)
                    resolve(data)
                }).catch(error => {
                    // to re-login
                    store.dispatch('user/resetToken').then(() => {
                        location.reload()
                    })
                    reject(error)
                })
            })
        },

        // user logout
        logout({
                   commit,
                   state
               }) {
            return new Promise((resolve, reject) => {
                logout(state.token).then(() => {
                    commit('SET_TOKEN', '')
                    commit('SET_NAME', '')
                    commit('SET_ROLES', [])
                    removeToken()
                    resolve()
                }).catch(error => {
                    reject(error)
                })
            })
        },

        async resetToken({commit}) {
            commit('SET_TOKEN', '')
            commit('SET_NAME', '')
            commit('SET_ROLES', [])
            removeToken()
        }
    }
}