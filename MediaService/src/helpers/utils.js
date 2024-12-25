const fs = require("fs");
const path = require("path");

const formatDateTime = (date) => {
    const day = date.getDate();
    const month = date.getMonth() + 1;
    const year = date.getFullYear();

    let formattedDate = "";
    if (day < 10) {
        formattedDate += "0";
    }
    formattedDate += `${day}/`;

    if (month < 10) {
        formattedDate += "0";
    }
    formattedDate += `${month}/${year}`;

    return formattedDate;
};

const createFolderIfNotExist = (folderPath) => {
    if (!fs.existsSync(folderPath)) {
        fs.mkdirSync(folderPath, { recursive: true });
    }
};

const saveAchievementLocally = (achievementFile, studentName) => {
    const date = new Date();

    const pathToProject = process.cwd();
    const localFolderPath = path.join(
        pathToProject,
        "public",
        "media-service",
        "temp_achievements"
    );

    createFolderIfNotExist(localFolderPath);

    const fileExtension = achievementFile.originalname.split(".").pop();

    const fileName = `${studentName}_${date.getTime()}.${fileExtension}`;
    const localPath = `${localFolderPath}/${fileName}`;
    const localPathInPublic = `/media-service/temp_achievements/${fileName}`;

    fs.writeFileSync(localPath, achievementFile.buffer);

    return {
        localPath,
        localPathInPublic
    };
};

module.exports = {
    createFolderIfNotExist,
    saveAchievementLocally,
    formatDateTime
};
