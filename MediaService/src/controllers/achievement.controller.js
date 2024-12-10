const {
    getAllAchievements
} = require("../services/achievements/list-of-achievements.service");

module.exports = {
    getAchievements: async (req, res, next) => {
        try {
            const achievements = await getAllAchievements();
            res.json(achievements);
        } catch (error) {
            console.log(error.message);
            next(error);
        }
    }
};
