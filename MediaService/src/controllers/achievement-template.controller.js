const {
    getCourseTemplates,
    getAllAchievementTemplates
} = require("../services/achievement-templates/list-of-achievement-templates.service");
const {
    getCourseTemplateById,
    createTemplateForCourse,
    updateTemplateForCourse,
    deleteCourseTemplate,
    generateAchievement
} = require("../services/achievement-templates/achievement-template-detail.service");

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
            if (!courseId) {
                return res
                    .status(400)
                    .json({ message: "Invalid request query" });
            }

            const templates = await getCourseTemplates(courseId);
            res.json(templates);
        } catch (error) {
            next(error);
        }
    },
    getCourseTemplateById: async (req, res, next) => {
        try {
            const courseTemplateId = req.params.id;
            const courseTemplate =
                await getCourseTemplateById(courseTemplateId);

            if (!courseTemplate) {
                return res
                    .status(404)
                    .json({ message: "Course template not found" });
            }

            res.json(courseTemplate);
        } catch (error) {
            next(error);
        }
    },
    createCourseTemplate: async (req, res, next) => {
        try {
            const {
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

            const responseInfo = await createTemplateForCourse(req.body);
            res.json(responseInfo);
        } catch (error) {
            next(error);
        }
    },
    updateCourseTemplate: async (req, res, next) => {
        try {
            const courseTemplateId = req.params.id;
            const {
                studentStyle,
                courseNameStyle,
                dateStyle,
                teacherNameStyle
            } = req.body;

            if (
                !studentStyle ||
                !courseNameStyle ||
                !dateStyle ||
                !teacherNameStyle
            ) {
                return res
                    .status(400)
                    .json({ message: "Invalid request body" });
            }

            const courseTemplate = await updateTemplateForCourse(
                courseTemplateId,
                req.body
            );

            res.json(courseTemplate);
        } catch (error) {
            next(error);
        }
    },
    deleteCourseTemplate: async (req, res, next) => {
        try {
            const courseTemplateId = req.params.id;
            const responseInfo = await deleteCourseTemplate(courseTemplateId);
            res.json(responseInfo);
        } catch (error) {
            next(error);
        }
    },
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

            res.json(responseInfo);
        } catch (error) {
            next(error);
        }
    }
};
