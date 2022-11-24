
var posts = [];

function ExtractProfileFromPostUrl(link) {
    const url = new URL(link ?? '', document.location.origin)
    return url.origin + url.pathname.match(/\/[^/]+/)
}
function ProcessStory(story) {
    const post = __FindFirstInObjectRecursive(story, (key, value) => value?.wwwURL ? value : undefined)
    if (!post) {
        return false
    }
    const time = __FindFirstInObjectRecursive(post.comet_sections, (key, value) => {
        return key == 'creation_time' && typeof value == 'number' ? value : undefined
    })
    const reactions = __FindFirstInObjectRecursive(story, (key, value) => {
        return key == 'reaction_count' ? value.count : undefined
    })
    const comments = __FindFirstInObjectRecursive(story, (key, value) => {
        return key == 'comment_count' ? value.total_count : undefined
    })
    const shares = __FindFirstInObjectRecursive(story, (key, value) => {
        return key == 'share_count' ? value.count : undefined
    })
    const profile = post.actors?.[0]?.url ?? ExtractProfileFromPostUrl(post.wwwURL)

    const links = {}
    const images = []
    const videos = []
    const attachments = post.attachments.map(x => x.styles.attachment)

    //TODO attached story
    //if (post.attached_story) {
    //    attachments.push(post.attached_story)
    //}
    const attached_story = __FindFirstInObjectRecursive(post.attached_story, (key, value) => {
        return typeof value?.creation_time == 'number' ? value.url : undefined
    })
    if (attached_story) {
        links[attached_story] = true
    }

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
        else if (key == 'url' && typeof value == 'string') {
            if (!value.startsWith(document.location.origin)) {
                links[value] = true
            }
        }
    })

    //TODO Check for CDN
    //for (const url in images) {
    //    fetch(url, { cache: 'force-cache' });
    //}

    posts.push({
        ProfileLink: profile,
        Link: post.wwwURL,

        FacebookPostId: post.id,

        UnixTime: time ?? 0,
        Text: post.message?.text,

        Images: images,
        Videos: videos,
        Links: Object.keys(links),

        Reactions: reactions ?? 0,
        Comments: comments ?? 0,
        Shares: shares ?? 0,
    })
    return true
}
__WalkObjectRecursive(__GetCurrentFacebookRequestsDump(), (key, value) => {
    if (value?.__isFeedUnit == 'Story') {
        return ProcessStory(value)
    }
})

return JSON.stringify(posts);
