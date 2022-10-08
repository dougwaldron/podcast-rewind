// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

function copyToClipboard(caller) {
    let feedLink = caller.getAttribute('data-link')
    navigator.clipboard.writeText(feedLink)
}
