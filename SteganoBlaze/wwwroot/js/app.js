﻿function saveFile(file) {
    var saveLink = document.getElementById('saveLink');
    saveLink.download = file.fileName;
    saveLink.href = "data:" + file.contentType + ";base64," + file.base64Data;
    saveLink.click();
}

function getLanguage() {
    return navigator.language;
}

function isDarkMode() {
    if (window.matchMedia) {
        if (window.matchMedia('(prefers-color-scheme: dark)').matches) {
            return true;
        }
    }
    return false;
}

function disableWebP() {
    let userAgent = navigator.userAgent;
    let browserName;

    if (userAgent.match(/chrome|chromium|crios/i)) {
        browserName = "chrome";
    } else if (userAgent.match(/firefox|fxios/i)) {
        browserName = "firefox";
    } else if (userAgent.match(/safari/i)) {
        browserName = "safari";
    } else if (userAgent.match(/opr\//i)) {
        browserName = "opera";
    } else if (userAgent.match(/edg/i)) {
        browserName = "edge";
    } else {
        browserName = "No browser detection";
    }

    if (browserName == "firefox" || browserName == "safari") return true;
    else return false;
}