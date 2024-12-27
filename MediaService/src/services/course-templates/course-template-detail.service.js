const pool = require("../../configs/database");
const { v4: uuidv4 } = require("uuid");
const { logInfo, logError } = require("../logger.service");
const {
    createEarlyErrorResponse,
    createResponseInfo
} = require("../../helpers/response-info-helper");

/**
 * Get the default course template in a course
 * @author TaiPV
 * @createdDate 2024/12/24
 * @param {Guid} courseId
 * @returns
 */
const getDefaultCourseTemplateInCourse = async (courseId) => {
    const caller = "getDefaultCourseTemplateInCourse";
    try {
        logInfo(caller, "Start");

        const query = `
            SELECT c."Id", "CourseId", "AchievementTemplateId", t."TemplateURL", "CourseNameTextStyle", "StudentNameTextStyle", "DateTextStyle", "TeacherNameTextStyle", "IsDefault"
            FROM "CourseAchievementTemplates" c
            JOIN "AchievementTemplates" t ON c."AchievementTemplateId" = t."Id"
            WHERE "CourseId" = $1 AND "IsDefault" = true
        `;

        const { rows } = await pool.query(query, [courseId]);
        let courseTemplate = null;
        if (rows.length > 0) {
            courseTemplate = convertCourseTemplateToDto(rows[0]);
        }

        return createResponseInfo("courseTemplate", courseTemplate);
    } catch (error) {
        logError(caller, error);
        throw error;
    } finally {
        logInfo(caller, "End");
    }
};

/**
 * Get course template by id
 * @author TaiPV
 * @createdDate 2024/12/21
 * @param {Guid} courseTemplateId - Id of course template
 */
const getCourseTemplateById = async (courseTemplateId) => {
    const caller = "getCourseTemplateById";
    try {
        logInfo(caller, "Start");

        const query = `
            SELECT "CourseId", "AchievementTemplateId", "CourseNameTextStyle", "StudentNameTextStyle", "DateTextStyle", "TeacherNameTextStyle", "IsDefault"
            FROM "CourseAchievementTemplates"
            WHERE "Id" = $1
        `;

        const { rows } = await pool.query(query, [courseTemplateId]);

        if (rows.length == 0) {
            return createEarlyErrorResponse(
                404,
                "Not Found",
                "Course template not found"
            );
        }

        return createResponseInfo(
            "courseTemplate",
            convertCourseTemplateToDto(rows[0])
        );
    } catch (error) {
        logError(caller, error);
    } finally {
        logInfo(caller, "End");
    }
};

/**
 * Teacher creates a template for a course
 * @author TaiPV
 * @createdDate 2024/12/17
 * @param {object} courseTemplateRequest
 * @returns {Guid} - Id of created course template
 */
const createTemplateForCourse = async (courseTemplateRequest) => {
    const caller = "createTemplateForCourse";
    try {
        logInfo(caller, "Start");
        const {
            isDefault,
            courseId,
            templateId,
            studentNameTextStyle,
            courseNameTextStyle,
            dateTextStyle,
            teacherNameTextStyle
        } = courseTemplateRequest;

        // Check the course whether it has the template?
        const isExistTemplateQuery = `
            SELECT 1 FROM "CourseAchievementTemplates"
            WHERE "CourseId" = $1
        `;

        const { rows: isExistTemplateRows } = await pool.query(
            isExistTemplateQuery,
            [courseId]
        );

        if (isExistTemplateRows.length > 0) {
            return createEarlyErrorResponse(
                400,
                "Bad Request",
                "Course has already had a template"
            );
        }

        const query = `
            INSERT INTO "CourseAchievementTemplates" (
                "Id",
                "CourseId",
                "AchievementTemplateId",
                "CourseNameTextStyle",
                "StudentNameTextStyle",
                "DateTextStyle",
                "TeacherNameTextStyle",
                "IsDefault"
            )
            VALUES ($1, $2, $3, $4, $5, $6, $7, $8)
            RETURNING "Id";
        `;

        const values = [
            uuidv4(),
            courseId,
            templateId,
            courseNameTextStyle,
            studentNameTextStyle,
            dateTextStyle,
            teacherNameTextStyle,
            isDefault
        ];

        const { rows } = await pool.query(query, values);
        return createResponseInfo("id", rows[0].Id);
    } catch (error) {
        logError(caller, error);
        throw error;
    } finally {
        logInfo(caller, "End");
    }
};

