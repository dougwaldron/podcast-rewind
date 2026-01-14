// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Store timeout ID to prevent race conditions with rapid clicks
let copyTimeoutId = null;

function copyToClipboard(caller) {
    let feedLink = caller.getAttribute('data-link')
    
    // Clear any existing timeout to prevent race conditions
    if (copyTimeoutId) {
        clearTimeout(copyTimeoutId);
    }
    
    navigator.clipboard.writeText(feedLink).then(() => {
        // Get the image element inside the button
        const img = caller.querySelector('img')
        if (img) {
            // Store the original src and title
            const originalSrc = img.src
            const originalTitle = caller.title
            
            // Change to checkmark icon
            img.src = '/img/clipboard-check.svg'
            // Update the button title for accessibility
            caller.title = 'Copied!'
            
            // Restore the original icon after 2 seconds
            copyTimeoutId = setTimeout(() => {
                img.src = originalSrc
                caller.title = originalTitle
                copyTimeoutId = null
            }, 2000)
        }
    }).catch(err => {
        console.error('Failed to copy RSS link to clipboard: ', err)
        // Store original title before modifying
        const originalTitle = caller.title
        // Update title to show error
        caller.title = 'Failed to copy. Please try again.'
        copyTimeoutId = setTimeout(() => {
            caller.title = originalTitle
            copyTimeoutId = null
        }, 3000)
    })
}
