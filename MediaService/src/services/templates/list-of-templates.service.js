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

module.exports = {
    getAllTemplates
};
