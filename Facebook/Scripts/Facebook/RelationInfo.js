
var anchors =
    (document.location.host == 'm.facebook.com') ?
        Array.from(document.querySelectorAll('h1 a, h3 a')) :
        Array.from(document.querySelectorAll('a')).filter(a => {
            return __FindClosestFiber(__GetFiber(a), fb => {
                return fb.pendingProps?.timelineAppCollectionItem
                    && fb.elementType?.displayName?.indexOf('ListItem') >= 0
            })
        })

var relations = []

function NormalizeUrl(link) {
    const url = new URL(link, 'https://www.facebook.com');
    const id = url.searchParams.get('id')
    const path = id ? url.pathname + '?id=' + id : url.pathname
    return new URL(path, 'https://www.facebook.com').href
}

var source = NormalizeUrl(document.location.href)

var infos = {}
anchors.forEach(a => {
    const target = NormalizeUrl(a.href)
    if (!infos[target]) {
        infos[target] = {}
    }
    const url = new URL(a.href, 'https://www.facebook.com')
    const type = url.searchParams.get('pn_ref')
    infos[target].type = type || infos[target].type
    infos[target].name = a.innerText.trim() || infos[target].name
})
Object.entries(infos).forEach(info => {
    relations.push({
        Link: source,
        TargetLink: info[0],
        Type: info[1].type,
        Name: info[1].name,
    })
})

return JSON.stringify(relations)