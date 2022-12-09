
function ExtractUsername(link) {
    try {
        link = decodeURIComponent(link ?? '')
    } catch { }
    const url = new URL(link ?? '', document.location.origin)
    const match = url.href.match(/\/profile\.php\?\W*id=(\d+)/)
    if (match) {
        return match[1]
    }
    return url.pathname.replace(/^\/+|\/+$/g, '') || null
}
function Validated(item) {
    const name = ExtractUsername(item.url)
    return name && name == ExtractUsername(document.location.href) ? item : null
}
function ParseTimelineContextItem(item) {
    const title = item.renderer?.context_item?.title
    if (!title) {
        return null
    }
    let external = title.ranges?.[0]?.entity?.external_url
    if (external && external.startsWith(document.location.origin)) {
        external = null
    }
    return {
        Key: item.timeline_context_list_item_type,
        Value: external ?? title.text
    }
}
function ScanProfileSocialContext(context, uri_part) {
    if (!context) {
        return null
    }
    const item = context.content?.find(x => x.uri?.indexOf(uri_part) >= 0)
    if (!item) {
        return null
    }
    return item.text?.text
}

var timeline_context_items = []
var profile_status_text = null
var user = {}
var page = {}
__WalkObjectRecursive(__GetCurrentFacebookRequestsDump(), (key, value) => {
    if (key == 'user') {
        user = Object.assign({}, value, user)
    }
    else if (key == 'page') {
        page = Object.assign({}, value, page)
    }
    else if (key == 'timeline_context_item') {
        timeline_context_items.push(ParseTimelineContextItem(value))
    }
    else if (key == 'profile_status_text') {
        profile_status_text = value?.text
    }
})
user = Validated(user);
page = Validated(page);

if (!user && !page) {
    return "null";
}
if (user && page) {
    throw new Error('Page have both "user" and "page" definitions')
}

user = user ?? page

return JSON.stringify({
    Link: user.url,
    Type: user.__typename,

    FacebookId: user.id,
    Id: ExtractUsername(user.url),
    Name: user.name,
    Gender: user.gender,
    Description: [
        profile_status_text,
        user.page_about_fields?.blurb,
        user.page_about_fields?.description?.text,
        user.page_about_fields?.impressum?.text,
    ].filter(x => x).join('\n\n'),

    HeaderImg: user.cover_photo?.photo?.image?.uri
        || user.comet_page_cover_renderer?.content?.[0]?.photo?.image?.uri,

    PhotoImg: user.profilePicLarge?.uri
        || user.profile_picture?.uri,

    IsVerified: user.is_verified,
    IsVisiblyMemorialized: user.is_visibly_memorialized,
    IsAdditionalProfilePlus: user.is_additional_profile_plus,

    RawLike: user.page_likers?.global_likers_count?.toString()
        || ScanProfileSocialContext(user.profile_social_context, 'friends_likes'),

    RawFollowers: user.follower_count?.toString() 
        || ScanProfileSocialContext(user.profile_social_context, 'followers'),

    Info: [
        user.were_here_count && { Key: 'were_here_count', Value: user.were_here_count.toString() },
        user.overall_star_rating && { Key: 'overall_star_rating', Value: user.overall_star_rating.value + '/' + user.overall_star_rating.opinion_count },

        user.page_about_fields?.email && { Key: 'email', Value: user.page_about_fields.email.text },
        user.page_about_fields?.website && { Key: 'website', Value: user.page_about_fields.website },
        user.page_about_fields?.formatted_phone_number && { Key: 'phone', Value: user.page_about_fields.formatted_phone_number },

        ...user.page_about_fields?.page_categories?.map(cat => ({ Key: 'category_name', Value: cat?.text })) ?? [],

        ...user.page_about_fields?.other_accounts?.map(acc => ({ Key: 'other_accounts', Value: acc?.uri?.url })) ?? [],

        ...timeline_context_items,

    ].filter(x => x),
});