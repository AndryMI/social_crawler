
function ExtractUsername(link) {
    const url = new URL(link ?? '', 'http://localhost')
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
__WalkObjectRecursive(Array.from(document.querySelectorAll('[data-dump]')).map(x => JSON.parse(x.text)), (key, value) => {
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

if (user) {
    return JSON.stringify({
        Link: user.url,
        Type: user.__typename,

        FacebookId: user.id,
        Id: ExtractUsername(user.url),
        Name: user.name,
        Gender: user.gender,
        Description: profile_status_text,

        HeaderImg: user.cover_photo?.photo?.image?.uri,
        PhotoImg: user.profilePicLarge?.uri,

        IsVerified: user.is_verified,
        IsVisiblyMemorialized: user.is_visibly_memorialized,
        IsAdditionalProfilePlus: user.is_additional_profile_plus,

        RawLike: ScanProfileSocialContext(user.profile_social_context, '/friends_likes/'),
        RawFollowers: ScanProfileSocialContext(user.profile_social_context, '/followers/'),

        Info: timeline_context_items.filter(x => x),
    });
}

if (page) {
    return JSON.stringify({
        Link: page.url,
        Type: page.__typename,

        FacebookId: page.id,
        Id: ExtractUsername(page.url),
        Name: page.name,
        Description: [
            page.page_about_fields?.blurb,
            page.page_about_fields?.description?.text,
            page.page_about_fields?.impressum?.text,
        ].filter(x => x).join('\n\n'),

        HeaderImg: page.comet_page_cover_renderer?.content?.[0]?.photo?.image?.uri,
        PhotoImg: page.profile_picture?.uri,

        IsVerified: page.is_verified,

        RawLike: page.page_likers?.global_likers_count?.toString(),
        RawFollowers: page.follower_count?.toString(),

        Info: [
            page.were_here_count && { Key: 'were_here_count', Value: page.were_here_count.toString() },
            page.overall_star_rating && { Key: 'overall_star_rating', Value: page.overall_star_rating.value + '/' + page.overall_star_rating.opinion_count },

            page.page_about_fields?.email && { Key: 'email', Value: page.page_about_fields.email.text },
            page.page_about_fields?.website && { Key: 'website', Value: page.page_about_fields.website },
            page.page_about_fields?.formatted_phone_number && { Key: 'phone', Value: page.page_about_fields.formatted_phone_number },

            ...page.page_about_fields?.page_categories?.map(cat => ({ Key: 'category_name', Value: cat?.text })),

            ...page.page_about_fields?.other_accounts?.map(acc => ({ Key: 'other_accounts', Value: acc?.uri?.url }))

        ].filter(x => x),
    });
}