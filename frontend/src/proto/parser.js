
import protoRoot from "./proto";

export function createTable(rootType) {
    let CheckEnum = typeName => {
        let isEnum = false;
        let values = [];
        try {
            let curt = protoRoot.lookupEnum(typeName);
            for (let item in curt.valuesById) {
                values.push(curt.valuesById[item]);
            }
            isEnum = true;
        } catch (err) {
            isEnum = false;
        }
        return { isEnum: isEnum, values: values };
    };

    let curtRoot = protoRoot.lookupType(rootType);
    let struct = curtRoot.toJSON().fields;
    let result = [];
    for (let item in struct) {
        try {
            result.push({
                alias: item,
                type: "object",
                child: createTable(struct[item].type)
            });
        } catch (err) {
            let { isEnum, values } = CheckEnum(struct[item].type);
            if (isEnum) {
                result.push({
                    alias: item,
                    type: "enum",
                    values: values
                });
            } else if (struct[item].type === "bool") {
                result.push({
                    alias: item,
                    type: "bool"
                });
            } else {
                result.push({
                    alias: item,
                    type: "input"
                });
            }
        }
    }
    return result;
}

export function createTableContentObject(table) {
    let result = {};
    for (let item in table) {
        let alias = table[item].alias;
        let type = table[item].type;
        if (type === "object") {
            result[alias] = createTableContentObject(table[item].child);
        } else if (type === "enum") {
            result[alias] = "";
        } else if (type === "input") {
            result[alias] = "";
        }
    }
    return result;
}