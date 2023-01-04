
var relations = []

var source = (function () {
    const id = new URLSearchParams(document.location.search).get('id')
    const path = id ? document.location.pathname + '?id=' + id : document.location.pathname
    return new URL(path, 'https://www.facebook.com').href
})()

var infos = {}
document.querySelectorAll('a').forEach(a => {
    if (a.href.indexOf('pn_ref=') > 0) {
        infos[a.href] = a.innerText.trim() || infos[a.href]
    }
})
Object.entries(infos).forEach(info => {
    const url = new URL(info[0])
    const type = url.searchParams.get('pn_ref')
    if (type) {
        const id = url.searchParams.get('id')
        const path = id ? url.pathname + '?id=' + id : url.pathname
        const target = new URL(path, 'https://www.facebook.com').href

        relations.push({
            Link: source,
            TargetLink: target,
            Type: type,
            Name: info[1],
        })
    }
})

return JSON.stringify(relations)