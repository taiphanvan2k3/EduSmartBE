const pool = require("../../configs/database");
const { logInfo, logError } = require("../logger.service");

/**
 * Get all achievement templates in the system
 * @author TaiPV
 * @createdDate 2024/12/13
 * @returns
 */
async function getAllTemplates() {
    const caller = "getAllAchievementTemplates";
    try {
        logInfo(caller, "Start");
        const { rows } = await pool.query(
            'select *from "AchievementTemplates"'
        );

        const templates = rows.map((row) => {
            return {
                id: row.Id,
                name: row.Name,
                thumbnailURL: row.ThumbnailURL,
                templateURL: row.TemplateURL
            };
        });

        return templates;
    } catch (error) {
        logError(caller, error);
        throw error;
    } finally {
        logInfo(caller, "End");
    }
}

/**
 * Get all templates which the teacher created for their courses
 * @author TaiPV
 * @createdDate 2024/12/15
 * @param {Guid} courseId
 * @returns
 */
async function getCourseTemplates(courseId) {
    const caller = "getCourseTemplates";
    try {
        logInfo(caller, "Start");
        const { rows } = await pool.query(
            'select *from "CourseAchievementTemplates" where "CourseId" = $1',
            [courseId]
        );

        const templates = rows.map((row) => {
            return {
                id: row.Id,
                achievementTemplateId: row.AchievementTemplateId,
                courseNameTextStyle: row.CourseNameTextStyle,
                studentNameTextStyle: row.StudentNameTextStyle,
                dateTextStyle: row.DateTextStyle,
                teacherNameTextStyle: row.TeacherNameTextStyle,
                isDefault: row.IsDefault
            };
        });

        return templates;
    } catch (error) {
        logError(caller, error);
        throw error;
    } finally {
        logInfo(caller, "End");
    }
}

module.exports = {
    getAllTemplates,
    getCourseTemplates
};
