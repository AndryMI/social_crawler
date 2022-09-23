
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
