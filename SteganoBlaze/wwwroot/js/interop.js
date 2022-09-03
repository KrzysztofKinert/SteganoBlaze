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

const getPasswordKey = (password) =>
    window.crypto.subtle.importKey(
        "raw",
        password,
        "PBKDF2",
        false,
        ["deriveKey"]
    );

const deriveKey = (passwordKey, salt, keyUsage) =>
    window.crypto.subtle.deriveKey(
        {
            name: "PBKDF2",
            salt: salt,
            iterations: 250000,
            hash: "SHA-256",
        },
        passwordKey,
        { name: "AES-CBC", length: 256 },
        false,
        keyUsage
    );

async function encryptData(secretData, password, salt, iv) {
    try {
        const passwordKey = await getPasswordKey(password);
        const aesKey = await deriveKey(passwordKey, salt, ["encrypt"]);
        const encryptedContent = await window.crypto.subtle.encrypt(
            {
                name: "AES-CBC",
                iv: iv,
            },
            aesKey,
            secretData
        );
        return encryptedContent;

    } catch (e) {
        console.log(`Error - ${e}`);
        return "";
    }
}
async function decryptData(encryptedData, password, salt, iv) {
    try {
        const passwordKey = await getPasswordKey(password);
        const aesKey = await deriveKey(passwordKey, salt, ["decrypt"]);
        const decryptedData = await window.crypto.subtle.decrypt(
            {
                name: "AES-CBC",
                iv: iv,
            },
            aesKey,
            encryptedData
        );
        return decryptedData;
    } catch (e) {
        throw new Error();
        console.log(`Error - ${e}`);
        return "";
    }
}