
var posts = [];

var queue = [];
queue.push(...(__FindProps(document.querySelector('article'), p => p?.topPosts)?.topPosts ?? []))
queue.push(...(__FindProps(document.querySelector('article'), p => p?.posts)?.posts ?? []))
queue.slice(-100).forEach(post => {

    const images = {};
    const videos = {};

    images[post.src ?? ''] = true;
    videos[post.videoUrl ?? ''] = true;

    post.sidecarChildren?.forEach(media => {
        images[media.src ?? ''] = true;
        videos[media.videoUrl ?? ''] = true;
    });

    delete images[''];
    delete videos[''];

    for (const url in images) {
        fetch(url);
    }

    posts.push({
        Link: (new URL('/p/' + post.code + '/', document.location.href)).href,
        ProfileLink: (new URL('/' + post.owner.username + '/', document.location.href)).href,

        Text: post.caption,
        Images: Object.keys(images),
        Videos: Object.keys(videos),

        Comments: post.numComments,
        Like: post.numLikes,

        UnixTime: post.postedAt,
    })
})

return JSON.stringify(posts);
