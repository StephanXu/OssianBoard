import request from '@/utils/request'

export function listArguments() {
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

export function updateSingleArguments({id, content}) {
    return request({
        url: `/argument/${id}`,
        method: 'put',
        data: {
            content
        }
    })
}

export function createArguments({name, schema, content}) {
    return request({
        url: '/argument',
        method: 'post',
        data: {
            name,
            schema: JSON.stringify(schema),
            content: JSON.stringify(content)
        }
    })
}