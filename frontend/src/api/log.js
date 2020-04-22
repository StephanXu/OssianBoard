import request from '@/utils/request'

export function getLogList() {
    return request({
        url: '/log',
        method: 'get'
    })
}

export function updateLog(logId, {
    name,
    description
}) {
    return request({
        url: `/log/${logId}`,
        method: 'put',
        data: {
            name,
            description
        }
    })
}

export function removeLog(logId) {
    return request({
        url: `log/${logId}`,
        method: 'delete'
    })
}

export function getPlot(logId) {
    return request({
        url: `log/${logId}/plot`,
        method: 'get'
    })
}

export function getLog(logId, page, itemsPerPage) {
    return request({
        url: `log/${logId}`,
        method: 'get',
        params: {
            'page': page,
            'items-per-page': itemsPerPage
        }
    })
}

export function getLogByTime(logId, time, itemsPerPage) {
    return request({
        url: `log/${logId}/time-navigation`,
        method: 'get',
        params: {
            'time': time,
            'items-per-page': itemsPerPage
        },
    })
}