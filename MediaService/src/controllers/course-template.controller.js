const {
    getAllTemplates,
    getCourseTemplates
} = require("../services/achievement-templates/list-of-achievement-templates.service");
const {
    getDefaultCourseTemplateInCourse,
    getCourseTemplateById,
    createTemplateForCourse,
    updateTemplateForCourse,
    deleteCourseTemplate,
    generateAchievement,
    saveExportedStudentAchievement,
    getExportedAchievement
} = require("../services/achievement-templates/achievement-template-detail.service");
const { handleResponseInfo } = require("../helpers/response-info-helper");

module.exports = {
    getAllTemplates: async (req, res, next) => {
        try {
            const templates = await getAllTemplates();
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
    getDefaultCourseTemplate: async (req, res, next) => {
        try {
            const { courseId } = req.query;
            if (!courseId) {
                return res
                    .status(400)
                    .json({ message: "Invalid request query" });
            }

            const responseInfo =
                await getDefaultCourseTemplateInCourse(courseId);
            if (responseInfo.statusCode === 200) {
                return res.json(
                    handleResponseInfo("courseTemplate", responseInfo)
                );
            }

            return res.json(responseInfo);
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
            return res.json(handleResponseInfo("id", responseInfo));
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

            res.json(handleResponseInfo("courseTemplate", courseTemplate));
        } catch (error) {
            next(error);
        }
    },
    deleteCourseTemplate: async (req, res, next) => {
        try {
            const courseTemplateId = req.params.id;
            const responseInfo = await deleteCourseTemplate(courseTemplateId);
            res.json(handleResponseInfo("id", responseInfo));
        } catch (error) {
            next(error);
        }
    }
};
