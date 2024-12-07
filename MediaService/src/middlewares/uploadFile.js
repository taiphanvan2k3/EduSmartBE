const multer = require("multer");

// Sử dụng bộ nhớ tạm để lưu trữ file trước khi upload lên Cloudinary
const storage = multer.memoryStorage();

const upload = multer({
    storage,
    limits: {
        fileSize: 5 * 1024 * 1024 // Giới hạn kích thước file 5MB
    },
    fileFilter: (req, file, cb) => {
        const allowedMimes = [
            "image/jpeg", "image/png", "image/gif", // Image
            "audio/mpeg", "audio/wav", "audio/ogg", // Audio
            "application/pdf", // PDF
            "application/msword", "application/vnd.openxmlformats-officedocument.wordprocessingml.document", // Word (doc, docx)
            "application/vnd.ms-excel", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", // Excel (xls, xlsx)
            "text/plain", // Text
            "application/zip", "application/x-tar", "application/x-gzip", "application/x-rar-compressed", // Archive files
            "application/rtf", "application/json" // Other document types
        ];

        if (allowedMimes.includes(file.mimetype)) {
            cb(null, true);
        } else {
            cb(new Error("Invalid file type. Only JPEG, PNG, GIF, PDF, DOC, DOCX, MP3, WAV, OGG are allowed."));
        }
    }
});

module.exports = {
    upload
};