/**
 * Teacher updates a template for a course
 * @author TaiPV
 * @createdDate 2024/12/17
 * @param {Guid} courseTemplateId - Id of course template
 * @param {object} courseTemplateRequest
 * @returns {Promise<object>} - Updated course template
 */
const updateTemplateForCourse = async (
    courseTemplateId,
    courseTemplateRequest
) => {
    const caller = "createTemplateForCourse";
    try {
        logInfo(caller, "Start");
        const {
            templateId,
            isDefault,
            studentNameTextStyle,
            courseNameTextStyle,
            dateTextStyle,
            teacherNameTextStyle
        } = courseTemplateRequest;

        const findCourseTemplateQuery = `
            SELECT "CourseId", "AchievementTemplateId"
            FROM "CourseAchievementTemplates"
            WHERE "Id" = $1
        `;

        const { rows: courseTemplateRows } = await pool.query(
            findCourseTemplateQuery,
            [courseTemplateId]
        );

        if (courseTemplateRows.length == 0) {
            return createEarlyErrorResponse(
                404,
                "Not Found",
                "Course template not found"
            );
        }

        const query = `
            UPDATE "CourseAchievementTemplates"
            SET
                "CourseNameTextStyle" = $1,
                "StudentNameTextStyle" = $2,
                "DateTextStyle" = $3,
                "TeacherNameTextStyle" = $4,
                "AchievementTemplateId" = $5,
                "IsDefault" = $6
            WHERE "Id" = $7
            RETURNING *;
        `;

        const values = [
            courseNameTextStyle,
            studentNameTextStyle,
            dateTextStyle,
            teacherNameTextStyle,
            templateId,
            isDefault,
            courseTemplateId
        ];

        const { rows } = await pool.query(query, values);
        if (rows.length == 0) {
            return createEarlyErrorResponse(
                500,
                "Internal Server Error",
                "Error updating course template"
            );
        }

        return createResponseInfo(
            "courseTemplate",
            convertCourseTemplateToDto(rows[0])
        );
    } catch (error) {
        logError(caller, error);
        throw error;
    } finally {
        logInfo(caller, "End");
    }
};

/**
 * Teacher deletes a course template by id
 * @author TaiPV
 * @createdDate 2024/12/21
 * @param {Guid} courseTemplateId: Id of course template
 * @returns
 */
const deleteCourseTemplate = async (courseTemplateId) => {
    const caller = "deleteCourseTemplate";
    try {
        logInfo(caller, "Start");

        const query = `
            DELETE FROM "CourseAchievementTemplates"
            WHERE "Id" = $1
        `;

        const { rowCount } = await pool.query(query, [courseTemplateId]);

        if (rowCount == 0) {
            return createEarlyErrorResponse(
                404,
                "Not Found",
                "Course template not found"
            );
        }

        return createResponseInfo("id", courseTemplateId);
    } catch (error) {
        logError(caller, error);
        throw error;
    } finally {
        logInfo(caller, "End");
    }
};

const convertCourseTemplateToDto = (courseTemplate) => {
    return {
        id: courseTemplate.Id,
        courseId: courseTemplate.CourseId,
        templateId: courseTemplate.AchievementTemplateId,
        templateURL: courseTemplate.TemplateURL,
        courseNameTextStyle: courseTemplate.CourseNameTextStyle,
        studentNameTextStyle: courseTemplate.StudentNameTextStyle,
        dateTextStyle: courseTemplate.DateTextStyle,
        teacherNameTextStyle: courseTemplate.TeacherNameTextStyle,
        isDefault: courseTemplate.IsDefault
    };
};

module.exports = {
    getDefaultCourseTemplateInCourse,
    getCourseTemplateById,
    createTemplateForCourse,
    updateTemplateForCourse,
    deleteCourseTemplate
};
