
var profile = document.querySelector('section header a');
var image = document.querySelector('section img');
var video = document.querySelector('section video');
var time = document.querySelector('section time');

return JSON.stringify({
    ProfileLink: profile?.href,
    Link: document.location.href,
    StoryImg: image?.src,
    VideoUrl: video?.src || video?.querySelector('source')?.src,
    Time: time?.dateTime,
});