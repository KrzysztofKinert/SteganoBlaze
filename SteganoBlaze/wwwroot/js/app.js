function saveFile(file) {
    var saveLink = document.getElementById('saveLink');
    saveLink.download = file.fileName;
    saveLink.href = "data:" + file.contentType + ";base64," + file.base64Data;
    saveLink.click();
}

function getPageTitle() {
    return document.title;
}

function getCulture() {
    return window.localStorage['Culture'];
}

function setCulture(value) {
    window.localStorage['Culture'] = value
}

function getDarkMode() {
    return window.localStorage['DarkMode'];
}

function setDarkMode(value) {
    window.localStorage['DarkMode'] = value
}