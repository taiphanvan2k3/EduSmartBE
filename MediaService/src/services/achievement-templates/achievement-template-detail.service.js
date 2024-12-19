const pool = require("../../configs/database");
const { v4: uuidv4 } = require("uuid");
const { logInfo, logError } = require("../logger.service");
const {
    createEarlyErrorResponse
} = require("../../helpers/response-info-helper");

async function createTemplateForCourse(courseTemplateRequest) {
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

        const errorMessage = await isValidCourseTemplateCreationRequest(
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
        return rows[0];
    } catch (error) {
        logError(caller, error);
        throw error;
    } finally {
        logInfo(caller, "End");
    }
}

const isValidCourseTemplateCreationRequest = async (
    courseId,
    templateId,
    isDefault
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
        } else if (row.AchievementTemplateId == templateId) {
            errorMessage = "Template is existed in course";
            break;
        }
    }

    return errorMessage;
};

module.exports = {
    createTemplateForCourse
};
