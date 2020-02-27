import {
    HubConnectionBuilder,
    LogLevel
} from "@microsoft/signalr";
import {
    getToken
} from "@/utils/auth";


export default {
    namespaced: true,
    state: {
        connection: null,
        currentLogId:null
    },
    getters: {
        connectionStatus: state => state.connection == null ? false : state.connection.connectionState == "Connected",
        connection: state => state.connection
    },
    mutations: {
        SET_CONNECTION: (state, connection) => {
            state.connection = connection
        }
    },
    actions: {
        async connect({
            state,
            commit
        }) {
            commit('SET_CONNECTION', new HubConnectionBuilder()
                .withUrl("/log-viewer", {
                    accessTokenFactory: () => getToken()
                })
                .withAutomaticReconnect()
                .configureLogging(LogLevel.Information)
                .build());
            await state.connection.start();
        }
    }

}