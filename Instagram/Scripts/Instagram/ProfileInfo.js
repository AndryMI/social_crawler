
var id = document.querySelector('h2');
if (!id) {
    return "null";
}

var url = document.querySelector('header section a[target=_blank]');
var photo = document.querySelector('header img');
var name = document.querySelector('header section > div:last-child > span');
var description = document.querySelector('header section > div:last-child');

var following = document.querySelector('[href="' + document.location.pathname + 'following/"] span');
var followers = document.querySelector('[href="' + document.location.pathname + 'followers/"] span');

return JSON.stringify({
    Link: document.location.href,

    Id: id?.innerText?.trim(),
    Name: name?.innerText?.trim(),
    Description: description?.innerText?.trim(),
    Url: url?.href,

    PhotoImg: photo?.src,

    RawFollowing: following?.title || following?.innerText,
    RawFollowers: followers?.title || followers?.innerText
})