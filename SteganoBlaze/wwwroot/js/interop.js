function getImageWidth() {
    var image = document.getElementById('carrierImage');
    return image.width;
}
function getImageHeight() {
    var image = document.getElementById('carrierImage');
    return image.height;
}

function getImageData() {
    var myimage = document.getElementById('carrierImage');
    var canvas = document.getElementById('canvas');
    canvas.width = myimage.width;
    canvas.height = myimage.height;
    var ctx = canvas.getContext('2d');
    ctx.drawImage(myimage, 0, 0);
    const data = ctx.getImageData(0, 0, myimage.width, myimage.height).data;
    return data;
}

function getImageURL(data, type) {
    var myimage = document.getElementById('carrierImage');
    var canvas = document.getElementById('canvas');
    var ctx = canvas.getContext('2d');

    let imageData = new ImageData(Uint8ClampedArray.from(data), myimage.width, myimage.height)

    for (let i = 0; i < imageData.data.length; i += 4) {
        imageData.data[i + 3] = 255;
    }

    ctx.putImageData(imageData, 0, 0);

    var dataURL = canvas.toDataURL(type, 1.0)
    return dataURL;
}

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