const {
    generateAchievement,
    getExportedAchievement,
    saveExportedStudentAchievement,
    saveExportedStudentAchievementFromWeb
} = require("../services/achievement-templates/achievement-template-detail.service");
const { handleResponseInfo } = require("../helpers/response-info-helper");

module.exports = {
    generateAchievement: async (req, res, next) => {
        try {
            const currentUser = req.user;
            const { courseId } = req.body;

            if (!currentUser) {
                return res.status(401).json({ message: "Unauthorized user" });
            }

            if (!courseId) {
                return res
                    .status(400)
                    .json({ message: "Invalid request body" });
            }

            const responseInfo = await generateAchievement(
                courseId,
                currentUser.userId
            );

            res.json(handleResponseInfo("achievement", responseInfo));

            if (responseInfo.statusCode === 200) {
                setImmediate(async () => {
                    // Do something after sending response
                    await saveExportedStudentAchievement(
                        courseId,
                        currentUser.userId,
                        responseInfo.data.achievement.achievementURL
                    );
                });
            }
        } catch (error) {
            next(error);
        }
    },
    saveExportedStudentAchievement: async (req, res, next) => {
        try {
            const currentUser = req.user;
            const { courseId } = req.body;
            const achievementFile = req.file;

            if (!currentUser) {
                return res.status(401).json({ message: "Unauthorized user" });
            }

            if (!courseId || !achievementFile) {
                return res
                    .status(400)
                    .json({ message: "Invalid request body" });
            }

            const responseInfo = await saveExportedStudentAchievementFromWeb(
                courseId,
                currentUser.userId,
                currentUser.fullName,
                achievementFile
            );

            res.json(handleResponseInfo("achievement", responseInfo));
        } catch (error) {
            next(error);
        }
    },
    getAchievementInCourse: async (req, res, next) => {
        try {
            const currentUser = req.user;
            const { courseId } = req.params;

            if (!currentUser) {
                return res.status(401).json({ message: "Unauthorized user" });
            }

            if (!courseId) {
                return res
                    .status(400)
                    .json({ message: "Invalid request query" });
            }

            const exportedAchievementInCourse = await getExportedAchievement(
                courseId,
                currentUser.userId
            );

            return res.json(
                handleResponseInfo("achievement", exportedAchievementInCourse)
            );
        } catch (error) {
            next(error);
        }
    }
};
