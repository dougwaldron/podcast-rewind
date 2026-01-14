// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Store timeout ID to prevent race conditions with rapid clicks
let copyTimeoutId = null;

// Icon paths
const CLIPBOARD_ICON = '/img/clipboard.svg';
const CLIPBOARD_CHECK_ICON = '/img/clipboard-check.svg';

function copyToClipboard(caller) {
    let feedLink = caller.getAttribute('data-link')
    
    // Validate feedLink exists
    if (!feedLink) {
        console.error('No data-link attribute found on button');
        return;
    }
    
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
            img.src = CLIPBOARD_CHECK_ICON
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
