export function timeToString(t) {
    t = new Date(t)
    return `${t.getFullYear()}-${t.getMonth().toString().padStart(2, "0")}-${t.getDay().toString().padStart(2, "0")} ${t.getHours().toString().padStart(2, "0")}:${t.getMinutes().toString().padStart(2, "0")}:${t.getSeconds().toString().padStart(2, "0")}`
}