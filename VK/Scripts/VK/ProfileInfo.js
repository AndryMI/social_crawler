
var type = null
if (document.getElementById('profile')) {
    type = 'Profile'
}
if (document.getElementById('public')) {
    type = 'Public'
}
if (document.getElementById('group')) {
    type = 'Group'
}
if (!type) {
    return 'null'
}

function CssBackgroundImage(el) {
    if (el) {
        var match = el.style.backgroundImage.match(/url\(['"]?(.*?)['"]?\)/i)
        return match ? match[1] : el.style.backgroundImage
    }
    return null
}

// Unwrap emoji
document.querySelectorAll('img.emoji').forEach(function (img) {
    img.outerHTML = img.alt
});

var name = document.querySelector('h1')
var status = document.querySelector('#page_current_info')
var photo = document.querySelector('#page_avatar img') || document.querySelector('.page_cover_image img')
var header = CssBackgroundImage(document.querySelector('.page_cover'))

var url = Array.from(document.querySelectorAll('.profile_info a')).map(a => a.href).find(u => u.indexOf('/away.php') >= 0)
       || document.querySelector('.group_info_row.site a')?.href

var description = []

document.querySelectorAll('.profile_info_row').forEach(row => {
    var label = row.querySelector('.label')
    var value = row.querySelector('.labeled')
    description.push({
        Key: label?.innerText?.trim(),
        Value: value?.innerText?.trim(),
    })
})

document.querySelectorAll('.group_info_row').forEach(row => {
    row.querySelector('.wall_post_more')?.click()
    var label = row.title || row.className.replace('group_info_row', '').trim()
    var value = row.querySelector('.line_value')
    description.push({
        Key: label,
        Value: value?.innerText?.trim(),
    })
})

return JSON.stringify({
    Link: document.location.href,
    Type: type,

    Name: name?.innerText?.trim(),
    Status: status?.innerText?.trim(),
    Description: description,
    Url: url,

    PhotoImg: photo?.src,
    HeaderImg: header,
})