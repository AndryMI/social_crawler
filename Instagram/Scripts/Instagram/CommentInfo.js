
var profile = document.querySelector('[role=dialog] header a');
var comments = [];

document.querySelectorAll('[role=dialog] article ul > div').forEach(function (comment) {

    var text = comment.innerText.split('\n');
    var header = text.shift();
    var footer = text.pop();
    var time = comment.querySelector('time');

    comments.push({
        ProfileLink: profile?.href,
        PostLink: document.location.href,
        Link: time.closest('a')?.href || document.location.href,

        Header: header,
        Body: text.join('\n'),
        Footer: footer,
        Time: time.dateTime,
    });
});

return JSON.stringify(comments);
