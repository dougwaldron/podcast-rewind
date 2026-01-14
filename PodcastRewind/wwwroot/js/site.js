// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

function copyToClipboard(caller) {
    let feedLink = caller.getAttribute('data-link')
    navigator.clipboard.writeText(feedLink).then(() => {
        // Get the image element inside the button
        const img = caller.querySelector('img')
        if (img) {
            // Store the original src
            const originalSrc = img.src
            // Change to checkmark icon
            img.src = '/img/clipboard-check.svg'
            // Update the button title for accessibility
            const originalTitle = caller.title
            caller.title = 'Copied!'
            
            // Restore the original icon after 2 seconds
            setTimeout(() => {
                img.src = originalSrc
                caller.title = originalTitle
            }, 2000)
        }
    }).catch(err => {
        console.error('Failed to copy RSS link to clipboard: ', err)
        // Update title to show error
        caller.title = 'Failed to copy. Please try again.'
        setTimeout(() => {
            caller.title = 'Copy RSS link to clipboard'
        }, 3000)
    })
}
