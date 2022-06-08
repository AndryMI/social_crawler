
var tweets = [];

document.querySelectorAll('[data-testid=tweet]').forEach(function (tweet) {

    // Unwrap emoji
    tweet.querySelectorAll('[data-testid=tweetText] img').forEach(function (img) {
        img.outerHTML = img.alt
    });

    // Unwrap links
    tweet.querySelectorAll('[data-testid=tweetText] a').forEach(function (a) {
        if (a.href.startsWith('https://t.co/')) {
            a.outerHTML = a.href
        }
    });

    const time = tweet.querySelector('time');
    const text = tweet.querySelector('[data-testid=tweetText]');

    const reply = tweet.querySelector('[data-testid=reply]');
    const retweet = tweet.querySelector('[data-testid=retweet]');
    const like = tweet.querySelector('[data-testid=like]');

    const attach = tweet.querySelector('[aria-labelledby]')

    tweets.push({
        Link: time.closest('a').href,
        Time: time.dateTime,
        Text: text?.innerText?.trim(),

        Reply: parseInt(reply?.getAttribute('aria-label')) || 0,
        Retweet: parseInt(retweet?.getAttribute('aria-label')) || 0,
        Like: parseInt(like?.getAttribute('aria-label')) || 0,

        Attach: {
            Videos: Array.from(attach?.querySelectorAll('video') ?? []).map(video => video.src),
            Images: Array.from(attach?.querySelectorAll('img') ?? []).map(video => video.src),
            Text: attach?.innerText?.trim(),
        }
    });
});

return JSON.stringify(tweets);