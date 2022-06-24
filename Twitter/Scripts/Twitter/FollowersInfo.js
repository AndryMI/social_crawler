
var followers = [];

document.querySelectorAll('[data-testid=UserCell]').forEach(function (follower) {

    const link = follower.querySelector('a').href

    followers.push({
        Link: link
    });
});

return JSON.stringify(followers);