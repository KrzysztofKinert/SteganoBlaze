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

function getImageWidth() {
    var image = document.getElementById('carrierImage');
    return image.width;
}

function getImageHeight() {
    var image = document.getElementById('carrierImage');
    return image.height;
}

function getImageData() {
    var img = document.getElementById('carrierImage');
    var canvas = document.getElementById('canvas');
    canvas.width = img.width;
    canvas.height = img.height;
    var ctx = canvas.getContext('2d');
    ctx.drawImage(img, 0, 0);
    const data = ctx.getImageData(0, 0, img.width, img.height).data;
    return data;
}

function getImageURL(data, type) {
    var img = document.getElementById('carrierImage');
    var canvas = document.getElementById('canvas');
    var ctx = canvas.getContext('2d');

    let imageData = new ImageData(Uint8ClampedArray.from(data), img.width, img.height)

    for (let i = 0; i < imageData.data.length; i += 4) {
        imageData.data[i + 3] = 255;
    }

    ctx.putImageData(imageData, 0, 0);

    var dataURL = canvas.toDataURL(type, 1.0)
    return dataURL;
}
