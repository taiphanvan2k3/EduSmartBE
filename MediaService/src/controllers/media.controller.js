const createError = require("http-errors");
const {
    uploadCloudinary,
    deleteCloudinary
} = require("../helpers/init_cloudinary");
const MediaService = require("../services/media.service");

module.exports = {
    getMedia: async (req, res, next) => {
        try {
            // Lấy thông tin user từ req.user
            const { userId } = req.user;

            // Gọi service để lấy thông tin storage của user
            const storageInfo = await MediaService.getUserStorageInfo(userId);

            // Trả về thông tin storage của user
            res.json(storageInfo);
        } catch (error) {
            console.log(error.message);
            next(error);
        }
    },
    uploadFile: async (req, res, next) => {
        try {
            const { userId } = req.user;
            const file = req.file; // File received from multer

            if (!file) {
                throw new createError.BadRequest("No file provided!");
            }

            const fileSizeInKB = file.size / 1024; // Convert bytes to KB

            // Check if there is enough storage
            const hasEnoughStorage = await MediaService.isEnoughStorage(
                userId,
                fileSizeInKB
            );
            if (!hasEnoughStorage) {
                return res.status(400).json({
                    error: "Dung lượng đã vượt quá giới hạn, vui lòng nâng cấp để tiếp tục sử dụng."
                });
            }

            // Proceed with file upload to Cloudinary
            const result = await uploadCloudinary(file.buffer, "files");
            const fileUrl = result.secure_url;
            const filePublicId = result.public_id;

            // Update user's storage usage in DB
            await MediaService.updateStorageInfo(userId, fileSizeInKB);

            res.json({
                message: "File uploaded successfully",
                data: {
                    fileUrl,
                    filePublicId
                }
            });
        } catch (error) {
            console.log(error.message);
            next(error);
        }
    }
};
