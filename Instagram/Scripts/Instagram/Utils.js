
window.__CollectSearchResultLinks = function () {
    const links = [];

    const results = __WalkFiberRecursive(__GetFiber(document.querySelector('body>div>div')), (fb) => {
        return fb.pendingProps?.results
    }) ?? [];

    results.forEach(item => {
        switch (item.type) {
            case 'USER_RESULT':
                links.push((new URL('/' + item.username + '/', document.location.href)).href);
                break;

            case 'HASHTAG_RESULT':
                links.push((new URL('/explore/tags/' + item.name + '/', document.location.href)).href);
                break;

            case 'PLACE_RESULT':
                links.push((new URL('/explore/locations/' + item.locationId + '/' + item.slug + '/', document.location.href)).href);
                break;

            default:
                console.warn('unknown', item)
                break;
        }
    });

    return JSON.stringify(links);
}
