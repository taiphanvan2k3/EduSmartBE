const {
    getAllTemplates
} = require("../services/achievement-templates/list-of-achievement-templates.service");

module.exports = {
    getAllTemplates: async (req, res, next) => {
        try {
            const templates = await getAllTemplates();
            res.json(templates);
        } catch (error) {
            next(error);
        }
    }
};
