const { logInfo, logError } = require("../services/logger.service");

const cloudinary = require("cloudinary").v2;
cloudinary.config({
    cloud_name: process.env.CLOUDINARY_CLOUD_NAME,
    api_key: process.env.CLOUDINARY_API_KEY,
    api_secret: process.env.CLOUDINARY_API_SECRET
});

const uploadCloudinary = async (
    fileBuffer,
    folderName,
    resourceType = "auto",
    fileName = ""
) => {
    logInfo("uploadCloudinary", "Uploading file to Cloudinary...");
    const uploadOptions = {
        folder: folderName,
        resource_type: resourceType
    };

    if (resourceType === "raw") {
        uploadOptions.public_id = fileName;
    }

    return new Promise((resolve, reject) => {
        cloudinary.uploader
            .upload_stream(uploadOptions, (error, result) => {
                if (error) {
                    reject(error);
                    logError(
                        "uploadCloudinary",
                        "Error uploading file to Cloudinary",
                        error
                    );
                } else {
                    resolve(result);
                    logInfo("uploadCloudinary", "File uploaded successfully");
                }
            })
            .end(fileBuffer);
    });
};

const uploadCloudinaryFromFilePath = async (filePath, folderName) => {
    logInfo("uploadCloudinaryFromFilePath", "Uploading file to Cloudinary...");
    const uploadOptions = {
        folder: folderName
    };

    return new Promise((resolve, reject) => {
        cloudinary.uploader.upload(filePath, uploadOptions, (error, result) => {
            if (error) {
                reject(error);
                logError(
                    "uploadCloudinaryFromFilePath",
                    "Error uploading file to Cloudinary",
                    error
                );
            } else {
                resolve(result);
                logInfo(
                    "uploadCloudinaryFromFilePath",
                    "File uploaded successfully"
                );
            }
        });
    });
};

const deleteCloudinary = async (publicId) => {
    return new Promise((resolve, reject) => {
        cloudinary.uploader.destroy(publicId, (error, result) => {
            if (error) {
                reject(error);
            } else {
                resolve(result);
            }
        });
    });
};

module.exports = {
    uploadCloudinary,
    uploadCloudinaryFromFilePath,
    deleteCloudinary
};
