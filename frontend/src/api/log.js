import request from '@/utils/request'

export function getLogList() {
    return request({
        url: '/log',
        method: 'get'
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
        params: {
            'page': page,
            'items-per-page': itemsPerPage
        }
    })
}