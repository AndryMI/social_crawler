
window.__GetFiber = function (dom) {
    return dom ? dom[Object.keys(dom).find(key => key.startsWith("__reactFiber$"))] : null
}

window.__WalkFiberRecursive = function (fiber, fn) {
    return (fiber?.sibling ? fn(fiber.sibling) || __WalkFiberRecursive(fiber.sibling, fn) : null)
        || (fiber?.child ? fn(fiber.child) || __WalkFiberRecursive(fiber.child, fn) : null)
}

window.__FindClosestFiber = function (fiber, fn) {
    while (fiber != null && !fn(fiber)) {
        fiber = fiber.return
    }
    return fiber
}

window.__FindProps = function (dom, fn) {
    for (let fiber = __GetFiber(dom); fiber != null; fiber = fiber.return) {
        if (fn(fiber.pendingProps)) {
            return fiber.pendingProps;
        }
    }
    return null;
}
