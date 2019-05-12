// JavaScript code that the site requires

/** Control using cookies according to the user's selection. */
(function () {
    var buttonCookieConsent = document.querySelector("#cookie-consent button[data-cookie-string]");
    if (buttonCookieConsent != null) {
        buttonCookieConsent.addEventListener("click", function (_event) {
            document.cookie = buttonCookieConsent.dataset.cookieString;
            location.reload(); // reload the page
        }, false);
    }
})();

/** Change the color of the menu icon when the menu for the navigation bar is collapsed or expanded with the help of styles from Bootstrap 4. */
var buttonMenu = document.getElementById("navbar-menu");
if (buttonMenu != null) {
    buttonMenu.onclick = function () {
        var menuIcon = document.getElementById("navbar-menu-icon");
        // when the button is clicked to collapse/expand the menu, the value of the attribute "aria-expanded" is "true"/"false" and then will be changed to "false"/"true" after executing the following
        if (buttonMenu.getAttribute("aria-expanded") == "true") {
            menuIcon.classList.remove("text-success");
            menuIcon.classList.add("text-black-50");
        } else {
            menuIcon.classList.remove("text-black-50");
            menuIcon.classList.add("text-success");
        }
    }
};

/** Enable the feature of scrolling to top. */
window.onload = function () {
    var buttonScrollToTop = document.getElementById("button-scroll-to-top");
    var timerScrollToTop = null;
    var isTop = true;
    // execute when the user scrolls with the bar
    window.onscroll = function () {
        // get the distance between the top and the current position of the scroll bar
        var distanceFromTop = document.documentElement.scrollTop || document.body.scrollTop;
        if (distanceFromTop > 100) {
            buttonScrollToTop.style.display = "block"; // display the button for scrolling to top when the user scrolls with the bar for a specified distance
        } else {
            buttonScrollToTop.style.display = "none";
        };
        // stop the timer if the user scrolls with the bar during the process of scrolling to top
        if (!isTop) {
            clearInterval(timerScrollToTop);
        };
        isTop = false;
    };
    buttonScrollToTop.onclick = function () {
        // set a timer
        timerScrollToTop = setInterval(function () {
            // get the distance between the top and the current position of the scroll bar
            var distanceFromTop = document.documentElement.scrollTop || document.body.scrollTop;
            var speed = Math.floor(-distanceFromTop / 7);
            document.documentElement.scrollTop = document.body.scrollTop = distanceFromTop + speed;
            // stop the timer after scrolling to top
            if (distanceFromTop == 0) {
                clearInterval(timerScrollToTop);
            };
            isTop = true;
        }, 30);
    };
};

/** Hide or show the password in the text box for the password according to the user's selection with the help of styles from Bootstrap 4 and Font Awesome 5. */
function hideOrShowPassword() {
    var passwordEye = document.getElementById("password-eye");
    var textBoxPassword = document.getElementById("Input_Password");
    if (textBoxPassword.type == "password") {
        textBoxPassword.type = "text";
        passwordEye.classList.remove("fa-eye-slash");
        passwordEye.classList.add("e-active", "fa-eye", "fa-flip-horizontal");
        passwordEye.title = "Click to hide the password."
    } else {
        textBoxPassword.type = "password";
        passwordEye.classList.remove("e-active", "fa-eye", "fa-flip-horizontal");
        passwordEye.classList.add("fa-eye-slash");
        passwordEye.title = "Click to show the password."
    }
};

/** Hide or show the confirmation password in the text box for the confirm password according to the user's selection with the help of styles from Bootstrap 4 and Font Awesome 5. */
function hideOrShowConfirmationPassword() {
    var confirmationPasswordEye = document.getElementById("confirmation-password-eye");
    var textBoxConfirmPassword = document.getElementById("Input_ConfirmPassword");
    if (textBoxConfirmPassword.type == "password") {
        textBoxConfirmPassword.type = "text";
        confirmationPasswordEye.classList.remove("fa-eye-slash");
        confirmationPasswordEye.classList.add("e-active", "fa-eye", "fa-flip-horizontal");
        confirmationPasswordEye.title = "Click to hide the confirmation password."
    } else {
        textBoxConfirmPassword.type = "password";
        confirmationPasswordEye.classList.remove("e-active", "fa-eye", "fa-flip-horizontal");
        confirmationPasswordEye.classList.add("fa-eye-slash");
        confirmationPasswordEye.title = "Click to show the confirmation password."
    }
};

/** Enable the ripple effect for certain controls with the help of styles from Essential JS 2. */
ej.base.enableRipple(true);
var inputObject = {};
var input = document.querySelectorAll(".e-input-group .e-input, .e-float-input.e-input-group input");
var inputIcon = document.querySelectorAll(".e-input-group-icon");
var focusFunction = function (index) {
    return function () {
        getParentNode(input[index]).classList.add("e-input-focus");
    };
};
var blurFunction = function (index) {
    return function () {
        getParentNode(input[index]).classList.remove("e-input-focus");
    };
};
var mouseupFunction = function () {
    var element = this;
    setTimeout(function () {
        element.classList.remove("e-input-btn-ripple");
    }, 500);
};
for (var i = 0; i < input.length; i++) {
    input[i].addEventListener("focus", focusFunction(i));
    input[i].addEventListener("blur", blurFunction(i));
}
for (var j = 0; j < inputIcon.length; j++) {
    inputIcon[j].addEventListener("mousedown", function () {
        this.classList.add("e-input-btn-ripple");
    });
    inputIcon[j].addEventListener("mouseup", mouseupFunction);
}
function getParentNode(element) {
    var parentNode = element.parentNode;
    if (parentNode.classList.contains("e-input-in-wrap")) {
        return parentNode.parentNode;
    }
    return parentNode;
};

// customise the message content of an uploader
ej.base.L10n.load({
    "customised": {
        "uploader": {
            "Browse": "",
            "Clear": "",
            "Upload": "",
            "dropFilesHint": "Or drop files here.",
            "invalidMaxFileSize": "The file size is too large.",
            "invalidMinFileSize": "The file size is too small.",
            "invalidFileType": "The file type is not allowed.",
            "uploadFailedMessage": "The file failed to upload. Try refreshing the page or retrying.",
            "uploadSuccessMessage": "The file uploaded successfully. Refresh the page to check.",
            "removedSuccessMessage": "The file removed successfully. Relevant URL changed to default.",
            "removedFailedMessage": "Unable to remove the file.",
            "inProgress": "Uploading...",
            "readyToUploadMessage": "Ready to upload.",
            "abort": "Abort the file upload.",
            "remove": "Remove the file.",
            "cancel": "Cancel the file upload.",
            "delete": "Delete the file.",
            "pauseUpload": "The file upload paused.",
            "pause": "Pause the file upload.",
            "resume": "Resume the file upload.",
            "retry": "Retry.",
            "fileUploadCancel": "The file upload cancelled."
        }
    }
});

var labelCoverPhoto = document.getElementById("label-cover-photo");
/** Activate the label for the cover photo. */
function activateLabelCoverPhoto() {
    if (labelCoverPhoto != null) {
        labelCoverPhoto.classList.add("active");
    }
}
/** Deactivate the label for the cover photo. */
function deactivateLabelCoverPhoto() {
    if (labelCoverPhoto != null) {
        labelCoverPhoto.classList.remove("active");
    }
};