
var posts = [];

__FindProps(document.querySelector('article'), p => p.posts)?.posts?.slice(-100)?.forEach(post => {

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
