import request from '@/utils/request'

export function getArguments() {
  return request({
    url: '/argument',
    method: 'get'
  })
}

export function resetArguments() {
  return request({
    url: '/argument/reset',
    method: 'post'
  })
}

export function setArguments(data) {
  return request({
    url: '/argument',
    method: 'put',
    data: {
      arguments: JSON.stringify(data)
    }
  })
}
