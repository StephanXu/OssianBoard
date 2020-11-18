import lodash from 'lodash'

export function timeToString(t) {
    t = new Date(t)
    return `${t.getFullYear()}-${t.getMonth().toString().padStart(2, "0")}-${t.getDay().toString().padStart(2, "0")} ${t.getHours().toString().padStart(2, "0")}:${t.getMinutes().toString().padStart(2, "0")}:${t.getSeconds().toString().padStart(2, "0")}`
}

export function initSchemaContent(schema, curtContent) {
    for (const prop in schema.properties) {
        if (schema.properties[prop].type === 'object') {
            curtContent[prop] = curtContent.hasOwnProperty(prop) ? curtContent[prop] : {}
            initSchemaContent(schema.properties[prop], curtContent[prop])
        } else if (schema.properties[prop].type === 'number') {
            curtContent[prop] = curtContent.hasOwnProperty(prop) ? curtContent[prop] : 0
        } else {
            curtContent[prop] = curtContent.hasOwnProperty(prop) ? curtContent[prop] : ''
        }
    }
}

function joinSchemaContentHelper(schema, curtContent, importContent, count) {
    for (const prop in schema.properties) {
        let defaultValue = null
        if (schema.properties[prop].type === 'object') {
            curtContent[prop] = curtContent.hasOwnProperty(prop) ? curtContent[prop] : {}
            importContent[prop] = importContent.hasOwnProperty(prop) ? importContent[prop] : {}
            joinSchemaContentHelper(schema.properties[prop], curtContent[prop], importContent[prop], count)
            continue
        } else if (schema.properties[prop].type === 'number') {
            defaultValue = 0
        } else {
            defaultValue = ''
        }
        if (importContent.hasOwnProperty(prop)) {
            if (curtContent.hasOwnProperty(prop)) {
                if (curtContent[prop] != importContent[prop]) {
                    count.modifyCount += 1
                } else {
                    count.defaultCount += 1
                }
            } else {
                count.addedCount += 1
            }
            curtContent[prop] = importContent[prop]
        } else if (!curtContent.hasOwnProperty(prop)) {
            curtContent[prop] = defaultValue
        } else {
            count.defaultCount += 1
        }
    }
}

export function joinSchemaContent(schema, curtContent, importContent) {
    let content = lodash.cloneDeep(curtContent)
    let count = {
        modifyCount: 0,
        addedCount: 0,
        defaultCount: 0
    }
    joinSchemaContentHelper(schema, content, lodash.cloneDeep(importContent), count)
    return {content, ...count}
}