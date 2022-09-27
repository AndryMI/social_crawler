
window.__FindFacebookViewer = function () {
    let result = ''
    const scripts = document.querySelectorAll('[data-sjs]')
    for (let i = 0; i < scripts.length; i++) {
        const data = JSON.parse(scripts[i].innerHTML)
        __WalkObjectRecursive(data, (key, value) => {
            if (result) {
                return true
            }
            if (key == 'result' && value?.data?.viewer?.actor?.name) {
                result = value.data.viewer.actor.name
                return true
            }
        })
    }
    return result
}

window.__DumpPrefetchedFacebookRequests = function () {
    document.querySelectorAll('[data-sjs]').forEach(script => {
        __WalkObjectRecursive(JSON.parse(script.text), (key, value) => {
            if (key == 'result' && value && typeof value == 'object') {
                const scr = document.createElement('script')
                scr.setAttribute('data-dump', 'prefetch')
                scr.type = 'text/plain'
                scr.text = JSON.stringify(value)
                document.body.appendChild(scr)
                return true
            }
        })
    })
}

window.__GetCurrentFacebookRequestsDump = function () {
    const texts = []
    document.querySelectorAll('[data-dump]').forEach(dump => texts.push(...dump.text.split('\n').filter(x => x)))
    return texts.map(JSON.parse)
}
