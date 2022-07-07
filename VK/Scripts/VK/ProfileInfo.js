
if (!document.getElementById('profile')) {
    return 'null'
}

var name = document.querySelector('h1')
var status = document.querySelector('#page_current_info')
var photo = document.querySelector('#page_avatar img')

var url = Array.from(document.querySelectorAll('.profile_info a')).map(a => a.href).find(u => u.indexOf('/away.php') >= 0)

var description = []

document.querySelectorAll('.profile_info_row').forEach(row => {
    var label = row.querySelector('.label')
    var value = row.querySelector('.labeled')
    description.push({
        Key: label?.innerText?.trim(),
        Value: value?.innerText?.trim(),
    })
})

return JSON.stringify({
    Link: document.location.href,

    Name: name?.innerText?.trim(),
    Status: status?.innerText?.trim(),
    Description: description,
    Url: url,

    PhotoImg: photo?.src,
})