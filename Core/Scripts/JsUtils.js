
window.__WalkObjectRecursive = function (obj, fn) {
    const queue = Object.entries(obj ?? {})
    while (queue.length > 0) {
        const item = queue.shift()
        if (!fn(item[0], item[1]) && item[1] && typeof item[1] == 'object') {
            queue.push(...Object.entries(item[1]))
        }
    }
}

window.__FindFirstInObjectRecursive = function (obj, fn) {
    const queue = Object.entries(obj ?? {})
    while (queue.length > 0) {
        const item = queue.shift()
        const result = fn(item[0], item[1])
        if (result !== undefined) {
            return result
        }
        if (item[1] && typeof item[1] == 'object') {
            queue.push(...Object.entries(item[1]))
        }
    }
}
