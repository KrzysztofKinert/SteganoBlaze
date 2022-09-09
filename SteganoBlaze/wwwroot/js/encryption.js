﻿const getPasswordKey = (password) =>
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