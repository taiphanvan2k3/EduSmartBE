const multer = require('multer');

// Sử dụng bộ nhớ tạm để lưu trữ file trước khi upload lên Cloudinary
const storage = multer.memoryStorage();

const upload = multer({
    storage,
    limits: {
        fileSize: 5 * 1024 * 1024, // Giới hạn kích thước file 5MB
    },
    fileFilter: (req, file, cb) => {
        // Chỉ cho phép các định dạng ảnh
        if (file.mimetype.startsWith('image/')) {
            cb(null, true);
        } else {
            cb(new Error('Only image files are allowed!'), false);
        }
    },
});

module.exports = {
    upload,
}