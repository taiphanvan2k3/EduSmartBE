const pool = require("../../configs/database");
const { v4: uuidv4 } = require("uuid");
const { logInfo, logError } = require("../logger.service");
const {
    createEarlyErrorResponse,
    createResponseInfo
} = require("../../helpers/response-info-helper");
const { checkStudentCompletedCourse } = require("../grpc/grpc-course.service");
const { createAchievement } = require("../canvas.service");
const {
    uploadCloudinary,
    deleteCloudinary
} = require("../../helpers/init-cloudinary");

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
            SELECT "Id", "CourseId", "AchievementTemplateId", "CourseNameTextStyle", "StudentNameTextStyle", "DateTextStyle", "TeacherNameTextStyle", "IsDefault"
            FROM "CourseAchievementTemplates"
            WHERE "CourseId" = $1 AND "IsDefault" = true
        `;

        const { rows } = await pool.query(query, [courseId]);
        let courseTemplate = null;
        if (rows.length > 0) {
            courseTemplate = {
                id: rows[0].Id,
                courseId: rows[0].CourseId,
                achievementTemplateId: rows[0].AchievementTemplateId,
                courseNameTextStyle: rows[0].CourseNameTextStyle,
                studentNameTextStyle: rows[0].StudentNameTextStyle,
                dateTextStyle: rows[0].DateTextStyle,
                teacherNameTextStyle: rows[0].TeacherNameTextStyle,
                isDefault: rows[0].IsDefault
            };
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
        return rows[0];
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
            studentStyle,
            courseNameStyle,
            dateStyle,
            teacherNameStyle
        } = courseTemplateRequest;

        const errorMessage = await isValidCourseTemplateRequest(
            courseId,
            templateId,
            isDefault
        );

        if (errorMessage) {
            return createEarlyErrorResponse(400, "Bad Request", errorMessage);
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
            courseNameStyle,
            studentStyle,
            dateStyle,
            teacherNameStyle,
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
            isDefault,
            studentStyle,
            courseNameStyle,
            dateStyle,
            teacherNameStyle
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

        const errorMessage = await isValidCourseTemplateRequest(
            courseTemplateRows[0].CourseId,
            courseTemplateRows[0].AchievementTemplateId,
            isDefault,
            true
        );

        if (errorMessage) {
            return createEarlyErrorResponse(400, "Bad Request", errorMessage);
        }

        const query = `
            UPDATE "CourseAchievementTemplates"
            SET
                "CourseNameTextStyle" = $1,
                "StudentNameTextStyle" = $2,
                "DateTextStyle" = $3,
                "TeacherNameTextStyle" = $4,
                "IsDefault" = $5
            WHERE "Id" = $6
            RETURNING *;
        `;

        const values = [
            courseNameStyle,
            studentStyle,
            dateStyle,
            teacherNameStyle,
            isDefault,
            courseTemplateId
        ];

        const { rows } = await pool.query(query, values);
        return createResponseInfo("courseTemplate", rows[0]);
    } catch (error) {
        logError(caller, error);
        throw error;
    } finally {
        logInfo(caller, "End");
    }
};

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

/**
 * Generate achievement for student who completed the course
 * @author TaiPV
 * @createdDate 2024/12/21
 * @param {Guid} courseId
 * @param {number} studentId
 */
const generateAchievement = async (courseId, studentId) => {
    const caller = "generateAchievement";
    try {
        logInfo(caller, "Start");
        if (await checkIsExportedAchievement(courseId, studentId)) {
            return createEarlyErrorResponse(
                400,
                "Bad Request",
                "Achievement has already been exported"
            );
        }

        const grpcResponse = await checkStudentCompletedCourse(
            courseId,
            studentId
        );

        if (!grpcResponse.isSuccess) {
            return createEarlyErrorResponse(
                500,
                "Internal Server Error",
                grpcResponse.message
            );
        }

        if (!grpcResponse.isCompleted) {
            return createEarlyErrorResponse(
                400,
                "Bad Request",
                "Student has not completed the course"
            );
        }

        const defaultTemplate = await getDefaultTemplateOfCourse(courseId);
        if (!defaultTemplate) {
            return createEarlyErrorResponse(
                404,
                "Not Found",
                "Your teacher has not created any template for this course"
            );
        }

        const achievementURL = await createAchievement(
            defaultTemplate.AchievementTemplateId,
            grpcResponse.studentName,
            grpcResponse.courseName,
            grpcResponse.teacherName,
            defaultTemplate.StudentNameTextStyle,
            defaultTemplate.CourseNameTextStyle,
            defaultTemplate.DateTextStyle,
            defaultTemplate.TeacherNameTextStyle
        );

        logInfo(caller, "End");
        return createResponseInfo("achievement", {
            courseId,
            achievementURL,
            createdAt: new Date().toISOString()
        });
    } catch (error) {
        logError(caller, error);
    }
};

/**
 * Save the achievement that student has exported to the database
 * @param {Guid} courseId
 * @param {number} studentId
 */
const saveExportedStudentAchievementFromWeb = async (
    courseId,
    studentId,
    studentName,
    achievementFile
) => {
    const caller = "saveExportedStudentAchievement";
    let publicId = null;

    try {
        logInfo(caller, "Start");

        if (await checkIsExportedAchievement(courseId, studentId)) {
            return createEarlyErrorResponse(
                400,
                "Bad Request",
                "Achievement has already been exported in this course"
            );
        }

        const isCompletedValidation = await validateCompletedCourse(
            courseId,
            studentId
        );

        if (isCompletedValidation.statusCode !== 200) {
            return isCompletedValidation;
        }

        const date = new Date();
        const uploadResponse = await uploadCloudinary(
            achievementFile.buffer,
            "achievements",
            "auto",
            `${studentName}_${date.getTime()}`
        );

        publicId = uploadResponse?.public_id;

        const query = `
            INSERT INTO "StudentAchievements" (
                "Id",
                "CourseId",
                "StudentId",
                "AchievementURL"
            )
            VALUES ($1, $2, $3, $4)
            RETURNING "AchievementURL", "CourseId", "CreatedAt";
        `;

        const values = [
            uuidv4(),
            courseId,
            studentId,
            uploadResponse.secure_url
        ];

        const { rows } = await pool.query(query, values);
        if (rows.length == 0) {
            return createEarlyErrorResponse(
                500,
                "Internal Server Error",
                "Error saving achievement"
            );
        }

        return createResponseInfo("achievement", {
            courseId: rows[0].CourseId,
            achievementURL: rows[0].AchievementURL,
            createdAt: rows[0].CreatedAt
        });
    } catch (error) {
        if (publicId) {
            await deleteCloudinary(publicId);
        }

        logError(caller, error);
        throw error;
    } finally {
        logInfo(caller, "End");
    }
};

/**
 * Save the achievement that student has exported to the database
 * @param {Guid} courseId
 * @param {number} studentId
 * @param {string} achievementURL
 */
const saveExportedStudentAchievement = async (
    courseId,
    studentId,
    achievementURL
) => {
    const caller = "saveExportedStudentAchievement";
    try {
        logInfo(caller, "Start");

        const query = `
            INSERT INTO "StudentAchievements" (
                "Id",
                "CourseId",
                "StudentId",
                "AchievementURL"
            )
            VALUES ($1, $2, $3, $4)
        `;

        const values = [uuidv4(), courseId, studentId, achievementURL];
        await pool.query(query, values);
    } catch (error) {
        logError(caller, error);
    } finally {
        logInfo(caller, "End");
    }
};

/**
 * Get the exported achievement of a student in a course
 * @author TaiPV
 * @createdDate 2024/12/22
 * @param {Guid} courseId
 * @param {number} studentId
 */
const getExportedAchievement = async (courseId, studentId) => {
    const caller = "getExportedAchievement";
    try {
        logInfo(caller, "Start");

        const query = `
            SELECT "AchievementURL", "CreatedAt"
            FROM "StudentAchievements"
            WHERE "CourseId" = $1 AND "StudentId" = $2
        `;

        const { rows } = await pool.query(query, [courseId, studentId]);
        if (rows.length == 0) {
            return createEarlyErrorResponse(
                404,
                "Not Found",
                "Achievement not found"
            );
        }

        return createResponseInfo("achievement", {
            achievementURL: rows[0].AchievementURL,
            createdAt: rows[0].CreatedAt
        });
    } catch (error) {
        logError(caller, error);
    } finally {
        logInfo(caller, "End");
    }
};

/**
 * Check if the request is valid for creating a course template
 * @author TaiPV
 * @createdDate 2024/12/22
 * @param {Guid} courseId
 * @param {number} templateId - Range from 1 to 6
 * @param {boolean} isDefault - True if the template is default
 * @returns {Promise<string>} - Error message if the request is invalid, otherwise return empty string
 */
const isValidCourseTemplateRequest = async (
    courseId,
    templateId,
    isDefault,
    isEdit = false
) => {
    // Kiểm tra xem đã thêm template này chưa và đã tồn tại template nào là default chưa
    const checkQuery = `
        SELECT "AchievementTemplateId", "IsDefault"
        FROM "CourseAchievementTemplates"
        WHERE "CourseId" = $1 AND ("AchievementTemplateId" = $2 OR "IsDefault" = true)
    `;
    const checkValues = [courseId, templateId];

    const { rows } = await pool.query(checkQuery, checkValues);
    if (rows.length == 0) {
        return "";
    }

    let errorMessage = "";
    for (let row of rows) {
        if (
            isDefault &&
            row.IsDefault &&
            row.AchievementTemplateId != templateId
        ) {
            errorMessage = "Default template is existed in course";
            break;
        } else if (row.AchievementTemplateId == templateId && !isEdit) {
            errorMessage = "Template is existed in course";
            break;
        }
    }

    return errorMessage;
};

/**
 * Get the default template of the course
 * @author TaiPV
 * @createdDate 2024/12/22
 * @param {Guid} courseId
 * @returns
 */
const getDefaultTemplateOfCourse = async (courseId) => {
    const caller = "getDefaultTemplateForCourse";
    try {
        logInfo(caller, "Start");

        const query = `
            SELECT "AchievementTemplateId", "CourseNameTextStyle", "StudentNameTextStyle", "DateTextStyle", "TeacherNameTextStyle"
            FROM "CourseAchievementTemplates"
            WHERE "CourseId" = $1
            ORDER BY "IsDefault" DESC
        `;

        const { rows } = await pool.query(query, [courseId]);
        return rows[0];
    } catch (error) {
        logError(caller, error);
    } finally {
        logInfo(caller, "End");
    }
};

const checkIsExportedAchievement = async (courseId, studentId) => {
    const caller = "checkIsExportedAchievement";
    try {
        logInfo(caller, "Start");
        const query = `
            SELECT 1 FROM "StudentAchievements"
            WHERE "CourseId" = $1 AND "StudentId" = $2
        `;

        const { rows } = await pool.query(query, [courseId, studentId]);
        return rows.length > 0;
    } catch (error) {
        logError(caller, error);
    }
};

const validateCompletedCourse = async (courseId, studentId) => {
    const grpcResponse = await checkStudentCompletedCourse(courseId, studentId);
    if (!grpcResponse.isSuccess) {
        return createEarlyErrorResponse(
            500,
            "Internal Server Error",
            grpcResponse.message
        );
    }

    if (!grpcResponse.isCompleted) {
        return createEarlyErrorResponse(
            400,
            "Bad Request",
            "Student has not completed the course"
        );
    }

    return createResponseInfo("statusCode", 200);
};

module.exports = {
    getDefaultCourseTemplateInCourse,
    getCourseTemplateById,
    createTemplateForCourse,
    updateTemplateForCourse,
    deleteCourseTemplate,
    generateAchievement,
    saveExportedStudentAchievement,
    saveExportedStudentAchievementFromWeb,
    getExportedAchievement
};
