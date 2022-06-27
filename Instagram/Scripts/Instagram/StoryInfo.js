
var image = document.querySelector('section img');
var video = document.querySelector('section video');

return JSON.stringify({
    Link: document.location.href,
    ImageUrl: image?.src,
    VideoUrl: video?.src || video?.querySelector('source')?.src,
});