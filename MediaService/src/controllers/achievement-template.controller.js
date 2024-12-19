const {
    createTemplateForCourse
} = require("../services/achievement-templates/achievement-template-detail.service");
const {
    getCourseTemplates,
    getAllAchievementTemplates
} = require("../services/achievement-templates/list-of-achievement-templates.service");

module.exports = {
    getAchievementTemplates: async (req, res, next) => {
        try {
            const templates = await getAllAchievementTemplates();
            res.json(templates);
        } catch (error) {
            next(error);
        }
    },
    getCourseTemplates: async (req, res, next) => {
        try {
            const { courseId } = req.query;
            const templates = await getCourseTemplates(courseId);
            res.json(templates);
        } catch (error) {
            next(error);
        }
    },
    createCourseTemplate: async (req, res, next) => {
        try {
            const {
                isDefault,
                courseId,
                templateId,
                studentStyle,
                courseNameStyle,
                dateStyle,
                teacherNameStyle
            } = req.body;

            if (
                !templateId ||
                !studentStyle ||
                !courseNameStyle ||
                !dateStyle ||
                !teacherNameStyle
            ) {
                return res
                    .status(400)
                    .json({ message: "Invalid request body" });
            }

            const courseTemplate = await createTemplateForCourse({
                isDefault,
                courseId,
                templateId,
                studentStyle,
                courseNameStyle,
                teacherNameStyle,
                dateStyle
            });

            res.json(courseTemplate);
        } catch (error) {
            next(error);
        }
    }
};
