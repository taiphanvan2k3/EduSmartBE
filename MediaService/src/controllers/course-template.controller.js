const {
    getDefaultCourseTemplateInCourse,
    getCourseTemplateById,
    createTemplateForCourse,
    updateTemplateForCourse,
    deleteCourseTemplate
} = require("../services/course-templates/course-template-detail.service");
const { handleResponseInfo } = require("../helpers/response-info-helper");

module.exports = {
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
            const responseInfo = await getCourseTemplateById(courseTemplateId);

            return res.json(handleResponseInfo("courseTemplate", responseInfo));
        } catch (error) {
            next(error);
        }
    },
    createCourseTemplate: async (req, res, next) => {
        try {
            const {
                templateId,
                studentNameTextStyle,
                courseNameTextStyle,
                dateTextStyle,
                teacherNameTextStyle
            } = req.body;

            if (
                !templateId ||
                templateId <= 0 ||
                templateId > 6 ||
                !studentNameTextStyle ||
                !courseNameTextStyle ||
                !dateTextStyle ||
                !teacherNameTextStyle
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
                templateId,
                studentNameTextStyle,
                courseNameTextStyle,
                dateTextStyle,
                teacherNameTextStyle
            } = req.body;

            if (
                !templateId ||
                templateId <= 0 ||
                templateId > 6 ||
                !studentNameTextStyle ||
                !courseNameTextStyle ||
                !dateTextStyle ||
                !teacherNameTextStyle
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
