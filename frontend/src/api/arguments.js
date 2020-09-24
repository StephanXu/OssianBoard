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
            content: JSON.stringify(content)
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

export function deleteArguments(argId) {
    return request({
        url: `/argument/${argId}`,
        method: 'delete'
    })
}

export function createArgumentsSnapshot(argId, name) {
    return request({
        url: `/argument/${argId}/snapshot`,
        method: 'post',
        data: {
            name
        }
    })
}

export function listArgumentsSnapshot(argId) {
    return request({
        url: `/argument/${argId}/snapshot`,
        method: 'get'
    })
}

export function tagArgumentsSnapshot(snapshotId, tag) {
    return request({
        url: `/argument/snapshot/${snapshotId}/tag`,
        method: 'post',
        data: {
            tag
        }
    })
}

export function untagArgumentsSnapshot(snapshotId) {
    return request({
        url: `/argument/snapshot/${snapshotId}/tag`,
        method: 'delete'
    })
}

export function deleteArgumentsSnapshot(snapshotId) {
    return request({
        url: `/argument/snapshot/${snapshotId}`,
        method: 'delete'
    })
}

export function getSingleArgumentsSnapshot(snapshotId) {
    return request({
        url: `/argument/snapshot/${snapshotId}`,
        method: 'get'
    })
}