
var posts = []

Array.from(document.querySelectorAll('.post')).slice(-50).forEach(post => {

    post.querySelector('.wall_post_more')?.click()

    // Unwrap emoji
    post.querySelectorAll('img.emoji').forEach(function (img) {
        img.outerHTML = img.alt
    });

    var quote = post.querySelector('.copy_post_date a')
    var link = post.querySelector('.post_link')
    var time = post.querySelector('.post_date')
    var text = post.querySelector('.wall_post_text')

    var reactions = JSON.parse(post.querySelector('[data-reaction-counts]')?.dataset?.reactionCounts ?? '[]').reduce((a, b) => a + b, 0)
    var shares = JSON.parse(post.querySelector('[data-count].share')?.dataset?.count ?? '0')
    var views = parseInt(post.querySelector('.like_views').title) || 0

    var media = []

    // Unwrap text links
    text?.querySelectorAll('a')?.forEach(function (a) {
        a.outerHTML = a.title || a.innerText
    })

    post.querySelectorAll('.wall_post_cont a').forEach(a => {
        if (a.href) {
            var match = a.style.backgroundImage.match(/url\(['"]?(.*?)['"]?\)/i)
            media.push({
                Url: a.href,
                Image: match ? match[1] : a.style.backgroundImage,
            })
        }
    })

    posts.push({
        Link: link.href,

        Text: text?.innerText?.trim(),
        Media: media,
        QuoteLink: quote?.href,
        Time: time?.innerText?.trim(),

        Reactions: reactions,
        Shares: shares,
        Views: views,
    })
})

return JSON.stringify(posts);
