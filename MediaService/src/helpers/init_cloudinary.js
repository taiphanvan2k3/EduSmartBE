const cloudinary = require("cloudinary").v2;
cloudinary.config({
    cloud_name: process.env.CLOUDINARY_CLOUD_NAME,
    api_key: process.env.CLOUDINARY_API_KEY,
    api_secret: process.env.CLOUDINARY_API_SECRET
});

const uploadCloudinary = async (fileBuffer, folderName, resourceType = "auto", fileName = "") => {
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
                } else {
                    resolve(result);
                }
            })
            .end(fileBuffer);
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
    deleteCloudinary
};
