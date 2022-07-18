
var comments = [];

function CssBackgroundImage(el) {
    if (el) {
        var match = el.style.backgroundImage.match(/url\(['"]?(.*?)['"]?\)/i)
        return match ? match[1] : el.style.backgroundImage
    }
    return null
}

Array.from(document.querySelectorAll('.reply')).slice(-500).forEach(reply => {

    // Unwrap emoji
    reply.querySelectorAll('img.emoji').forEach(function (img) {
        img.outerHTML = img.alt
    });

    var date = reply.querySelector('.reply_date a')
    var author = reply.querySelector('.author');
    var mention = reply.querySelector('[mention_id]');
    var text = reply.querySelector('.wall_reply_text')
    var likes = JSON.parse(reply.querySelector('[data-count].like')?.dataset?.count ?? '0')

    var media = []

    // Unwrap text links
    text?.querySelectorAll('a')?.forEach(function (a) {
        a.outerHTML = a.title || a.innerText
    })

    reply.querySelectorAll('.reply_text a').forEach(a => {
        media.push({
            Url: a.href || null,
            Image: CssBackgroundImage(a) || a.querySelector('img')?.src,
        })
    })

    comments.push({
        Link: date.href,

        Author: author?.innerText?.trim(),
        AuthorUrl: author?.href,
        MentionUrl: mention?.href,
        Media: media,
        Text: text?.innerText?.trim(),
        Time: date?.innerText?.trim(),

        Likes: likes,
    })
})

return JSON.stringify(comments);
