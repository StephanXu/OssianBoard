import Vue from 'vue'
import VueRouter from 'vue-router'
import {
    getToken
} from '@/utils/auth' // get token from cookie
import Layout from '@/views/layout/Layout'
import store from '@/store'

Vue.use(VueRouter)

const routes = [
    {
        path: '/',
        name: 'index',
        component: Layout,
        redirect: '/argument'
    },
    {
        path: '/argument',
        name: 'argument',
        component: Layout,
        redirect: '/argument/index',
        children: [
            {
                path: 'index',
                name: 'argument',
                component: () => import('@/views/arguments/ArgumentList'),
                meta: {
                    drawer: () => import('@/views/layout/DefaultNavigator'),
                    title: 'Arguments'
                }
            },
            {
                path: ':argId',
                name: 'singleArgument',
                component: () => import('@/views/arguments/Argument'),
                props: true,
                meta: {
                    drawer: () => import('@/views/layout/DefaultNavigator'),
                    title: 'Argument'
                }
            },
            {
                path: ':argId/snapshot/:snapshotId',
                name: 'singleSnapshot',
                component: () => import('@/views/arguments/Argument'),
                props: true,
                meta: {
                    drawer: () => import('@/views/layout/DefaultNavigator'),
                    title: 'Argument Snapshot'
                }
            }]
    },
    {
        path: '/board',
        name: 'board',
        component: Layout,
        redirect: '/board/index',
        children: [{
            path: ':logId',
            name: 'board',
            component: () => import('@/views/onlineLogger/OnlineLogger'),
            props: true,
            meta: {
                drawer: () => import('@/views/layout/DefaultNavigator'),
                title: 'Online Logger'
            }
        }]
    },
    {
        path: '/profile',
        name: 'profile',
        component: Layout,
        redirect: '/profile/index',
        children: [{
            path: 'index',
            name: 'profile',
            component: () => import('@/views/profile/Profile'),
            meta: {
                drawer: () => import('@/views/layout/DefaultNavigator'),
                title: 'Profile'
            }
        }]
    },
    {
        path: '/login',
        name: 'login',
        component: () => import('../views/login/Login.vue'),
        meta: {
            title: 'Login'
        }
    }
]

const router = new VueRouter({
    mode: 'history',
    routes
})


router.beforeEach(async (to, from, next) => {
    // determine whether the user has logged in
    const hasToken = getToken()

    document.title = `${to.meta.title} - Ossian Board`

    if (hasToken) {
        if (to.path === '/login') {
            // if is logged in, redirect to the home page
            next({
                path: '/'
            })
        } else {
            let hasRole = store.getters['user/roles'] && store.getters['user/roles'].length > 0
            if (!hasRole) {
                await store.dispatch('user/getInfo')
            }
            next()
        }
    } else {
        if (to.path === '/login') {
            next()
        } else {
            next(`/login`)
        }
    }
})

export default router