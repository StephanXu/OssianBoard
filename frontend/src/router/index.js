import Vue from 'vue'
import VueRouter from 'vue-router'
import {
  getToken
} from '@/utils/auth' // get token from cookie
import Layout from '@/views/Layout'
import store from '@/store'

Vue.use(VueRouter)

const routes = [{
    path: '/',
    name: 'index',
    component: Layout,
    redirect: '/index',
    children: [{
      path: '/index',
      name: 'config',
      component: () => import('@/views/config'),
      meta: {
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
      component: () => import('@/views/OnlineLogger'),
      props: true,
      meta: {
        drawer: () => import('@/views/ClientChoose')
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
      component: () => import('@/views/Profile')
    }]
  },
  {
    path: '/about',
    name: 'about',
    component: () => import('../views/About.vue')
  },
  {
    path: '/login',
    name: 'login',
    component: () => import('../views/Login.vue')
  }
]

const router = new VueRouter({
  routes
})


router.beforeEach(async (to, from, next) => {
  // determine whether the user has logged in
  const hasToken = getToken()

  if (hasToken) {
    if (to.path === '/login') {
      // if is logged in, redirect to the home page
      next({
        path: '/'
      })
    } else {
      await store.dispatch('user/getInfo')
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