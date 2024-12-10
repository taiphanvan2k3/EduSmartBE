const pool = require("../../configs/database");

async function getAllAchievements() {
    try {
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
        throw error;
    }
}

module.exports = {
    getAllAchievements
};
