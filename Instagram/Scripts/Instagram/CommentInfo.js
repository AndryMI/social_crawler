
var article = __FindProps(document.querySelector('article'), p => p.post);
if (!article) {
    return '[]';
}

var comments = []

var comment_fibers = {};
__WalkFiberRecursive(__GetFiber(document.querySelector('article')), cf => {
    if (cf.pendingProps.parentComment) {
        comment_fibers[cf.pendingProps.parentComment.id] = cf;
    }
});
Object.values(comment_fibers).forEach(cf => {
    const comment = cf.pendingProps.parentComment;

    __WalkFiberRecursive(cf, uf => {
        if (uf.pendingProps.username) {
            comments.push({
                Link: (new URL('/p/' + article.post.code + '/c/' + comment.id + '/', document.location.href)).href,
                PostLink: (new URL('/p/' + article.post.code + '/', document.location.href)).href,
                ProfileLink: (new URL('/' + article.post.owner.username + '/', document.location.href)).href,

                Author: uf.pendingProps.username,
                Text: comment.text,
                Like: comment.likeCount,
                UnixTime: comment.postedAt,
            })
            return true;
        }
    })
})

return JSON.stringify(comments);