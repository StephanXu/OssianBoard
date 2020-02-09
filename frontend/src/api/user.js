import request from '@/utils/request'

export function login(data) {
  return request({
    url: '/auth/sign-in',
    method: 'post',
    data
  })
}

export function getInfo() {
  return request({
    url: '/user/',
    method: 'get',
    params: {}
  })
}

export function logout() {
  return request({
    url: '/auth/sign-out',
    method: 'post'
  })
}

export function getUserList() {
  return request({
    url: '/user/list',
    method: 'get'
  })
}

export function updateUser(userName, userProfile) {
  return request({
    url: `/user/${userName}`,
    method: 'put',
    data: userProfile
  })
}

export function registerUser({ userName, password, alias }) {
  return request({
    url: '/user/register',
    method: 'post',
    data: {
      userName,
      password,
      alias
    }
  })
}

export function changePassword({ oldPassword, newPassword }) {
  return request({
    url: '/user/change-password',
    method: 'patch',
    data: {
      oldPassword,
      newPassword
    }
  })
}
