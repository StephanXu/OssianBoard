import {
    HubConnectionBuilder,
    LogLevel
} from "@microsoft/signalr";
import {
    getToken
} from "@/utils/auth";
import {
    getLogList
} from '@/api/log'


export default {
    namespaced: true,
    state: {
        connection: new HubConnectionBuilder()
            .withUrl("/log-viewer", {
                accessTokenFactory: () => getToken()
            })
            .withAutomaticReconnect()
            .configureLogging(LogLevel.Information)
            .build(),
        logList: []
    },
    getters: {
        connectionStatus: state => {
            return state.connection == null ? false : state.connection.connectionState === "Connected"
        },
        connection: state => state.connection,
        logList: state => state.logList
    },
    mutations: {
        SET_CONNECTION: (state, connection) => {
            state.connection = connection
        },
        SET_LOG_LIST: (state, logList) => {
            state.logList = logList
                .sort(
                    (lhs, rhs) =>
                    Date.parse(rhs.createTime) - Date.parse(lhs.createTime)
                )
                .map((item) => {
                    return {
                        ...item,
                        createTime: new Date(item.createTime).toLocaleString(),
                    };
                });
        }
    },
    actions: {
        async connect({
            state,
            commit
        }) {
            state.connection.on("RefreshLogsList", (logList) => {
                commit('SET_LOG_LIST', logList);
            });
            await state.connection.start();
        },

        async refreshLogList({
            state,
            commit
        }) {
            commit('SET_LOG_LIST', await getLogList())
            return state.logList;
        }
    }

}