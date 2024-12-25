const {
    getAllTemplates
} = require("../services/templates/list-of-templates.service");

module.exports = {
    getAllTemplates: async (_, res, next) => {
        try {
            const templates = await getAllTemplates();
            res.json(templates);
        } catch (error) {
            next(error);
        }
    }
};
