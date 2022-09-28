
var comments = []

function ExtractProfileFromPostUrl(link) {
    const url = new URL(link ?? '', document.location.origin)
    return url.origin + url.pathname.match(/\/[^/]+/)
}
function ProcessComment(comment) {
    const comment_link = __FindFirstInObjectRecursive(comment, (key, value) => {
        return key == 'comment' && typeof value.url == 'string' ? value.url : undefined
    })
    if (!comment_link) {
        return
    }
    const reactions = __FindFirstInObjectRecursive(comment, (key, value) => {
        return key == 'reactors' ? value.count : undefined
    })
    const attachments = comment.attachments?.map(x => x.style_type_renderer.attachment)
    const images = []
    const videos = []
    __WalkObjectRecursive(attachments, (key, value) => {
        if (key == 'media' && value) {
            const image = value.photo_image?.uri
                || value.large_share_image?.uri
                || value.image?.uri
            if (image) {
                images.push(image)
            }

            const video = value.playable_url
            if (video) {
                videos.push(video)
            }
            return true
        }
    })

    comments.push({
        ProfileLink: ExtractProfileFromPostUrl(document.location.href),
        PostLink: document.location.href,
        Link: comment_link,

        AuthorLink: comment.author?.url,
        Text: comment.body?.text,

        UnixTime: comment.created_time ?? 0,
        Reactions: reactions ?? 0,

        Images: images,
        Videos: videos,
    })
}
__WalkObjectRecursive(__GetCurrentFacebookRequestsDump(), (key, value) => {
    if (value?.__typename == 'Comment') {
        ProcessComment(value)
    }
})

return JSON.stringify(comments);