import request from '@/utils/request'

export function getArguments() {
  return request({
    url: '/argument',
    method: 'get'
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
