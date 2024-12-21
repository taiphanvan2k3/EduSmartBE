const { logInfo, logError } = require("./logger.service");
const { createCanvas, loadImage } = require("canvas");
const { uploadCloudinary } = require("../helpers/init-cloudinary");
const { formatDateTime } = require("../helpers/utils");
const path = require("path");

class TextStyle {
    constructor(fontFamily, fontSize, color) {
        this.fontFamily = fontFamily;
        this.fontSize = fontSize;
        this.color = color;
    }
}

/**
 * Create an achievement image for a student
 * @param {number} templateId
 * @param {string} studentName
 * @param {string} courseName
 * @param {string} teacherName
 * @param {TextStyle} courseNameTextStyle
 * @param {TextStyle} studentNameTextStyle
 * @param {TextStyle} dateTextStyle
 * @param {TextStyle} teacherNameTextStyle
 */
async function createAchievement(
    templateId,
    studentName,
    courseName,
    teacherName,
    studentNameTextStyle,
    courseNameTextStyle,
    dateTextStyle,
    teacherNameTextStyle
) {
    const caller = "createAchievement";
    try {
        logInfo(caller, "Creating achievement image...");
        const templatePath = path.join(
            __dirname,
            `../assets/achievement-templates/${templateId}.png`
        );

        // Load the original image
        const image = await loadImage(templatePath);

        // Tạo canvas và context
        const canvas = createCanvas(image.width, image.height);
        const ctx = canvas.getContext("2d");

        // Vẽ ảnh lên canvas
        ctx.drawImage(image, 0, 0);

        // ====== Student Name ======
        setTextStyle(ctx, studentNameTextStyle);
        modifyFontSize(
            ctx,
            studentName,
            image.width,
            Number.parseFloat(studentNameTextStyle.fontSize)
        );
        const nameTextWidth = ctx.measureText(studentName).width;
        const namePosition = {
            x: (image.width - nameTextWidth) / 2, // Căn giữa tên sinh viên
            y: 395 // Tọa độ y
        };
        ctx.fillText(studentName, namePosition.x, namePosition.y);

        // ====== Course Name ======
        setTextStyle(ctx, courseNameTextStyle);
        modifyFontSize(
            ctx,
            courseName,
            image.width,
            Number.parseFloat(courseNameTextStyle.fontSize)
        );
        const courseTextWidth = ctx.measureText(courseName).width;
        const coursePosition = {
            x: (image.width - courseTextWidth) / 2, // Căn giữa tên khóa học
            y: 500 // Tọa độ y
        };
        ctx.fillText(courseName, coursePosition.x, coursePosition.y);

        // ====== Date ======
        ctx.font = `bold 30px "Dancing Script"`;
        ctx.fillStyle = dateTextStyle.color;
        ctx.fillText("Date", 255, 600);

        const date = new Date();
        const dateString = formatDateTime(date);
        setTextStyle(ctx, dateTextStyle);
        ctx.fillText(dateString, 227, 648);

        // ====== Teacher Name ======
        ctx.font = `bold 30px "Dancing Script"`;
        ctx.fillStyle = teacherNameTextStyle.color;
        ctx.fillText("Teacher", 825, 600);

        // Canh giữa teacherName qua toạ độ x = 857
        setTextStyle(ctx, teacherNameTextStyle);
        const teacherNameTextWidth = ctx.measureText(teacherName).width;
        const teacherNamePosition = {
            x: 860 - teacherNameTextWidth / 2,
            y: 648
        };

        setTextStyle(ctx, teacherNameTextStyle);
        ctx.fillText(teacherName, teacherNamePosition.x, teacherNamePosition.y);

        const buffer = canvas.toBuffer("image/jpeg");

        const uploadResult = await uploadCloudinary(
            buffer,
            "student-achievements",
            "image",
            `${studentName}-${date.getTime()}.jpg`
        );

        logInfo(caller, "Achievement image created successfully!");
        return uploadResult.secure_url;
    } catch (error) {
        logError(caller, error.message);
        throw error;
    }
}

const setTextStyle = (ctx, textStyle) => {
    ctx.font = `${textStyle.fontSize} "Dancing Script"`;
    ctx.fillStyle = textStyle.color;
};

const modifyFontSize = (ctx, text, maxWidth, currentFontSize) => {
    let currentTextWidth = ctx.measureText(text).width;
    while (currentTextWidth >= maxWidth || maxWidth - currentTextWidth <= 100) {
        currentFontSize -= 2;
        ctx.font = `${currentFontSize}px "Dancing Script"`;
        currentTextWidth = ctx.measureText(text).width;
    }
};

module.exports = {
    createAchievement
};
