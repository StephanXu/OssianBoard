import request from '@/utils/request'

export function listArguments(){
  return request({
    url: '/argument',
    method: 'get'
  })
}

export function getSingleArguments(argId) {
  return request({
    url: `/argument/${argId}`,
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
