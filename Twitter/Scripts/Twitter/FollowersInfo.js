
var followers = [];

document.querySelectorAll('[data-testid=UserCell]').forEach(function (follower) {

    const link = follower.querySelector('a')
    const photo = follower.querySelector('a img')

    const name = Array.from(follower.querySelectorAll('a')).find(a => a.innerText && a.href == link.href)
    const description = Array.from(follower.querySelectorAll('[dir=auto]'))?.pop()

    // Unwrap emoji
    name?.querySelectorAll('img')?.forEach(function (img) {
        img.outerHTML = img.alt
    });
    // Unwrap emoji
    description?.querySelectorAll('img')?.forEach(function (img) {
        img.outerHTML = img.alt
    });

    followers.push({
        Link: link.href,

        Name: name?.innerText?.trim(),
        Description: description?.id ? null : description?.innerText?.trim(),
        PhotoUrl: photo?.src,
    });
});

return JSON.stringify(followers);