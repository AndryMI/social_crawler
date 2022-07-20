
var profile = document.querySelector('[role=dialog] header a');

var video = document.querySelector('[role=dialog] article video');
var image = document.querySelector('[role=dialog] article img');

var likes = document.querySelector('[href="' + document.location.pathname + 'liked_by/"] span')
var time = document.querySelector('[href="' + document.location.pathname + '"] time')

return JSON.stringify({
    ProfileLink: profile?.href,
    Link: document.location.href,

    ImageUrl: (video?.poster ?? image?.src),
    VideoUrl: video?.src,

    Like: likes?.innerText,

    Time: time?.dateTime,
});
