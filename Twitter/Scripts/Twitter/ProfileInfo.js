﻿
var username = document.querySelector('[data-testid="UserName"]');
if (!username) {
    return "null";
}

var description = document.querySelector('[data-testid="UserDescription"]');
var location = document.querySelector('[data-testid="UserLocation"]');
var url = document.querySelector('[data-testid="UserUrl"]');
var joindate = document.querySelector('[data-testid="UserJoinDate"]');

var header = document.querySelector('[href="' + document.location.pathname + '/header_photo"] img');
var photo = document.querySelector('[href="' + document.location.pathname + '/photo"] img');
var following = document.querySelector('[href="' + document.location.pathname + '/following"]');
var followers = document.querySelector('[href="' + document.location.pathname + '/followers"]');

var { 0: name, 1: id } = username?.innerText?.split('\n');

// Unwrap emoji
description?.querySelectorAll('img')?.forEach(function (img) {
    img.outerHTML = img.alt
});

// Unwrap links
description?.querySelectorAll('a')?.forEach(function (a) {
    if (a.href.startsWith('https://t.co/')) {
        a.outerHTML = a.href
    }
});

return JSON.stringify({
    Link: document.location.href,

    Id: id?.trim(),
    Name: name?.trim(),
    Description: description?.innerText?.trim(),
    Location: location?.innerText?.trim(),
    Url: url?.href,

    HeaderImg: header?.src,
    PhotoImg: photo?.src,

    RawFollowing: following?.querySelector('span')?.innerText,
    RawFollowers: followers?.querySelector('span')?.innerText,
    RawJoinDate: joindate?.innerText?.trim(),
})