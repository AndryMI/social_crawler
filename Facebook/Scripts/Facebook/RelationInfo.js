
var relations = []
var unique = {}

var source = (function () {
    const id = new URLSearchParams(document.location.search).get('id')
    const path = id ? document.location.pathname + '?id=' + id : document.location.pathname
    return new URL(path, 'https://www.facebook.com').href
})()

document.querySelectorAll('a').forEach(a => {
    if (unique[a.href]) {
        return;
    }
    unique[a.href] = true

    const query = new URLSearchParams(a.search)
    const type = query.get('pn_ref')
    if (type) {
        const id = query.get('id')
        const path = id ? a.pathname + '?id=' + id : a.pathname
        const target = new URL(path, 'https://www.facebook.com').href

        relations.push({
            Link: source,
            TargetLink: target,
            Type: type,
        })
    }
})

return JSON.stringify(relations)