
var header = __FindProps(document.querySelector('header'), props => props.user);
if (!header) {
    return "null";
}

return JSON.stringify({
    Link: document.location.href,

    Id: header.user.username,
    Name: header.user.fullName,
    Description: header.user.bio,
    Url: header.user.website,

    IsBusinessAccount: header.user.isBusinessAccount,
    IsPrivate: header.user.isPrivate,
    IsProfessionalAccount: header.user.isProfessionalAccount,
    IsVerified: header.user.isVerified,

    PhotoImg: header.user.profilePictureUrl,

    Following: header.user.counts?.follows ?? 0,
    Followers: header.user.counts?.followedBy ?? 0,
});